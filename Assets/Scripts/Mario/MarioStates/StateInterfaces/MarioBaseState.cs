using UnityEngine;

public abstract class MarioBaseState : IMarioState
{
    public virtual void EnterState(MarioStateMachine context) { }
    public virtual void ExitState(MarioStateMachine context) { }
    public virtual void GotHit(MarioStateMachine context) { }
    public virtual void Update(MarioStateMachine context) { }
    public virtual void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType) { }
    public virtual void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision) { }
}