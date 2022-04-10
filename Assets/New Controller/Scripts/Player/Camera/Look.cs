using UnityEngine;

public class Look : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -30.0f;
    [Space]
    [SerializeField] private bool isRotationAllowed = true;
    
    [Header("Required Components")]
    [SerializeField] private IPlayerInputActions inputActions;
    
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private void Awake()
    {
        if (inputActions == null) inputActions = GetComponent<IPlayerInputActions>();
    }

    private void Update()
    {
        if (isRotationAllowed == false) return;

        var lookInput = inputActions.LookValue;
        _cinemachineTargetYaw += lookInput.x * Time.deltaTime * sensitivity;
        _cinemachineTargetPitch += lookInput.y * Time.deltaTime * sensitivity;

        // Clamp rotation
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        // Set new values
        cameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }
    
    private static float ClampAngle(float angle, float minClamp = -360f, float maxClamp = 360f)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        
        return Mathf.Clamp(angle, minClamp, maxClamp);
    }
}
