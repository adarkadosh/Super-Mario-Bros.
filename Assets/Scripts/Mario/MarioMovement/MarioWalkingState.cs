// using UnityEngine;
//
// /// <summary>
// /// The state representing Mario walking / running on the ground.
// /// </summary>
// public class MarioWalkingState : IMovementState
// {
//     private readonly MarioMoveController _controller;
//     private readonly MarioMovementStateMachine _stateMachine;
//     private readonly MovementData _data;
//
//     public MarioWalkingState(MarioMoveController controller, 
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
//         // Optionally reset or initialize any walking-related things here
//     }
//
//     public void Exit()
//     {
//         // Cleanup / exit logic, if any
//     }
//
//     public void HandleInput()
//     {
//         if (!_data.inputEnabled && !_data.isMovingToTarget)
//             return;
//
//         // Check if the jump was pressed - we rely on MarioMoveController's OnJump input hooking.
//         // The actual "switch to JumpingState" is done inside OnJump if grounded.
//         // So typically no direct "if pressed jump => jump" here unless we want duplication.
//
//         // If we detect we are no longer grounded (for example, walked off an edge):
//         if (!_data.grounded)
//         {
//             // Switch to jumping state (falling is also JumpingState here)
//             _stateMachine.ChangeState(_stateMachine.JumpingState);
//             return;
//         }
//     }
//
//     public void UpdateLogic()
//     {
//         // Check ground detection in case we step off an edge
//         _controller.HandleGroundDetection();
//
//         // If for some reason we are not grounded, go to JumpingState
//         if (!_data.grounded)
//         {
//             _stateMachine.ChangeState(_stateMachine.JumpingState);
//             return;
//         }
//
//         // Continue to apply "walking logic" animations, etc.
//         _controller.UpdateAnimations();
//     }
//
//     public void UpdatePhysics()
//     {
//         // If not in autonomous movement:
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
//                 // Decelerate to zero quickly
//                 _data.velocity.x = Mathf.MoveTowards(
//                     _data.velocity.x, 
//                     0f, 
//                     _data.deceleration * Time.fixedDeltaTime
//                 );
//             }
//         }
//         else
//         {
//             // If we are in autonomous movement, handle that
//             _controller.HandleAutonomousMovement();
//         }
//
//         // Prevent sliding if grounded and velocity is very small
//         if (_data.grounded && Mathf.Abs(_data.velocity.x) < 0.1f)
//         {
//             _data.velocity.x = 0f;
//         }
//
//         // Apply gravity while on ground just to clamp properly
//         // (Usually we don't add gravity if grounded, but the original code checks some max)
//         _controller.ApplyGravity();
//
//         // Now move Mario
//         _controller.MoveMario();
//     }
// }