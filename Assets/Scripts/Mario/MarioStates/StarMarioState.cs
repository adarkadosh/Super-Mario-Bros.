using System.Collections;
using UnityEngine;

public class StarMarioState : IMarioState
{
    private IMarioState _previousState;
    private Coroutine _coroutine;

    public void EnterState(MarioStateMachine context)
    {
        _coroutine = context.StartCoroutine(WaitAndExit(context));
        context.gameObject.layer = LayerMask.NameToLayer("LethalEnemies");
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