using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SmallMarioState : IMarioState
{
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int GetBigger = Animator.StringToHash("GetBigger");
    private static readonly int IsBig = Animator.StringToHash("IsBig");

    public void EnterState(MarioStateMachine context)
    {
        context.gameObject.layer = LayerMask.NameToLayer("Mario");
        context.SetColliderSize(new Vector2(0.75f, 1f), Vector2.zero);
        Debug.Log("Entered Small Mario State.");
    }

    public void GotHit(MarioStateMachine context)
    {
        context.Animator.SetTrigger(Die);
        // context.StartCoroutine(Extensions.WaitForSeconds(1));
        MarioEvents.OnMarioDeath?.Invoke();
        context.GetComponent<DeathAnimation>().TriggerDeathAnimation(1);
    }

    public void DoAction(MarioStateMachine marioContext)
    {
    }
    

    public void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        if (powerUpType == PowerUpType.SuperMashroom)
        {
            context.StartCoroutine(DoPickUpPowerUp(context, powerUpType));
        }
    }
    
    private IEnumerator DoPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
        context.Animator.SetTrigger(GetBigger);
        GameEvents.FreezeAllCharacters?.Invoke(1.2f);
        yield return new WaitForSeconds(1.2f);
        context.Animator.SetBool(IsBig, true);
        context.ChangeState(context.BigMarioState);
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        
    }
}