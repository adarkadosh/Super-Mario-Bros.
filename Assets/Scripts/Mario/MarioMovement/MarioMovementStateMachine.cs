// public class MarioMovementStateMachine
// {
//     public IMovementState CurrentState { get; private set; }
//
//     // The two states we want:
//     public IMovementState WalkingState { get; private set; }
//     public IMovementState JumpingState { get; private set; }
//
//     private MarioMoveController _controller;
//     private MovementData _movementData;
//
//     public MarioMovementStateMachine(MarioMoveController controller, MovementData movementData)
//     {
//         _controller = controller;
//         _movementData = movementData;
//
//         // Instantiate the states
//         WalkingState = new MarioWalkingState(_controller, this, _movementData);
//         JumpingState = new MarioJumpingState(_controller, this, _movementData);
//
//         // Start in WalkingState by default
//         CurrentState = WalkingState;
//         CurrentState.Enter();
//     }
//
//     public void ChangeState(IMovementState newState)
//     {
//         if (CurrentState != null)
//         {
//             CurrentState.Exit();
//         }
//
//         CurrentState = newState;
//         if (CurrentState != null)
//         {
//             CurrentState.Enter();
//         }
//     }
// }