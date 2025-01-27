// using UnityEngine;
//
// public class MarioJumpingState : IMovementState
// {
//     public void Enter(MarioMovementController mario)
//     {
//         // Initialize jump
//         mario._isJumping = true;
//         mario._velocity.y = mario.JumpForce;
//         
//         // Update Animator
//         if (mario._animator != null)
//             mario._animator.SetBool(MarioMovementController.IsJumping, true);
//         
//         Debug.Log("Entered JumpingState");
//     }
//
//     public void Execute(MarioMovementController mario)
//     {
//         // Handle movement and physics
//         mario.HandleMovementInput();
//         mario.ApplyGravity();
//         
//         // Update ground detection
//         // mario.HandleGroundDetection();
//         
//         // Update _onJump based on vertical velocity
//         // mario._onJump = mario._velocity.y > 0;
//         
//         // Update animations
//         mario.UpdateAnimations();
//         
//         // Transition back to WalkingState if grounded
//         // if (mario.Grounded)
//         // {
//         //     mario.ChangeState(new MarioWalkingState());
//         // }
//     }
//
//     public void Exit(MarioMovementController mario)
//     {
//         mario._velocity.x = 3f;
//         mario._isJumping = false;
//         // Debug.Log("Exited JumpingState");
//     }
// }