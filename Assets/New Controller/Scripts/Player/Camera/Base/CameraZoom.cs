using UnityEngine;

namespace Bdiebeak.TPC.Camera
{
    public abstract class CameraZoom : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected float startDistance = 5f;
        [SerializeField] protected float minDistance = 1f;
        [SerializeField] protected float maxDistance = 10f;
        [Space]
        [SerializeField] protected float sensitivity = 0.5f;
        [SerializeField] protected float smoothing = 2f;
    }
}