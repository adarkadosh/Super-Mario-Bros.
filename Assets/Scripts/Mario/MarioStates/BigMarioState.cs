using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BigMarioState : IMarioState
{
    private static readonly int GetSmaller = Animator.StringToHash("GetSmaller");
    private static readonly int IsBig = Animator.StringToHash("IsBig");

    public void EnterState(MarioStateMachine context)
    {
        // Adjust collider for Big Mario
        context.SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
        Debug.Log("Entered Big Mario State.");
    }
    
    public void GotHit(MarioStateMachine context)
    {
        context.flashTransparency?.StartFlashing();
        // context.StartCoroutine(GotHitCoroutine(context));
        context.Animator.SetTrigger(GetSmaller);
        context.Invoke(nameof(MarioStateMachine.StopFlashing), 3f);
        context.Animator.SetBool(IsBig, false);
        context.ChangeState(context.SmallMarioState);
        context.StartCoroutine(context.UntouchableDuration(context.untouchableDuration));
    }
    
    private IEnumerator GotHitCoroutine(MarioStateMachine context)
    {
        context.Animator.SetTrigger(GetSmaller);
        context.Invoke(nameof(MarioStateMachine.StopFlashing), 3f);
        yield return new WaitForSeconds(2.1f);
        context.Animator.SetBool(IsBig, false);
        context.ChangeState(context.SmallMarioState);
        context.StartCoroutine(context.UntouchableDuration(context.untouchableDuration));
    }

    public void DoAction(MarioStateMachine marioContext)
    {
        
    }
    
    public void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        if (powerUpType == PowerUpType.FireFlower)
        {
            // context.Animator.SetTrigger(GetOnFire);
            // GameEvents.FreezeAllCharacters?.Invoke(1.2f);
            // yield return new WaitForSeconds(0);
            // context.Animator.SetBool(OnFireMode, true);
            // context.ChangeState(context.FireMarioState);
            
            // context.StartCoroutine(DoPickUpPowerUp(context, powerUpType));
            context.paletteSwapper.StartFlashing();
            context.Invoke(nameof(PaletteSwapper.StopFlashing), 1.2f);
            GameEvents.FreezeAllCharacters?.Invoke(1.2f);
            context.ChangeState(context.FireMarioState);
        }
    }
    
    private IEnumerator DoPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        context.paletteSwapper.StartFlashing();
        context.Invoke(nameof(context.paletteSwapper.StopFlashing), 1.2f);
        GameEvents.FreezeAllCharacters?.Invoke(1.2f);
        yield return new WaitForSeconds(0);
        context.ChangeState(context.FireMarioState);
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        // throw new System.NotImplementedException();
    }
}