using UnityEngine;
using UnityEngine.InputSystem;

public class SmallMarioState : IMarioState
{
    public void EnterState(MarioStateMachine context)
    {
        context.gameObject.layer = LayerMask.NameToLayer("Mario");
        context.SetColliderSize(new Vector2(0.5f, 1f), Vector2.zero);
        Debug.Log("Entered Small Mario State.");
    }

    public void GotHit(MarioStateMachine context)
    {
        context.GetComponent<DeathAnimation>().TriggerDeathAnimation();
        MarioEvents.OnMarioDeath?.Invoke();
    }

    public void DoAction(MarioStateMachine marioContext)
    {
        throw new System.NotImplementedException();
    }
    

    public void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        
    }
}