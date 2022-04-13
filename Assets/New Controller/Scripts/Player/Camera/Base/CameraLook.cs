using UnityEngine;

namespace Bdiebeak.TPC.Camera
{
    public abstract class CameraLook : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] protected Transform cameraTarget;
        [SerializeField] protected float sensitivityX = 0.1f;
        [SerializeField] protected float sensitivityY = 0.1f;
        [Space]
        [SerializeField] protected float leftClamp = -180.0f;
        [SerializeField] protected float rightClamp = 180.0f;
        [Space]
        [SerializeField] protected float topClamp = 90.0f;
        [SerializeField] protected float bottomClamp = -90.0f;
    }
}