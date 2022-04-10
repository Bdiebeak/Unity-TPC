using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    
    [Header("Required Components")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private IPlayerInputActions inputActions;

    private void Awake() => InitializeComponents();
    private void InitializeComponents()
    {
        if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();
        if (inputActions == null) inputActions = GetComponent<IPlayerInputActions>();
    }

    private void Update()
    {
        var moveInput = inputActions.MoveValue;
        var moveSpeed = inputActions.SprintValue ? sprintSpeed : speed;

        var moveDirection = Vector3.zero;
        moveDirection += playerRigidbody.transform.right * moveInput.x;
        moveDirection += playerRigidbody.transform.forward * moveInput.y;
        moveDirection = moveDirection.normalized;
        
        var moveVelocity = moveDirection * moveSpeed;

        playerRigidbody.velocity = moveVelocity;
    }
}
