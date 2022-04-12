using UnityEngine;

public class FlightMovement : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    [Space]
    [SerializeField] private bool shouldMoveAccordingToCamera = true;
    [SerializeField] private float rotationSpeed;
    
    [Header("Required Components")]
    [SerializeField] private FlightPhysics physics;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private IPlayerInputActions inputActions;
    
    private void Awake() => InitializeComponents();
    private void InitializeComponents()
    {
        if (physics == null) physics = GetComponent<FlightPhysics>();
        if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();
        if (inputActions == null) inputActions = GetComponent<IPlayerInputActions>();
    }
    
    private void Update()
    {
        var moveInput = inputActions.MoveValue;
        var moveSpeed = inputActions.SprintValue ? sprintSpeed : speed;

        if (shouldMoveAccordingToCamera)
        {
            var requiredFacing = -Camera.main.transform.up;
            var targetRotation = Quaternion.LookRotation(requiredFacing, Camera.main.transform.forward);
            transform.rotation = targetRotation;
        }

        var moveDirection = Vector3.zero;
        moveDirection += playerRigidbody.transform.right * moveInput.x;
        moveDirection += playerRigidbody.transform.up * moveInput.y;
        moveDirection = moveDirection.normalized;
        
        var moveVelocity = moveDirection * moveSpeed;
        playerRigidbody.velocity = moveVelocity;
    }
}