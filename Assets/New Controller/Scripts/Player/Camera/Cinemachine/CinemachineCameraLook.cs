using Cinemachine;
using UnityEngine;

namespace Bdiebeak.TPC.Camera.Cinemachine
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class CinemachineCameraLook : CameraLook
    {
        [Space]
        [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
        
        private CinemachinePOV _pov;
        private CinemachineInputProvider _cinemachineInputProvider;
        private PlayerInputHandler _inputHandler;

        private void Awake()
        {
            InitializeComponents();
            InitializeCinemachine();
        }

        private void InitializeComponents()
        {
            _pov = cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
            _cinemachineInputProvider = cinemachineCamera.gameObject.GetComponent<CinemachineInputProvider>();
            _inputHandler = GetComponent<PlayerInputHandler>();
        }

        private void InitializeCinemachine()
        {
            // ToDo: can be public functions
            cinemachineCamera.Follow = cameraTarget;
            cinemachineCamera.LookAt = cameraTarget;

            _pov.m_HorizontalAxis.m_MinValue = leftClamp;
            _pov.m_HorizontalAxis.m_MaxValue = rightClamp;
            _pov.m_HorizontalAxis.m_MaxSpeed = sensitivityX;
            
            _pov.m_VerticalAxis.m_MinValue = bottomClamp;
            _pov.m_VerticalAxis.m_MaxValue = topClamp;
            _pov.m_VerticalAxis.m_MaxSpeed = sensitivityY;
        }

        private void Update()
        {
            _cinemachineInputProvider.inputVector.x = _inputHandler.LookValue.x;
            _cinemachineInputProvider.inputVector.y = _inputHandler.LookValue.y;
        }
    }
}