using System;
using Cinemachine;
using UnityEngine;

namespace Bdiebeak.TPC.Camera.Cinemachine
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class CinemachineCameraZoom : CameraZoom
    {
        [Space]
        [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
        
        private CinemachineFramingTransposer _framingTransposer;
        private PlayerInputHandler _inputHandler;

        private float _targetDistance;
        
        private void Awake() => InitializeComponents();
        private void InitializeComponents()
        {
            _framingTransposer = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _inputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Start()
        {
            _targetDistance = startDistance;
            SetDistance(startDistance);
        }

        private void Update() => HandleZoom();
        private void HandleZoom()
        {
            var zoomValue = _inputHandler.ZoomValue * sensitivity;
            _targetDistance = Mathf.Clamp(_targetDistance + zoomValue, minDistance, maxDistance);

            var currentDistance = _framingTransposer.m_CameraDistance;
            if (Math.Abs(currentDistance - _targetDistance) < 0.01f)
            {
                SetDistance(_targetDistance);
                return;
            }

            var lerpedZoom = Mathf.Lerp(currentDistance, _targetDistance, smoothing * Time.deltaTime);
            SetDistance(lerpedZoom);
        }

        // ToDo: can be public functions SetDistanceImmediately() and SetDistanceSmoothly().
        private void SetDistance(float newDistance) => _framingTransposer.m_CameraDistance = newDistance;
    }
}