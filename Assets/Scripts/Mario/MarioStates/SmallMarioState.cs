using System.Collections;
using PowerUps;
using UnityEngine;

// TODO: Crouch and Slide
// TODO: Attack
namespace Mario.MarioStates
{
    public class SmallMarioState : MarioBaseState
    {
        private static readonly int DieHash = Animator.StringToHash("Die");
        private static readonly int GetBiggerHash = Animator.StringToHash("GetBigger");
        private static readonly int IsBigHash = Animator.StringToHash("IsBig");

        public override void EnterState(MarioStateMachine context)
        {
            MarioEvents.OnMarioStateChange?.Invoke(MarioState.Small);
            // context.gameObject.layer = LayerMask.NameToLayer("Mario");
            context.SetColliderSize(new Vector2(0.75f, 1f), Vector2.zero);
            context.Animator.SetBool(IsBigHash, false);

            Debug.Log("Entered Small Mario State");
        }

        public override void GotHit(MarioStateMachine context)
        {
            // If small Mario is hit, typically Mario dies
            context.Animator.SetTrigger(DieHash);
            MarioEvents.OnMarioDeath?.Invoke();
        }

        public override void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
        {
            if (powerUpType is PowerUpType.SuperMashroom or PowerUpType.FireFlower or PowerUpType.IceFlower)
            {
                MarioEvents.OnMarioStateChange?.Invoke(MarioState.GrowShrink);
                context.StartCoroutine(DoPickUpSuperMushroom(context));
            }
        }

        private IEnumerator DoPickUpSuperMushroom(MarioStateMachine context)
        {
            context.Animator.SetTrigger(GetBiggerHash);
            GameEvents.FreezeAllCharacters?.Invoke(1.2f);
            yield return new WaitForSeconds(1.2f);

            context.Animator.SetBool(IsBigHash, true);
            context.ChangeState(MarioState.Big);
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