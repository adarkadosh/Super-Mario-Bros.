using System.Collections;
using UnityEngine;

public class StarMarioState : IMarioState
{
    private static readonly int OnStarMode = Animator.StringToHash("OnStarMode");
    private IMarioState _previousState;
    private Coroutine _coroutine;

    public void EnterState(MarioStateMachine context)
    {
        context.gameObject.layer = LayerMask.NameToLayer("StarMario");
        // context.Animator.SetBool(OnStarMode, true);
        _coroutine = context.StartCoroutine(WaitAndExit(context));
    }

    private IEnumerator WaitAndExit(MarioStateMachine context)
    {
        yield return new WaitForSeconds(context.starDuration);
        context.gameObject.layer = LayerMask.NameToLayer("Mario");
        context.ChangeState(_previousState);
    }

    public void GotHit(MarioStateMachine context)
    {
    }

    public void DoAction(MarioStateMachine marioContext)
    {
    }

    public void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
    {
    }

    public void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        
    }
}