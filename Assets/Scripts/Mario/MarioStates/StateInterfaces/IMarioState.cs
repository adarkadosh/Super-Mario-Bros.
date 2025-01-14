using UnityEngine;
using UnityEngine.InputSystem;

public interface IMarioState
{
    void EnterState(MarioStateMachine context);
    void GotHit(MarioStateMachine context);
    void DoAction(MarioStateMachine marioContext);

    void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType);
    void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision);
}