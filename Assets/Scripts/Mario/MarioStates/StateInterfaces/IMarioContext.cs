using System;
using UnityEngine;

public interface IMarioContext
{
    MovementController MovementController { get; }

    void ChangeState(IMarioState newState);
    void SetColliderSize(Vector2 size, Vector2 offset);
    
    // Event Hooks (Optional)
    public static Action OnEnterSmallMario;
    public static Action OnEnterBigMario;
    public static Action OnEnterFireMario;
}