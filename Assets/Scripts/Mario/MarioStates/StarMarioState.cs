using System.Collections;
using UnityEngine;

public class StarMarioState : MarioBaseState
{
    private static readonly int IsStarHash = Animator.StringToHash("IsStar");
    private Coroutine _fadeCoroutine;
    
    public override void EnterState(MarioStateMachine context)
    {
        MarioEvents.OnMarioStateChange?.Invoke(MarioState.Star);
        context.StartCoroutine(FlashingCoroutine(context));
        // context.PaletteSwapper.StartFlashing();
        
        Debug.Log("Entered Star Mario State - Flashing Started");
    }
    
    private IEnumerator FlashingCoroutine(MarioStateMachine context)
    {
        while (true)
        {
            context.PaletteSwapper.StartFlashing();
            yield return new WaitForSeconds(context.StarDuration);
            context.PaletteSwapper.StopFlashing();
            // yield return new WaitForSeconds(context.StarDurationDelay);
            yield return context.StartCoroutine(SwapStarWithDelay(context, context.StarDurationDelay));

        }
    }

    public override void ExitState(MarioStateMachine context)
    {
        // Stop flashing effect
        // context.StopFlashing();
        //
        // if (_fadeCoroutine != null)
        // {
        //     context.StopCoroutine(_fadeCoroutine);
        //     _fadeCoroutine = null;
        // }
        //
        // // Start the fade effect
        // _fadeCoroutine = context.StartCoroutine(SwapStarWithDelay(context, context.StarDurationDelay));
        

        Debug.Log("Exited Star Mario State");
    }
    
    private IEnumerator SwapStarWithDelay(MarioStateMachine context, float delay)
    {
        context.StartCoroutine(context.PaletteSwapper.SwapStarWithDelay(delay));
        yield return new WaitForSeconds(delay);
        context.StopCoroutine(_fadeCoroutine);
    }

    // If you do a timed star inside this state instead of the state machine:
    // you'd run a coroutine that eventually calls `ChangeState(MarioState.Small)`

    public override void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
    {
        // Example: kill enemies on collision
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Kill enemy
            EnemyBehavior enemy = collision.gameObject.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.StartCoroutine(enemy.DeathSequence());
            }
        }
    }
}