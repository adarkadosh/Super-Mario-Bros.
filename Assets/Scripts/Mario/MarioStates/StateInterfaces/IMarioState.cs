using UnityEngine;

public interface IMarioState
{
    void EnterState(MarioStateMachine context);
    void ExitState(MarioStateMachine context);
    void GotHit(MarioStateMachine context);
    void Update(MarioStateMachine context);
    void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType);
    void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision);
}