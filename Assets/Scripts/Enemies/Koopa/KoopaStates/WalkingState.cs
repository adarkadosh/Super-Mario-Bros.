using Enemies.Koopa.KoopaStates.StatesInterfaces;
using UnityEngine;

namespace Enemies.Koopa.KoopaStates
{
    public class WalkingState : IKoopaState
    {
        private static readonly int EnterShell = Animator.StringToHash("EnterShell");
        private static readonly int BackToLife = Animator.StringToHash("BackToLife");
        public void EnterState(KoopaStateMachine koopaState)
        {
            var animator = koopaState.GetComponent<Animator>();
            var movement = koopaState.GetComponent<EntityMovement>();
            var rb = koopaState.GetComponent<Rigidbody2D>();

            animator.SetBool(EnterShell, false);
            animator.SetBool(BackToLife, false);
            movement.enabled = true;
            movement.MovementDirection = Vector2.left;
            movement.MovementSpeed = EntityMovement.InitialMovementSpeed;
            rb.linearVelocity = Vector2.zero;
            koopaState.gameObject.tag = "Koopa";
        }

        public void ExitState(KoopaStateMachine koopaState)
        {
            var movement = koopaState.GetComponent<EntityMovement>();
            var rb = koopaState.GetComponent<Rigidbody2D>();

            movement.enabled = false;
            rb.linearVelocity = Vector2.zero;
        }

        public void UpdateState(KoopaStateMachine koopaState)
        {
            // No implementation needed
        }

        public void OnTriggerEnter2D(KoopaStateMachine koopaState, Collider2D collider2D)
        {
            // No implementation needed
        }

        public void GotHit(KoopaStateMachine koopaState)
        {
            koopaState.ChangeState(koopaState.ShellState);
        }
    }
}