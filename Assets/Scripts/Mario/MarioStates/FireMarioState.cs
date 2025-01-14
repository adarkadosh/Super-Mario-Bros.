using UnityEngine;
using UnityEngine.InputSystem;

public class FireMarioState : IMarioState
{
    public void EnterState(MarioStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public void ExitState(MarioStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState(MarioStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public void GotHit(MarioStateMachine context)
    {
        context.Animator.SetTrigger("GotHit");
        context.ChangeState(context.SmallMarioState);
    }

    public void FixedUpdate(MarioStateMachine context)
    {
        throw new System.NotImplementedException();
    }
    
    public void DoAction(MarioStateMachine marioContext)
    {
        if (marioContext.PlayerInputActions.Player.Crouch.IsPressed())
        {
            marioContext.SetColliderSize(new Vector2(0.5f, 1f), new Vector2(0f, 0.5f));
            marioContext.Animator.SetBool("OnCrouch", true);
        }
        else
        {
            marioContext.SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
            marioContext.Animator.SetBool("OnCrouch", false);
        }
        
        if (marioContext.PlayerInputActions.Player.Attack.WasPerformedThisFrame())
        {
            marioContext.ShootFireball();
        }
    }

    public void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        throw new System.NotImplementedException();
    }
}