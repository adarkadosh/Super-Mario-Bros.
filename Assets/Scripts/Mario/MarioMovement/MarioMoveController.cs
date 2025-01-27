// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.Serialization;
// using Unity.Cinemachine; // Assuming your Cinemachine references
//
// [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
// public class MarioMoveController : MonoBehaviour
// {
//     private static readonly int IsSliding = Animator.StringToHash("IsSliding");
//     private static readonly int IsJumping = Animator.StringToHash("IsJumping");
//     private static readonly int WalkingAnimation = Animator.StringToHash("Walking");
//
//     [Header("Audio Settings")] 
//     [SerializeField] private AudioClip smallJumpSound;
//     [SerializeField] private AudioClip bigJumpSound;
//     [SerializeField] private AudioClip stompSound;
//     [SerializeField] private AudioClip bumpSound;
//
//     [Header("Camera Settings")] 
//     [SerializeField] private CinemachineCamera cinemachineCamera;
//     [SerializeField] private Vector3 cameraOffset = new Vector3(2.5f, 0, 0);
//     [SerializeField] private float offsetTransitionSpeed = 2f;
//     [SerializeField] private float movementThreshold = 0.1f;
//
//     [Header("Movement Settings")] 
//     [SerializeField] private float moveSpeed = 8f;
//     [SerializeField] private float maxJumpHeight = 4f; 
//     [SerializeField] private float maxJumpTime = 1f;
//     [SerializeField] private float gravityMultiplier = 2f;
//     [SerializeField] private float onAirMultiplier = 1f;
//     [SerializeField] private float acceleration = 8f; 
//     [SerializeField] private float deceleration = 16f; 
//
//     [Header("Ground Detection")] 
//     [SerializeField] private LayerMask layerMask;
//     [SerializeField] private float groundRadius = 0.25f;
//     [SerializeField] private float groundDistance = 0.375f;
//
//     private Camera _mainCamera;
//     private Rigidbody2D _rigidbody;
//     private Collider2D _capsuleCollider;
//     private PlayerInputActions _playerInputActions;
//
//     private CinemachinePositionComposer _cinemachinePositionComposer;
//     private Animator _animator;
//
//     // The new data + state machine
//     private MovementData _movementData;
//     private MarioMovementStateMachine _stateMachine;
//
//     // Accessor for external classes
//     public Animator Animator => _animator;
//
//     // Creates or returns your PlayerInputActions
//     public PlayerInputActions PlayerInputActions
//     {
//         get
//         {
//             if (_playerInputActions == null)
//             {
//                 _playerInputActions = new PlayerInputActions();
//             }
//             return _playerInputActions;
//         }
//     }
//
//     // Quick property for whether Mario is flipped
//     public bool Flipped => _movementData.flipped;
//
//     #region Unity Lifecycle
//
//     private void Awake()
//     {
//         _mainCamera = Camera.main;
//         _rigidbody = GetComponent<Rigidbody2D>();
//         _capsuleCollider = GetComponent<Collider2D>();
//
//         // Instantiate and populate MovementData with the current settings
//         _movementData = new MovementData
//         {
//             moveSpeed = moveSpeed,
//             maxJumpHeight = maxJumpHeight,
//             maxJumpTime = maxJumpTime,
//             gravityMultiplier = gravityMultiplier,
//             onAirMultiplier = onAirMultiplier,
//             acceleration = acceleration,
//             deceleration = deceleration,
//             inputEnabled = true
//         };
//
//         // Create StateMachine, passing in this controller and the MovementData
//         _stateMachine = new MarioMovementStateMachine(this, _movementData);
//     }
//
//     private void Start()
//     {
//         _cinemachinePositionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
//         if (_cinemachinePositionComposer == null)
//         {
//             Debug.LogError("CinemachinePositionComposer component not found on the Virtual Camera.");
//         }
//
//         _animator = GetComponent<Animator>();
//         if (_animator == null)
//         {
//             Debug.LogError("Animator component not found on " + gameObject.name);
//         }
//     }
//
//     private void OnEnable()
//     {
//         _rigidbody.bodyType = RigidbodyType2D.Dynamic;
//         _capsuleCollider.enabled = true;
//
//         PlayerInputActions.Enable();
//         PlayerInputActions.Player.Jump.started += OnJump;
//         PlayerInputActions.Player.Jump.canceled += OnJumpStop;
//     }
//
//     private void OnDisable()
//     {
//         _rigidbody.bodyType = RigidbodyType2D.Kinematic;
//         _capsuleCollider.enabled = false;
//
//         PlayerInputActions.Player.Jump.started -= OnJump;
//         PlayerInputActions.Player.Jump.canceled -= OnJumpStop;
//         PlayerInputActions.Disable();
//     }
//
//     private void Update()
//     {
//         // State-based approach:
//         _stateMachine.CurrentState.HandleInput();
//         _stateMachine.CurrentState.UpdateLogic();
//
//         // (Optional) If you want to do camera offset updates here:
//         // HandleCameraOffset();
//     }
//
//     private void FixedUpdate()
//     {
//         // All physics-based updates happen in the state's UpdatePhysics
//         _stateMachine.CurrentState.UpdatePhysics();
//     }
//
//     #endregion
//     
//     // Called from states or from the user input
//     public void OnJump(InputAction.CallbackContext context)
//     {
//         // If we are grounded, begin jump
//         if (_movementData.grounded)
//         {
//             _movementData.isJumping = true;
//             _movementData.jumpTimeCounter = _movementData.maxJumpTime;
//             _movementData.velocity.y = JumpForce;
//             if (_animator != null)
//             {
//                 _animator.SetBool(IsJumping, true);
//             }
//
//             // Switch to JumpingState if needed:
//             _stateMachine.ChangeState(_stateMachine.JumpingState);
//         }
//     }
//
//     public void OnJumpStop(InputAction.CallbackContext context)
//     {
//         _movementData.isJumping = false;
//     }
//
//     public void ApplyGravity()
//     {
//         bool falling = _movementData.velocity.y < 0f || !_movementData.isJumping;
//         float multiplier = falling ? _movementData.gravityMultiplier : _movementData.onAirMultiplier;
//
//         // Same formula from your original code
//         float gravityValue = (-2f * _movementData.maxJumpHeight) / Mathf.Pow(_movementData.maxJumpTime / 2f, 2f);
//         _movementData.velocity.y += gravityValue * multiplier * Time.deltaTime;
//
//         // You had a clamp to prevent infinite negativity
//         _movementData.velocity.y = Mathf.Max(_movementData.velocity.y, gravityValue / 2f);
//     }
//
//     public void MoveMario()
//     {
//         // Move Mario based on velocity
//         Vector2 position = _rigidbody.position;
//         position += _movementData.velocity * Time.fixedDeltaTime;
//
//         // Keep Mario within screen
//         Vector2 leftEdge = _mainCamera.ScreenToWorldPoint(Vector2.zero);
//         Vector2 rightEdge = _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
//         position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);
//
//         _rigidbody.MovePosition(position);
//
//         // Handle flipping in horizontal animation:
//         HorizontalAnimationHandler();
//     }
//
//     public void HandleGroundDetection()
//     {
//         _movementData.grounded = IsGrounded();
//         if (_movementData.grounded)
//         {
//             if (_animator != null)
//             {
//                 _animator.SetBool(IsJumping, false);
//             }
//             _movementData.velocity.y = Mathf.Max(_movementData.velocity.y, 0f);
//         }
//     }
//
//     // Raycast-based ground check
//     private bool IsGrounded()
//     {
//         const float extraHeight = 0.1f;
//         var leftRayOrigin = new Vector2(_capsuleCollider.bounds.min.x, _capsuleCollider.bounds.min.y);
//         var rightRayOrigin = new Vector2(_capsuleCollider.bounds.max.x, _capsuleCollider.bounds.min.y);
//
//         var leftCheck = Physics2D.Raycast(leftRayOrigin, Vector2.down, extraHeight, layerMask);
//         var rightCheck = Physics2D.Raycast(rightRayOrigin, Vector2.down, extraHeight, layerMask);
//
//         var rayColor = (leftCheck.collider != null || rightCheck.collider != null) ? Color.green : Color.red;
//         Debug.DrawRay(leftRayOrigin, Vector2.down * extraHeight, rayColor);
//         Debug.DrawRay(rightRayOrigin, Vector2.down * extraHeight, rayColor);
//
//         return leftCheck.collider != null || rightCheck.collider != null;
//     }
//
//     public void UpdateAnimations()
//     {
//         if (_animator == null) return;
//
//         bool walking = Mathf.Abs(_movementData.velocity.x) > 0.1f || Mathf.Abs(_movementData.inputActionsAxis) > 0.1f;
//         bool running = Mathf.Abs(_movementData.velocity.x) > 4f;
//         bool sliding = 
//             (_movementData.inputActionsAxis > 0f && _movementData.velocity.x < 0f) ||
//             (_movementData.inputActionsAxis < 0f && _movementData.velocity.x > 0f);
//
//         // Walk / Running
//         if (walking)
//         {
//             _animator.SetBool(WalkingAnimation, true);
//             // This was your approach to slow down the walk animation or speed it up
//             _animator.speed = running ? 1f : 0.5f;
//
//             _animator.SetBool(IsSliding, sliding);
//         }
//         else
//         {
//             _animator.SetBool(WalkingAnimation, false);
//         }
//
//         // Ensure we reset speed if we want
//         // (Optional if you want to keep it at 1 only if not walking)
//         if (!walking)
//         {
//             _animator.speed = 1f;
//         }
//
//         // Jump state
//         _animator.SetBool(IsJumping, _movementData.isJumping);
//     }
//
//     private void HorizontalAnimationHandler()
//     {
//         // Flip sprite to face direction
//         if (_movementData.velocity.x > 0f)
//         {
//             _movementData.flipped = false;
//             transform.eulerAngles = Vector3.zero;
//         }
//         else if (_movementData.velocity.x < 0f)
//         {
//             _movementData.flipped = true;
//             transform.eulerAngles = new Vector3(0f, 180f, 0f);
//         }
//     }
//
//     public void DisableInput()
//     {
//         PlayerInputActions.Disable();
//         _movementData.inputEnabled = false;
//     }
//
//     public void EnableInput()
//     {
//         PlayerInputActions.Enable();
//         _movementData.inputEnabled = true;
//     }
//
//     public void MoveToPosition(Vector3 target, float speed = 2f)
//     {
//         _movementData.targetPosition = new Vector3(target.x, transform.position.y, transform.position.z);
//         _movementData.autonomousSpeed = speed;
//         _movementData.isMovingToTarget = true;
//         DisableInput(); 
//     }
//
//     public void HandleAutonomousMovement()
//     {
//         // Calculate direction towards the target
//         Vector3 direction = (_movementData.targetPosition - transform.position).normalized;
//
//         // Determine desired input axis based on direction
//         float desiredInput = Mathf.Sign(direction.x);
//
//         _movementData.inputActionsAxis = desiredInput;
//
//         // Apply movement logic based on desired input
//         if (Mathf.Abs(desiredInput) > 0.01f)
//         {
//             // Accelerate towards target speed
//             _movementData.velocity.x = Mathf.MoveTowards(
//                 _movementData.velocity.x, 
//                 desiredInput * _movementData.moveSpeed, 
//                 _movementData.acceleration * Time.deltaTime
//             );
//         }
//         else
//         {
//             // Decelerate to zero
//             _movementData.velocity.x = Mathf.MoveTowards(
//                 _movementData.velocity.x, 
//                 0f, 
//                 _movementData.deceleration * Time.deltaTime
//             );
//         }
//
//         // Prevent sliding if grounded
//         if (_movementData.grounded && Mathf.Abs(_movementData.velocity.x) < 0.1f)
//         {
//             _movementData.velocity.x = 0f;
//         }
//
//         HorizontalAnimationHandler();
//
//         // Check if Mario has reached the target
//         if (Mathf.Abs(transform.position.x - _movementData.targetPosition.x) < 0.1f)
//         {
//             _movementData.isMovingToTarget = false;
//         }
//     }
//
//     // The same jump force formula from your code
//     private float JumpForce => 
//         (2f * _movementData.maxJumpHeight) / (_movementData.maxJumpTime / 2f);
//
//     private void OnCollisionEnter2D(Collision2D other)
//     {
//         Vector2 direction = other.transform.position - transform.position;
//
//         if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
//         {
//             // Bounce off enemy head
//             if (Vector2.Dot(direction.normalized, Vector2.down) > 0.25f)
//             {
//                 _movementData.velocity.y = JumpForce / 3f;
//                 _movementData.velocity.x = _movementData.moveSpeed * Mathf.Sign(direction.x);
//                 _movementData.isJumping = true;
//
//                 if (_animator != null)
//                 {
//                     _animator.SetBool(IsJumping, true);
//                 }
//             }
//         }
//         else if (other.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
//         {
//             // Stop vertical movement if Mario bonks his head
//             if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
//             {
//                 _movementData.velocity.y = 0f;
//                 if (_animator != null)
//                 {
//                     _animator.SetBool(IsJumping, false);
//                 }
//             }
//         }
//     }
//
//     // Optional camera offset if you want it (unchanged from your original)
//     private void HandleCameraOffset()
//     {
//         if (_cinemachinePositionComposer == null) return;
//
//         float marioVelocityX = _movementData.velocity.x;
//         bool isMoving = Mathf.Abs(marioVelocityX) > movementThreshold;
//
//         Vector3 targetOffset;
//         if (isMoving)
//         {
//             targetOffset = new Vector3(
//                 Mathf.Sign(marioVelocityX) * cameraOffset.x, 
//                 cameraOffset.y, 
//                 cameraOffset.z
//             );
//         }
//         else
//         {
//             targetOffset = Vector3.zero;
//         }
//
//         _cinemachinePositionComposer.TargetOffset.x = Mathf.Lerp(
//             _cinemachinePositionComposer.TargetOffset.x,
//             0.5f + (Mathf.Abs(targetOffset.x) > movementThreshold 
//                 ? (targetOffset.x / cameraOffset.x) * 0.1f 
//                 : 0f),
//             Time.deltaTime * offsetTransitionSpeed
//         );
//     }
// }