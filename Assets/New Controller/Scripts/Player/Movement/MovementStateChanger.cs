using UnityEngine;
using UnityEngine.InputSystem;

public class MovementStateChanger : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame) ChangeOnFlight();
        else if (Keyboard.current.gKey.wasPressedThisFrame) ChangeOnGround();
    }

    private void ChangeOnFlight()
    {
        Debug.Log("Flight");
    }

    private void ChangeOnGround()
    {
        Debug.Log("Ground");
    }
}