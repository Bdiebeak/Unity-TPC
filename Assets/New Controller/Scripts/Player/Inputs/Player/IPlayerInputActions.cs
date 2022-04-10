using UnityEngine;

public interface IPlayerInputActions
{
    public Vector2 MoveValue {get; set;}
    public Vector2 LookValue {get; set;}
    public bool JumpValue {get; set;}
    public bool SprintValue {get; set;}
}
