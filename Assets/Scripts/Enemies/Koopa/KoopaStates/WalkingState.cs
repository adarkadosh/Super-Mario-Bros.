using UnityEngine;

public class WalkingState : IKoopaState
{
    private static readonly int EnterShell = Animator.StringToHash("EnterShell");
    private static readonly int BackToLife = Animator.StringToHash("BackToLife");

    public void EnterState(KoopaStateMachine koopaState)
    {
        koopaState.GetComponent<Animator>().SetBool(EnterShell, false);
        koopaState.GetComponent<Animator>().SetBool(BackToLife, false);
        koopaState.GetComponent<EntityMovement>().enabled = true;
        koopaState.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        
    }

    public void ExitState(KoopaStateMachine koopaState)
    {
        koopaState.GetComponent<EntityMovement>().enabled = false;
        koopaState.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public void UpdateState(KoopaStateMachine koopaState)
    {
    }

    public void OnTriggerEnter2D(KoopaStateMachine koopaState, Collider2D collider2D)
    {
    }

    public void GotHit(KoopaStateMachine koopaState)
    {
        koopaState.ChangeState(koopaState.shellState);
    }
}