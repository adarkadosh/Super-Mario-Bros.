using System.Collections;
using Enemies;
using UnityEngine;

namespace Mario.MarioStates
{
    public class StarMarioState : MarioBaseState
    {
        private Coroutine _fadeCoroutine;

        public override void EnterState(MarioStateMachine context)
        {
            MarioEvents.OnMarioStateChange?.Invoke(MarioState.Star);
            context.StartCoroutine(FlashingCoroutine(context));
            Debug.Log("Entered Star Mario State - Flashing Started");
        }

        private IEnumerator FlashingCoroutine(MarioStateMachine context)
        {
            context.PaletteSwapper.StartFlashing();
            yield return new WaitForSeconds(context.StarDuration);
            context.PaletteSwapper.StopFlashing();
            yield return context.StartCoroutine(SwapStarWithDelay(context, context.StarDurationDelay));
        }

        public override void ExitState(MarioStateMachine context)
        {
            Debug.Log("Exited Star Mario State");
        }

        private IEnumerator SwapStarWithDelay(MarioStateMachine context, float delay)
        {
            _fadeCoroutine = context.StartCoroutine(context.PaletteSwapper.SwapStarWithDelay(delay));
            yield return new WaitForSeconds(delay);
            // if (_fadeCoroutine != null) 
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
}