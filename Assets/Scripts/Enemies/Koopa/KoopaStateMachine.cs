using System;
using UnityEngine;

public class KoopaStateMachine : EnemyBehavior, IPoolable
{
    [SerializeField] private float shellDuration = 3f;
    [SerializeField] private float backToLifeTime = 2f;
    [SerializeField] private float shellSpeed = 12f;
    
    public float ShellDuration => shellDuration;
    public float BackToLifeTime => backToLifeTime;
    public float ShellSpeed => shellSpeed;
    
    
    private IKoopaState _currentState;
    internal readonly WalkingState WalkingState = new WalkingState();
    internal readonly ShellState ShellState = new ShellState();

    void Start()
    {
        _currentState = WalkingState;
        _currentState.EnterState(this);
    }

    void Update()
    {
        _currentState.UpdateState(this);
    }

    internal void ChangeState(IKoopaState newState)
    {
        _currentState.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        _currentState.OnTriggerEnter2D(this, collision);
    }

    protected override void GotHit()
    {
        _currentState.GotHit(this);
    }

    public void Reset()
    {
        // _currentState.ExitState(this);
        _currentState = WalkingState;
        _currentState.EnterState(this);
    }

    public void Trigger()
    {
        throw new NotImplementedException();
    }
}