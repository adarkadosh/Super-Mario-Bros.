using Managers;
using PowerUps;
using UnityEngine;

namespace Mario.MarioStates
{
    public class FireMarioState : MarioBaseState
    {
        // private static readonly int IsFireHash = Animator.StringToHash("IsFire");
        private static readonly int HitHash = Animator.StringToHash("GetSmaller");

        public override void EnterState(MarioStateMachine context)
        {
            MarioEvents.OnMarioStateChange?.Invoke(MarioState.Fire);
            Debug.Log("Entered Fire Mario State");
        }

        public override void GotHit(MarioStateMachine context)
        {
            // Revert to Big Mario
            SoundFXManager.Instance.PlaySound(context.PowerDownClip, context.transform);
            context.FlashTransparency?.StartFlashing();
            context.Animator.SetTrigger(HitHash);
            context.Invoke(nameof(context.StopFlashing), context.UntouchableDurationValue);
            context.StartCoroutine(context.UntouchableDurationCoroutine(context.UntouchableDurationValue));
            GameEvents.FreezeAllCharacters?.Invoke(1.1f);
            context.ChangeState(MarioState.Small);
        }

        public override void Update(MarioStateMachine context)
        {
            // Example: shooting fireball logic could go here, or be triggered by an input event
            // if (context.PlayerInputActions.Player.Attack.triggered)
            // {
            // context.ShootFireball();
            // }
            // context.ShootFireball();
        }
    
        public override void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.IceFlower)
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