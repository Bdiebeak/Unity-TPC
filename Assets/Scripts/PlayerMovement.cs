using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float rotationToCameraSpeed;

    [SerializeField]
    private float jumpForce;

    private Animator playerAnimator;
    private CharacterController controller;
    private Camera mainCamera;
    private Vector3 moveDirection;
    private bool isGrounded = true;

    /// <summary>
    /// Инициализируем переменные
    /// </summary>
    void Start()
    {
        mainCamera = Camera.main;
        controller = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        // Если игрок не стоит на земле - нажал на прыжок.
        // Уменьшаем его высоту (падает) с помощью физики.
        // Блокируем все остальное движение и вращение до того момента,
        // пока игрок не приземлится.
        if (!isGrounded)
        {
            moveDirection.y += Physics.gravity.y * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime * moveSpeed);
            return;
        }

        // Получаем ввод с WASD
        var inputX = Input.GetAxisRaw("Horizontal"); // отвечает за движение влево и вправо
        var inputZ = Input.GetAxisRaw("Vertical"); // отвечает за движение вперед и назад

        // Получаем положение камеры
        var forward = mainCamera.transform.forward;
        var right = mainCamera.transform.right;

        // Блокируем наклон камеры по координате У, чтобы игрок не наклонялся вперед и назад
        forward.y = 0;
        right.y = 0;

        // Вычисляем направление движения и вращения
        moveDirection = forward * inputZ + right * inputX;
        moveDirection = moveDirection.normalized;

        // Если новый вектор не равен нулю:
        // осуществляем движение и вращение, включаем анимацию ходьбы
        if (moveDirection != Vector3.zero)
        {
            playerAnimator.SetBool("walking", true);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), rotationToCameraSpeed * Time.deltaTime);
            controller.Move(moveDirection * Time.deltaTime * moveSpeed);
        }
        // Иначе: выключаем анимацию ходьбы
        else
        {
            playerAnimator.SetBool("walking", false);
        }

        // Если был нажат пробел : осуществляем прыжок
        if (Input.GetKeyDown("space"))
        {
            playerAnimator.SetTrigger("jump");
            isGrounded = false;

            moveDirection.y = jumpForce;
            controller.Move(moveDirection * Time.deltaTime * moveSpeed);
        }
    }

    /// <summary>
    /// Функция, используемая ивентом на анимации прыжка
    /// </summary>
    public void Grounded()
    {
        isGrounded = true;
    }
}
