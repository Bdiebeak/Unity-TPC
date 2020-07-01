using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Параметры для движения")]
    public float moveSpeed = 5f;

    [Header("Параметры для поворота игрока к прицелу")]
    public float rotationStep = 15f;

    [Header("Параметры для поворота головы и рук к прицелу")]
    public float zOffsetHeadArms = 250f;

    [Header("Параметры для поднятия рук при прицеливании")]
    [Range(0, 1)]
    public float armsWeight = 1f;
    public float xOffsetLeftArm = -100f;
    public float xOffsetRightArm = 100f;
    public float timeForRaisingHands = 0.5f;
    public Transform rightWrist = null;
    public Transform leftWrist = null;

    [Header("Параметры для прыжка")]
    public float jumpForce = 100f;
    public GameObject mainPlayerCollider = null;
    public GameObject jumpingPlayerCollider = null;
    public LayerMask checkingLayers;
    public float checkDistance = 1f;
    public float delayBeforeCheck = 1f;

    [Header("Параметры для замедления времени при прыжке")]
    [Range(0, 1)]
    public float slowedTime;
    public PostProcessVolume mainPPVolume = null;
    public PostProcessProfile mainPPProfile = null;
    public PostProcessProfile slowtimePPProfile = null;

    [Header("Параметры для стрельбы")]
    public Shooting shooting;

    private bool isRunning = false;
    private bool isAiming = false;
    private bool handsAreRaised = false;
    private bool isGrounded = true;
    private bool isStanding = true;
    private bool isTryingToStand = false;

    private float currentTimeForRisingHands = 0f;
    private float currentArmsWeight = 0f;
    private float neededTimeForGroundCheck = 0f;

    private Vector3 runDirection;
    private Vector3 crosshairPosition;
    private Vector3 crosshairWorldPositionWithOffsets;
    private Vector3 rightArmAimPosition;
    private Vector3 leftArmAimPosition;

    private Camera mainCamera = null;
    private Animator playerAnimator = null;
    private Transform playerTransform = null;
    private Rigidbody playerRigidbody = null;

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
        playerRigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Обработка ввода данных.
    /// </summary>
    private void Update()
    {
        runDirection.x = Input.GetAxisRaw("Horizontal");
        runDirection.z = Input.GetAxisRaw("Vertical");
        runDirection = runDirection.normalized;

        isRunning = runDirection.magnitude == 0 ? false : true;

        // Координаты прицела + смещения из инспектора
        crosshairPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        crosshairWorldPositionWithOffsets = mainCamera.ScreenToWorldPoint(new Vector3(crosshairPosition.x,
                                                                                      crosshairPosition.y,
                                                                                      zOffsetHeadArms));

        leftArmAimPosition = mainCamera.ScreenToWorldPoint(new Vector3(crosshairPosition.x + xOffsetLeftArm,
                                                                       crosshairPosition.y, zOffsetHeadArms));

        rightArmAimPosition = mainCamera.ScreenToWorldPoint(new Vector3(crosshairPosition.x + xOffsetRightArm,
                                                                        crosshairPosition.y, zOffsetHeadArms));

        // Прицеливание
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }

        if (Input.GetMouseButtonUp(1) && !Input.GetMouseButton(0))
        {
            isAiming = false;
        }

        // Стрельба
        if (Input.GetMouseButton(0))
        {
            isAiming = true;
            shooting.Shoot(crosshairWorldPositionWithOffsets);
        }

        if (Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1))
        {
            isAiming = false;
        }

        // Прыжок или поднятие
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && isStanding)
            {
                Jump();
            }
            else if (isGrounded && !isStanding && !isTryingToStand)
            {
                isTryingToStand = true;
                GetUp();
            }
        }
    }

    private void FixedUpdate()
    {
        // Если игрок в полете - передает в его Animator параметры для
        // корректного отоброжения положения тела в полете - 
        // анимация разворачивается в ту же сторону, в которую игрок целится.
        // --
        // Если игрок на земле - разворачиваем его в сторону прицела и
        // передаем параметры для корректного отображения анимации бега.
        if (!isStanding && !isTryingToStand)
        {
            // Получаем направление взгляда камеры.
            Vector3 aimDirection = mainCamera.transform.forward;
            aimDirection.y = 0f;
            aimDirection = aimDirection.normalized;
            aimDirection = playerTransform.InverseTransformDirection(aimDirection);

            playerAnimator.SetFloat("aimHorizontal", aimDirection.x);
            playerAnimator.SetFloat("aimVertical", aimDirection.z);

            CheckForGrounded();
        }
        else if (isGrounded && isStanding)
        {
            RotateToCrosshair();

            playerAnimator.SetFloat("moveHorizontal", runDirection.x);
            playerAnimator.SetFloat("moveVertical", runDirection.z);

            Vector3 runVector = playerTransform.TransformDirection(runDirection) * moveSpeed;
            runVector.y = playerRigidbody.velocity.y;

            playerRigidbody.velocity = runVector;
        }
    }

    /// <summary>
    /// Замена анимации. Обязательно происходит в LateUpdate.
    /// </summary>
    private void LateUpdate()
    {
        // Если игрок целится - ориентируем объекты запястьев по направлению к прицелу.
        // Вместе с запястьями будут вращаться и оружия.
        if ((isAiming || !isStanding) && !isTryingToStand)
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
        if ((isAiming || !isStanding) && !isTryingToStand)
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
        }

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, currentArmsWeight);
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, currentArmsWeight);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightArmAimPosition);
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftArmAimPosition);
    }

    /// <summary>
    /// Функция, вращения игрока к прицелу.
    /// </summary>
    private void RotateToCrosshair()
    {
        // Получаем текущий угол вращения камеры.
        float cameraYRotation = mainCamera.transform.rotation.eulerAngles.y;

        // Меняем угол поворота только по оси У.
        playerTransform.rotation = Quaternion.Slerp(transform.rotation,
                                                    Quaternion.Euler(0f, cameraYRotation, 0f),
                                                    rotationStep * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Функций, отвечающая за прыжок игрока.
    /// </summary>
    private void Jump()
    {
        // Обнуляем скорость игрока, чтобы он не пролетал больше нужного
        playerRigidbody.velocity = Vector3.zero;

        // Изменяем статус игрока
        isGrounded = false;
        isStanding = false;

        // Активируем коллайдер игрока в полете и деактивируем коллайдер для перемещения
        mainPlayerCollider.SetActive(false);
        jumpingPlayerCollider.SetActive(true);

        // Запуска анимации
        playerAnimator.SetTrigger("Jump");

        // Физика прыжка
        Vector3 jumpDirection;
        if (runDirection.magnitude == 0)
        {
            jumpDirection = playerTransform.forward;
        }
        else
        {
            jumpDirection = playerTransform.TransformDirection(runDirection);
        }
        jumpDirection = jumpDirection.normalized;

        playerRigidbody.AddForce((jumpDirection + Vector3.up) * playerRigidbody.mass * jumpForce);

        // Разворот игрока по направлению прыжка
        playerTransform.rotation = Quaternion.LookRotation(jumpDirection);

        // Замедляем время.
        SlowDownTime();

        // Время, после которого будет осуществляться проверка на приземление
        neededTimeForGroundCheck = Time.time + delayBeforeCheck;
    }

    /// <summary>
    /// Находится ли игрок на земле.
    /// </summary>
    private void CheckForGrounded()
    {
        if (Time.time < neededTimeForGroundCheck) return;

        CapsuleCollider playerCapsCollider = jumpingPlayerCollider.GetComponent<CapsuleCollider>();
        Bounds playerCapsColliderBounds = playerCapsCollider.bounds;
        Vector3 offsetVector = new Vector3(0f, 0f, playerCapsCollider.height / 2);

        Ray[] checkingRays = {
                                new Ray(playerCapsColliderBounds.center, Vector3.down),
                                new Ray(playerCapsColliderBounds.center + offsetVector, Vector3.down),
                                new Ray(playerCapsColliderBounds.center - offsetVector, Vector3.down),
                             };

        foreach (Ray currentRay in checkingRays)
        {
            isGrounded |= Physics.Raycast(currentRay, checkDistance, checkingLayers);
        }

        if (isGrounded)
        {
            NormalizeTime();
        }
    }

    /// <summary>
    /// Включает замедление времени.
    /// </summary>
    private void SlowDownTime()
    {
        mainPPVolume.profile = slowtimePPProfile;
        Time.timeScale = slowedTime;
    }

    /// <summary>
    /// Нормализирует время.
    /// </summary>
    private void NormalizeTime()
    {
        mainPPVolume.profile = mainPPProfile;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Функция, вызывающияся при вставании игрока.
    /// </summary>
    private void GetUp()
    {
        shooting.canShoot = false;
        isAiming = false;

        // Запуска анимации
        playerAnimator.SetTrigger("GetUp");

        // Активируем коллайдер игрока в полете и деактивируем коллайдер для перемещения
        mainPlayerCollider.SetActive(true);
        jumpingPlayerCollider.SetActive(false);
    }

    /// <summary>
    /// Ивент для анимаций вставания.
    /// </summary>
    public void ChangeIsStandingFromAnimator()
    {
        isTryingToStand = false;
        isStanding = true;
        shooting.canShoot = true;
    }
}
