using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBlocker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool shouldBlockCursor;
    [SerializeField] private bool shouldHideCursor;

    private void Awake()
    {
        ChangeCursorBlockedState(shouldBlockCursor);
        ChangeCursorVisibility(shouldHideCursor);
    }

    public void ChangeCursorBlockedState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
    
    public void ChangeCursorVisibility(bool newState)
    {
        Cursor.visible = newState;
    }
}
