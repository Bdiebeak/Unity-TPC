using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Параметры для поворота игрока к прицелу")]
    public float rotationStep = 15f;

    [Header("Параметры для поворота головы к прицелу")]
    public float xOffsetHead = 0f;
    public float yOffsetHead = 0f;
    [Tooltip("Так же использует для рук при прицеливании")]
    public float zOffsetHeadArms = 250f;

    [Header("Параметры для поднятия рук при прицеливании")]
    [Range(0, 1)]
    public float armsWeight = 1f;
    public float xOffsetLeftArm = -100f;
    public float xOffsetRightArm = 100f;
    public Transform rightWrist = null;
    public Transform leftWrist = null;
    public float timeForRaisingHands = 0.5f;

    private bool isRunning = false;
    private bool isAiming = false;
    private bool handsAreRaised = false;
    private float currentTimeForRisingHands;
    private float currentArmsWeight;
    private Vector2 runDirection;
    private Vector3 crosshairPosition;
    private Vector3 crosshairWorldPositionWithOffsets;
    private Vector3 rightArmAimPosition;
    private Vector3 leftArmAimPosition;

    private Camera mainCamera = null;
    private Animator playerAnimator = null;
    private Transform playerTransform = null;

    /// <summary>
    /// Инициализация параметров.
    /// Блокируем курсор.
    /// </summary>
    private void Start()
    {
        // Блокируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Инициализация нужных переменных
        mainCamera = Camera.main;
        playerAnimator = GetComponent<Animator>();
        playerTransform = transform;
    }

    /// <summary>
    /// Обработка ввода данных.
    /// </summary>
    private void Update()
    {
        #region Обработка ввода игрока
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }

        runDirection.x = Input.GetAxisRaw("Horizontal");
        runDirection.y = Input.GetAxisRaw("Vertical");
        runDirection = runDirection.normalized;

        isRunning = runDirection.magnitude == 0 ? false : true;

        // Координаты прицела + смещения из инспектора
        crosshairPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        crosshairWorldPositionWithOffsets = mainCamera.ScreenToWorldPoint(new Vector3(crosshairPosition.x + xOffsetHead,
                                                                                      crosshairPosition.y + yOffsetHead,
                                                                                      zOffsetHeadArms));

        leftArmAimPosition = mainCamera.ScreenToWorldPoint(new Vector3(crosshairPosition.x + xOffsetLeftArm,
                                                                       crosshairPosition.y, zOffsetHeadArms));

        rightArmAimPosition = mainCamera.ScreenToWorldPoint(new Vector3(crosshairPosition.x + xOffsetRightArm,
                                                                        crosshairPosition.y, zOffsetHeadArms));

        #endregion
    }

    private void FixedUpdate()
    {
        RotatePlayerToCrosshair();

        playerAnimator.SetFloat("moveHorizontal", runDirection.x);
        playerAnimator.SetFloat("moveVertical", runDirection.y);
    }

    /// <summary>
    /// Замена анимации. Обязательно происходит в LateUpdate.
    /// </summary>
    private void LateUpdate()
    {
        // Если игрок целится - ориентируем объекты запястьев по направлению к прицелу.
        // Вместе с запястьями будут вращаться и оружия.
        if (isAiming)
        {
            Vector3 rightDirection = crosshairWorldPositionWithOffsets - rightWrist.position;
            Vector3 leftDirection = crosshairWorldPositionWithOffsets - leftWrist.position;

            leftWrist.LookAt(leftDirection);
            rightWrist.LookAt(rightDirection);
        }
    }

    /// <summary>
    /// Inverse Kinematic.
    /// </summary>
    private void OnAnimatorIK()
    {
        playerAnimator.SetLookAtPosition(crosshairWorldPositionWithOffsets);
        playerAnimator.SetLookAtWeight(1.0f, 0.5f, 1.0f, 1.0f, 0.7f);

        // Вытягивание рук при прицеливании
        if (isAiming)
        {
            // Плавное поднятие рук
            handsAreRaised = true;

            if (currentTimeForRisingHands < timeForRaisingHands)
            {
                currentTimeForRisingHands += Time.deltaTime;
                currentArmsWeight = Mathf.Lerp(0, armsWeight, currentTimeForRisingHands / timeForRaisingHands);
            }
            else
            {
                currentTimeForRisingHands = timeForRaisingHands;
                currentArmsWeight = armsWeight;
            }

            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, currentArmsWeight);
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, currentArmsWeight);

            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightArmAimPosition);
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftArmAimPosition);
        }
        else
        {
            // Плавное опускание рук
            if (!handsAreRaised)
            {
                return;
            }

            if (currentTimeForRisingHands > 0)
            {
                currentTimeForRisingHands -= Time.deltaTime;
                currentArmsWeight = Mathf.Lerp(0, armsWeight, currentTimeForRisingHands / timeForRaisingHands);
            }
            else
            {
                currentTimeForRisingHands = 0;
                currentArmsWeight = 0;
                handsAreRaised = false;
            }

            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, currentArmsWeight);
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, currentArmsWeight);

            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightArmAimPosition);
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftArmAimPosition);
        }
    }

    /// <summary>
    /// Функция, вращения игрока к прицелу.
    /// </summary>
    private void RotatePlayerToCrosshair()
    {
        // Получаем текущий угол вращения камеры.
        float cameraYRotation = mainCamera.transform.rotation.eulerAngles.y;

        // Меняем угол поворота только по оси У.
        playerTransform.rotation = Quaternion.Slerp(transform.rotation,
                                                    Quaternion.Euler(0f, cameraYRotation, 0f),
                                                    rotationStep * Time.fixedDeltaTime);
    }
}
