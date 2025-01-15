using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BigMarioState : IMarioState
{
    private static readonly int GetSmaller = Animator.StringToHash("GetSmaller");
    private static readonly int IsBig = Animator.StringToHash("IsBig");
    private static readonly int GetOnFire = Animator.StringToHash("GetOnFire");

    public void EnterState(MarioStateMachine context)
    {
        // Adjust collider for Big Mario
        context.SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
        Debug.Log("Entered Big Mario State.");
    }
    
    public void GotHit(MarioStateMachine context)
    {
        context.StartCoroutine(GotHitCoroutine(context));
    }
    
    private IEnumerator GotHitCoroutine(MarioStateMachine context)
    {
        context.Animator.SetTrigger(GetSmaller);
        yield return new WaitForSeconds(2.1f);
        context.Animator.SetBool(IsBig, false);
        context.ChangeState(context.SmallMarioState);
        context.StartCoroutine(context.UntouchableDuration(context.untouchableDuration));
    }

    public void DoAction(MarioStateMachine marioContext)
    {
        if (marioContext.PlayerInputActions.Player.Crouch.IsPressed())
        {
            marioContext.SetColliderSize(new Vector2(0.75f, 1f), Vector2.zero);
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
        if (powerUpType == PowerUpType.FireFlower)
        {
            context.StartCoroutine(DoPickUpPowerUp(context, powerUpType));
        }
    }
    
    private IEnumerator DoPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        context.Animator.SetTrigger(GetOnFire);
        GameEvents.FreezeAllCharacters?.Invoke(1.2f);
        yield return new WaitForSeconds(1.2f);
        context.ChangeState(context.FireMarioState);
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        // throw new System.NotImplementedException();
    }
}