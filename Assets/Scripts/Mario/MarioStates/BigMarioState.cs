using UnityEngine;
using UnityEngine.InputSystem;

public class BigMarioState : IMarioState
{
    private static readonly int Hit = Animator.StringToHash("GotHit");
    private static readonly int IsBig = Animator.StringToHash("IsBig");

    public void EnterState(MarioStateMachine context)
    {
        // Adjust collider for Big Mario
        context.SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
        Debug.Log("Entered Big Mario State.");
    }
    
    public void GotHit(MarioStateMachine context)
    {
        context.Animator.SetTrigger(Hit);
        context.Animator.SetBool(IsBig, false);
        context.ChangeState(context.SmallMarioState);
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
    }
    
    public void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        if (powerUpType == PowerUpType.SuperMashroom)
        {
            context.ChangeState(context.FireMarioState);
        }
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        // throw new System.NotImplementedException();
    }
}