using PowerUps;
using UnityEngine;

namespace Mario.MarioStates
{
    public class BigMarioState : MarioBaseState
    {
        private static readonly int GetSmallerHash = Animator.StringToHash("GetSmaller");
        private static readonly int IsBigHash = Animator.StringToHash("IsBig");

        public override void EnterState(MarioStateMachine context)
        {
            MarioEvents.OnMarioStateChange?.Invoke(MarioState.Big);
            context.SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
            context.Animator.SetBool(IsBigHash, true);

            Debug.Log("Entered Big Mario State");
        }

        public override void GotHit(MarioStateMachine context)
        {
            // Become small
            SoundFXManager.Instance.PlaySpatialSound(context.PowerDownClip, context.transform);
            context.FlashTransparency?.StartFlashing();
            context.Animator.SetTrigger(GetSmallerHash);
            context.Invoke(nameof(context.StopFlashing), context.UntouchableDurationValue);
            context.StartCoroutine(context.UntouchableDurationCoroutine(context.UntouchableDurationValue));
            GameEvents.FreezeAllCharacters?.Invoke(1.1f);

            context.ChangeState(MarioState.Small);
        }

        public override void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
        {
            if (powerUpType is PowerUpType.FireFlower or PowerUpType.SuperMashroom)
            {
                context.PaletteSwapper.StartFlashing();
                context.Invoke(nameof(context.PaletteSwapper.StopFlashing), 1.2f);
                GameEvents.FreezeAllCharacters?.Invoke(1.2f);
            
                context.ChangeState(MarioState.Fire);
            } else if (powerUpType == PowerUpType.IceFlower)
            {
                context.PaletteSwapper.StartFlashing();
                context.Invoke(nameof(context.PaletteSwapper.StopFlashing), 1.2f);
                GameEvents.FreezeAllCharacters?.Invoke(1.2f);
            
                context.ChangeState(MarioState.Ice);
            }
        }
    
        // public override void OnCollisionEnter2D(MarioStateMachine context, Collision2D collision)
        // {
        //     if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        //     {
        //         Vector2 direction = collision.transform.position - context.transform.position;
        //         if (Vector2.Dot(direction.normalized, Vector2.down) < 0.25f)
        //         {
        //             MarioEvents.OnMarioGotHit?.Invoke();
        //         }
        //     }
        // }
    }
}