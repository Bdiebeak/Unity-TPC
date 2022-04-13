using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerActions
{
    Move,
    Look,
    Zoom,
    Jump,
    Sprint
}

[RequireComponent(typeof(PlayerInput))]
public class PlayerMapInputHandler : PlayerInputHandler
{
    private PlayerInput _playerInput;
    private Dictionary<PlayerActions, InputAction> _inputActions = new Dictionary<PlayerActions, InputAction>();

    private void Update()
    {
        // This initialization is handled in Update because Unity Input System's documentation says do it.
        if (_playerInput == null) Reinitialize();
        HandleInput();
    }

    private void Reinitialize()
    {
        _playerInput = GetComponent<PlayerInput>();
        _inputActions = new Dictionary<PlayerActions, InputAction>();

        foreach (var actionValue in (PlayerActions[]) Enum.GetValues(typeof(PlayerActions)))
            _inputActions.Add(actionValue, _playerInput.actions[actionValue.ToString()]);
    }

    private void HandleInput()
    {
        MoveValue = _inputActions[PlayerActions.Move].ReadValue<Vector2>();
        LookValue = _inputActions[PlayerActions.Look].ReadValue<Vector2>();
        ZoomValue = _inputActions[PlayerActions.Zoom].ReadValue<float>();
        JumpValue = _inputActions[PlayerActions.Jump].triggered;
        SprintValue = _inputActions[PlayerActions.Sprint].inProgress;
    }
}