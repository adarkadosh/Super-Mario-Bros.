// using UnityEngine;
//
// /// <summary>
// /// The state representing Mario in the air (either jumping or falling).
// /// </summary>
// public class MarioJumpingState : IMovementState
// {
//     private readonly MarioMoveController _controller;
//     private readonly MarioMovementStateMachine _stateMachine;
//     private readonly MovementData _data;
//
//     public MarioJumpingState(MarioMoveController controller, 
//                         MarioMovementStateMachine stateMachine, 
//                         MovementData movementData)
//     {
//         _controller = controller;
//         _stateMachine = stateMachine;
//         _data = movementData;
//     }
//
//     public void Enter()
//     {
//         // We'll set the animator to jumping from here, if needed
//         if (_controller.Animator != null)
//         {
//             _controller.Animator.SetBool("IsJumping", true);
//         }
//     }
//
//     public void Exit()
//     {
//         // On exit from jump state, we can reset the jump flag, etc.
//     }
//
//     public void HandleInput()
//     {
//         if (!_data.inputEnabled && !_data.isMovingToTarget)
//             return;
//
//         // If we detect we are grounded again => go to Walking
//         if (_data.grounded)
//         {
//             _stateMachine.ChangeState(_stateMachine.WalkingState);
//             return;
//         }
//     }
//
//     public void UpdateLogic()
//     {
//         // Always check ground detection
//         _controller.HandleGroundDetection();
//
//         // If we land => go to Walking
//         if (_data.grounded)
//         {
//             _stateMachine.ChangeState(_stateMachine.WalkingState);
//             return;
//         }
//
//         // Keep animations updated
//         _controller.UpdateAnimations();
//     }
//
//     public void UpdatePhysics()
//     {
//         // Horizontal movement in the air (allow left/right control in air)
//         if (!_data.isMovingToTarget)
//         {
//             float input = _controller.PlayerInputActions.Player.Move.ReadValue<Vector2>().x;
//             _data.inputActionsAxis = input;
//
//             if (Mathf.Abs(input) > 0.01f)
//             {
//                 // Accelerate towards target speed
//                 _data.velocity.x = Mathf.MoveTowards(
//                     _data.velocity.x, 
//                     input * _data.moveSpeed, 
//                     _data.acceleration * Time.fixedDeltaTime
//                 );
//             }
//             else
//             {
//                 // Decelerate horizontally while in air
//                 _data.velocity.x = Mathf.MoveTowards(
//                     _data.velocity.x, 
//                     0f, 
//                     _data.deceleration * Time.fixedDeltaTime
//                 );
//             }
//         }
//         else
//         {
//             // If we are in autonomous movement in mid-air
//             _controller.HandleAutonomousMovement();
//         }
//
//         // Apply gravity multiplied by jump/fall multipliers
//         _controller.ApplyGravity();
//
//         // Move Mario
//         _controller.MoveMario();
//     }
// }