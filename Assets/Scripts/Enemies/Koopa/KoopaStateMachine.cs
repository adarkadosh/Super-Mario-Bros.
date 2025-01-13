using System;
using UnityEngine;

public class KoopaStateMachine : EnemyBehavior
{
    [SerializeField] private float shellDuration = 3f;
    [SerializeField] private float backToLifeTime = 2f;
    [SerializeField] private float shellSpeed = 12f;
    
    public float ShellDuration => shellDuration;
    public float BackToLifeTime => backToLifeTime;
    public float ShellSpeed => shellSpeed;
    
    
    private IKoopaState currentState;
    internal WalkingState walkingState = new WalkingState();
    internal ShellState shellState = new ShellState();

    void Start()
    {
        currentState = walkingState;
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    internal void ChangeState(IKoopaState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        currentState.OnTriggerEnter2D(this, collision);
    }

    protected override void GotHit()
    {
        currentState.GotHit(this);
    }
}