using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    [Space]
    [SerializeField] private float slopeLimit;
    [SerializeField] private float slideSpeed;
    [Space]
    [SerializeField] private float jumpForce;
    [SerializeField] private bool shouldMoveAccordingToCamera = true;
    [SerializeField] private float rotationSpeed;
    
    [Header("Required Components")]
    [SerializeField] private GroundPhysics physics;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private PlayerInputHandler inputActions;

    private void Awake() => InitializeComponents();
    private void InitializeComponents()
    {
        if (physics == null) physics = GetComponent<GroundPhysics>();
        if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();
        if (inputActions == null) inputActions = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        var moveInput = inputActions.MoveValue;
        var moveSpeed = inputActions.SprintValue ? sprintSpeed : speed;

        if (shouldMoveAccordingToCamera)
        {
            var cameraRotation = Camera.main.transform.forward;
            var playerRotation = playerRigidbody.transform.forward;
            cameraRotation.y = 0;
            playerRotation.y = 0;

            var requiredRotation = Quaternion.LookRotation(cameraRotation);
            playerRigidbody.transform.rotation = Quaternion.RotateTowards(transform.rotation, requiredRotation, rotationSpeed * Time.deltaTime * 100f);
        }

        var moveDirection = Vector3.zero;
        moveDirection += playerRigidbody.transform.right * moveInput.x;
        moveDirection += playerRigidbody.transform.forward * moveInput.y;
        moveDirection = moveDirection.normalized;
        
        var moveVelocity = moveDirection * moveSpeed;
        moveVelocity += playerRigidbody.transform.up * playerRigidbody.velocity.y;

        if (physics.IsGrounded && inputActions.JumpValue)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        if(CanStayOnSlope() == false)
        {
            var slideDirection = Vector3.ProjectOnPlane(-playerRigidbody.transform.up, physics.GetSurfaceNormal()).normalized;
            moveVelocity += slideDirection * slideSpeed;
        }

        playerRigidbody.velocity = moveVelocity;
    }
    
    // ToDo: doesn;t work correctly yet. Should slide downstairs and don't climb if slope limit is less than angle.
    private bool CanStayOnSlope()
    {
        if(physics.IsGrounded == false) return true;
    
        return (Vector3.Angle(physics.GetSurfaceNormal(), playerRigidbody.transform.up) > slopeLimit);
    }
}
