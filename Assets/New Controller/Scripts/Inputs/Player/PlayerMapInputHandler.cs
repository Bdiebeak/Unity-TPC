using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMapInputHandler : MonoBehaviour, IPlayerInputActions 
{
    public Vector2 MoveValue {get; set;}
    public Vector2 LookValue {get; set;}
    public bool JumpValue {get; set;}

    private void OnMove(InputValue value)
    {
        MoveValue = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        LookValue = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        JumpValue = value.isPressed;
    }
    
    
}
