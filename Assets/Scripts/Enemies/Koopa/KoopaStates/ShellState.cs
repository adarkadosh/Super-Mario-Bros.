using System.Collections;
using Enemies.Koopa.KoopaStates.StatesInterfaces;
using Managers;
using UnityEngine;

namespace Enemies.Koopa.KoopaStates
{
    public class ShellState : IKoopaState
    {
        private static readonly int EnterShell = Animator.StringToHash("EnterShell");
        private static readonly int BackToLife = Animator.StringToHash("BackToLife");
        private const string LethalShell = "LethalShell";
        private bool _isPushed;
        private Coroutine _shellCoroutine;

        private IEnumerator HandleShellDuration(KoopaStateMachine koopa)
        {
            yield return new WaitForSeconds(koopa.ShellDuration);
            koopa.GetComponent<Animator>().SetBool(BackToLife, true);
            yield return new WaitForSeconds(koopa.BackToLifeTime);
            koopa.ChangeState(koopa.WalkingState);
        }


        public void EnterState(KoopaStateMachine koopaState)
        {
            koopaState.GetComponent<Animator>().SetBool(EnterShell, true);
            koopaState.GetComponent<EntityMovement>().enabled = false;
            koopaState.GetComponent<CircleCollider2D>().enabled = false;
            koopaState.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            koopaState.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            _isPushed = false;
            koopaState.gameObject.tag = "ShellKoopa";
            _shellCoroutine = koopaState.StartCoroutine(HandleShellDuration(koopaState));
        }

        public void ExitState(KoopaStateMachine koopaState)
        {
            if (_shellCoroutine != null)
            {
                koopaState.StopCoroutine(_shellCoroutine);
                _shellCoroutine = null;
            }
            var movement = koopaState.GetComponent<EntityMovement>();
            movement.MovementDirection = Vector2.zero;
            movement.MovementSpeed = 0;
            // koopaState.gameObject.layer = LayerMask.NameToLayer("Enemy");
            var rb = koopaState.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            koopaState.GetComponent<CircleCollider2D>().enabled = true;
            // var collider = koopaState.GetComponent<Collider2D>();
            // collider.enabled = true;
            _isPushed = false;
        }

        public void UpdateState(KoopaStateMachine koopaState)
        {
            if (_isPushed)
            {
                // Stop the coroutine managing the shell duration
                if (_shellCoroutine != null)
                {
                    koopaState.StopCoroutine(_shellCoroutine);
                    _shellCoroutine = null;
                }
            }
        }
    
        //TODO: Make Mario hit the shell when pushed and trigger Death Animation
        public void OnTriggerEnter2D(KoopaStateMachine koopaState, Collider2D collider2D)
        {
            if (collider2D.CompareTag("Player"))
            {
                if (!_isPushed)
                {
                    GameEvents.OnEventTriggered?.Invoke(koopaState.ShellKoopaScore, koopaState.transform.position);
                    GotPush(koopaState, collider2D);
                    koopaState.GetComponent<CircleCollider2D>().enabled = true;
                }
            }
        }

        private void GotPush(KoopaStateMachine koopaState, Collider2D collider2D)
        {
            // Set BackToLife false to prevent unintended transitions
            koopaState.GetComponent<Animator>().SetBool(BackToLife, false);
            var direction = new Vector2(
                koopaState.transform.position.x - collider2D.transform.position.x, 0);
            koopaState.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            var movement = koopaState.GetComponent<EntityMovement>();
            movement.MovementDirection = direction.normalized;
            movement.MovementSpeed = koopaState.ShellSpeed;
            movement.enabled = true;
            koopaState.gameObject.tag = LethalShell;
            _isPushed = true;
        }

        public void GotHit(KoopaStateMachine koopaState)
        {
            if (_isPushed)
                koopaState.StartCoroutine(koopaState.DeathSequence());
        }
    }
}