using UnityEngine;
using UnityEngine.InputSystem;

public class FireMarioState : IMarioState
{
    private static readonly int Hit = Animator.StringToHash("GotHit");
    private static readonly int IsBig = Animator.StringToHash("IsBig");
    private static readonly int OnStarMode = Animator.StringToHash("OnStarMode");

    public void EnterState(MarioStateMachine context)
    {
        Debug.Log("Entered Fire Mario State.");
    }

    public void GotHit(MarioStateMachine context)
    {
        context.Animator.SetTrigger(Hit);
        context.Animator.SetBool(IsBig, false);
        context.ChangeState(context.SmallMarioState);
        context.StartCoroutine(context.UntouchableDuration(context.untouchableDuration));
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
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
    }
}