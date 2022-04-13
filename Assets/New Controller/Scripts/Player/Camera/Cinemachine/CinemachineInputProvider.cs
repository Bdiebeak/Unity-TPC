using Cinemachine;
using UnityEngine;

namespace Bdiebeak.TPC.Camera.Cinemachine
{
    public class CinemachineInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
    {
        public Vector3 inputVector = Vector3.zero;

        public float GetAxisValue(int axis)
        {
            return axis switch
            {
                0 => inputVector.x,
                1 => inputVector.y,
                2 => inputVector.z,
                _ => 0f
            };
        }
    }
}