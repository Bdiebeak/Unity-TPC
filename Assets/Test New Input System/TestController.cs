using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour
{
    public void OnMove(InputValue value)
    {
        Debug.Log($"On Move {value.Get<Vector2>()}");
    }

    public void OnFire(InputValue value)
    {
       // Debug.Log("Fire");
    }

    private void Update()
    {
        if (Keyboard.current.aKey.isPressed) Debug.Log("A");
    }
}
