using System.Collections;
using UnityEngine;

public class ShellState : IKoopaState
{
    private static readonly int EnterShell = Animator.StringToHash("EnterShell");
    private static readonly int BackToLife = Animator.StringToHash("BackToLife");
    private bool _isPushed;
    private Coroutine _shellCoroutine;

    private IEnumerator HandleShellDuration(KoopaStateMachine koopa)
    {
        yield return new WaitForSeconds(koopa.ShellDuration);
        koopa.GetComponent<Animator>().SetBool(BackToLife, true);
        yield return new WaitForSeconds(koopa.BackToLifeTime);
        koopa.ChangeState(koopa.walkingState);
    }
    

    public void EnterState(KoopaStateMachine koopaState)
    {
        koopaState.GetComponent<Animator>().SetBool(EnterShell, true);
        // koopaState.Animator.SetBool(EnterShell, true);
        koopaState.GetComponent<EntityMovement>().enabled = false;
        koopaState.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        // koopaState.gameObject.layer = LayerMask.NameToLayer("LethalEnemies");
        _isPushed = false;
        _shellCoroutine = koopaState.StartCoroutine(HandleShellDuration(koopaState));
    }

    public void ExitState(KoopaStateMachine koopaState)
    {
        if (_shellCoroutine != null)
        {
            koopaState.StopCoroutine(_shellCoroutine);
            _shellCoroutine = null;
        }
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

    public void OnTriggerEnter2D(KoopaStateMachine koopaState, Collider2D collider2D)
    {
        if (!_isPushed && collider2D.CompareTag("Player"))
        {
            GotPush(koopaState, collider2D);
        }
        else
        {
            MarioEvents.OnMarioGotHit?.Invoke();
        }
    }

    private void GotPush(KoopaStateMachine koopaState, Collider2D collider2D)
    {
        // Set BackToLife to false to prevent unintended transitions
        koopaState.GetComponent<Animator>().SetBool(BackToLife, false);
        Vector2 direction = new Vector2(
            koopaState.transform.position.x - collider2D.transform.position.x, 0);
        _isPushed = true;
        koopaState.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        EntityMovement movement = koopaState.GetComponent<EntityMovement>();
        movement.MovementDirection = direction.normalized;
        movement.MovementSpeed = koopaState.ShellSpeed;
        movement.enabled = true;
        koopaState.gameObject.layer = LayerMask.NameToLayer($"LethalEnemies");
    }

    public void GotHit(KoopaStateMachine koopaState)
    {
        
    }
}