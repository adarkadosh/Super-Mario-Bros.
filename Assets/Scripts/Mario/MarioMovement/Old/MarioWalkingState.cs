// using UnityEngine;
//
// namespace Mario.MarioMovement.Old
// {
//     public class MarioWalkingState : IMovementState
//     {
//         public void Enter(MarioMovementController mario)
//         {
//             // Called once when transitioning into WalkingState
//             // e.g., you can reset some parameters if needed.
//             mario._animator.SetBool(MarioMovementController.IsJumping, false);
//             mario._velocity.y = Mathf.Max(mario._velocity.y, 0);
//             mario._onJump = mario._velocity.y > 0;
//         }
//
//         public void Execute(MarioMovementController mario)
//         {
//             // This is called every frame while in WalkingState
//             mario.HandleMovementInput();
//             mario.HandleGroundDetection();
//             mario.ApplyGravity();
//             mario.UpdateAnimations();
//         }
//
//         public void Exit(MarioMovementController mario)
//         {
//             // Called once when leaving WalkingState
//         }
//     }
// }