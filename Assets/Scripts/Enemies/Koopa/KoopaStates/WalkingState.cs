// using UnityEngine;
//
// public class WalkingState : IKoopaState
// {
//     private static readonly int EnterShell = Animator.StringToHash("EnterShell");
//     private static readonly int BackToLife = Animator.StringToHash("BackToLife");
//
//     public void EnterState(KoopaStateMachine koopaState)
//     {
//         koopaState.GetComponent<Animator>().SetBool(EnterShell, false);
//         koopaState.GetComponent<Animator>().SetBool(BackToLife, false);
//         koopaState.GetComponent<EntityMovement>().enabled = true;
//         koopaState.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
//         
//     }
//
//     public void ExitState(KoopaStateMachine koopaState)
//     {
//         koopaState.GetComponent<EntityMovement>().enabled = false;
//         koopaState.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
//     }
//
//     public void UpdateState(KoopaStateMachine koopaState)
//     {
//         // Draw a the box cast for debugging purposes
//         Debug.DrawLine(koopaState.GetComponent<Rigidbody2D>().position, koopaState.GetComponent<Rigidbody2D>().position + Vector2.down * 0.1f, Color.red);
//         // make sure that koopa is always on the ground
//         if (!IsGrounded(koopaState.GetComponent<Rigidbody2D>(), 1f))
//         {
//             koopaState.GetComponent<EntityMovement>().MovementDirection *= -1;
//         }
//     }
//
//     public void OnTriggerEnter2D(KoopaStateMachine koopaState, Collider2D collider2D)
//     {
//     }
//
//     public void GotHit(KoopaStateMachine koopaState)
//     {
//         koopaState.ChangeState(koopaState.shellState);
//     }
//     
//     private static bool IsGrounded(Rigidbody2D rigidbody2D, float groundDetectionDistance)
//     {
//         var origin = rigidbody2D.position + Vector2.down * 0.1f; // Slightly below the entity
//     
//         // Perform a raycast to detect ground directly below
//         var hit = Physics2D.BoxCast(origin, new Vector2(0.9f, 0.1f), 0, Vector2.down, groundDetectionDistance);
//     
//         // Return true if ground is detected
//         return hit;
//     }
// }

using UnityEngine;

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

        // Freeze Y position to prevent falling 
        // rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void ExitState(KoopaStateMachine koopaState)
    {
        var movement = koopaState.GetComponent<EntityMovement>();
        var rb = koopaState.GetComponent<Rigidbody2D>();

        movement.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // Unfreeze Y position if other states require movement
        // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void UpdateState(KoopaStateMachine koopaState)
    {
        ///// if we want to use the box cast to detect ground -> if you want to put Koopa on the block ///////
        // Rigidbody2D rb = koopaState.GetComponent<Rigidbody2D>();
        // EntityMovement movement = koopaState.GetComponent<EntityMovement>();
        //
        // // Draw the box cast for debugging purposes
        // Debug.DrawLine(rb.position, rb.position + Vector2.down * 0.1f, Color.red);
        //
        // // Ensure Koopa is always on the ground
        // if (!IsGrounded(rb, 0.2f))
        // {
        //     // Reverse direction if no ground detected
        //     movement.MovementDirection *= -1;
        // }
    }

    public void OnTriggerEnter2D(KoopaStateMachine koopaState, Collider2D collider2D)
    {
        // No implementation needed
    }

    public void GotHit(KoopaStateMachine koopaState)
    {
        koopaState.ChangeState(koopaState.ShellState);
    }

    ///// Detect ground using a BoxCast /////
    // private bool IsGrounded(Rigidbody2D rigidbody2D, float groundDetectionDistance)
    // {
    //     Vector2 origin = rigidbody2D.position + Vector2.down * 0.1f; // Slightly below the entity
    //     Vector2 size = new Vector2(0.9f, 0.1f); // Width and height of the box
    //
    //     // Perform a BoxCast to detect ground directly below
    //     RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, groundDetectionDistance);
    //
    //     // Visualize the BoxCast in the editor
    //     Color castColor = hit ? Color.green : Color.red;
    //     Debug.DrawLine(origin, origin + Vector2.down * (groundDetectionDistance + 0.1f), castColor);
    //
    //     return hit.collider != null;
    // }
}