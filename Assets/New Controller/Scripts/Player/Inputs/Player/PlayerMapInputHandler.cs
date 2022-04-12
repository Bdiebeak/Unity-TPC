using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerActions
{
    Move,
    Look,
    Jump,
    Sprint
}

[RequireComponent(typeof(PlayerInput))]
public class PlayerMapInputHandler : MonoBehaviour, IPlayerInputActions 
{
    public Vector2 MoveValue {get; set;}
    public Vector2 LookValue {get; set;}
    public bool JumpValue {get; set;}
    public bool SprintValue {get; set;}
    
    private PlayerInput _playerInput;
    private Dictionary<PlayerActions, InputAction> _inputActions = new Dictionary<PlayerActions, InputAction>();

    private void Update()
    {
        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
            _inputActions = new Dictionary<PlayerActions, InputAction>()
            {
                {PlayerActions.Move, _playerInput.actions["Move"]},
                {PlayerActions.Look, _playerInput.actions["Look"]},
                {PlayerActions.Jump, _playerInput.actions["Jump"]},
                {PlayerActions.Sprint, _playerInput.actions["Sprint"]},
            };    
        }

        MoveValue = _inputActions[PlayerActions.Move].ReadValue<Vector2>();
        LookValue = _inputActions[PlayerActions.Look].ReadValue<Vector2>();
        JumpValue = _inputActions[PlayerActions.Jump].triggered;
        SprintValue = _inputActions[PlayerActions.Sprint].inProgress;
    }
}