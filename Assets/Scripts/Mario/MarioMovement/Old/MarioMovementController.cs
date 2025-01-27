// using Mario.MarioMovement.Old;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using Unity.Cinemachine;
//
// [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
// public class MarioMovementController : MonoBehaviour
// {
//     // Expose the animator hashes so states can reference them if needed
//     public static readonly int IsJumping = Animator.StringToHash("IsJumping");
//     private static readonly int IsSliding = Animator.StringToHash("IsSliding");
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
//     [SerializeField] private float offsetTransitionSpeed = 2f;
//     [SerializeField] private float movementThreshold = 0.1f;
//
//     [Header("Movement Settings")] 
//     [SerializeField] private float moveSpeed = 7f;
//     [SerializeField] private float maxJumpHeight = 4.8f;
//     [SerializeField] private float maxJumpTime = 1.15f;
//     [SerializeField] private float gravityMultiplier = 5f;
//     [SerializeField] private float onAirMultiplier = 1f;
//     [SerializeField] private float acceleration = 17f;
//     [SerializeField] private float deceleration = 25f;
//
//     [Header("Ground Detection")] 
//     [SerializeField] private LayerMask layerMask;
//     [SerializeField] private float groundRadius = 0.25f;
//     [SerializeField] private float groundDistance = 0.375f;
//
//     // Public so states can read/change them
//     [HideInInspector] public bool _isJumping;
//     [HideInInspector] public bool _onJump;
//     [HideInInspector] public Vector2 _velocity;
//     [HideInInspector] public Animator _animator;
//
//     private Camera _mainCamera;
//     private Rigidbody2D _rigidbody;
//     private Collider2D _capsuleCollider;
//     private PlayerInputActions _playerInputActions;
//     private CinemachinePositionComposer _cinemachinePositionComposer;
//     private float _inputActionsAxis;
//     private bool _inputEnabled = true;
//     private bool _isMovingToTarget;
//     private Vector3 _targetPosition;
//
//     // State machine
//     private IMovementState _currentState;
//
//     // Check if the player is grounded
//     public bool Grounded { get; private set; }
//
//     // Calculate jump force based on max jump height and time
//     public float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
//
//     // Calculate gravity based on max jump height and time
//     private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);
//
//     // True if Mario is walking horizontally
//     private bool Walking => Mathf.Abs(_velocity.x) > 0.1f || Mathf.Abs(_inputActionsAxis) > 0.1f;
//
//     public PlayerInputActions PlayerInputActions
//     {
//         get
//         {
//             if (_playerInputActions == null)
//                 _playerInputActions = new PlayerInputActions();
//             return _playerInputActions;
//         }
//     }
//
//     private void Awake()
//     {
//         _mainCamera = Camera.main;
//         _rigidbody = GetComponent<Rigidbody2D>();
//         _capsuleCollider = GetComponent<Collider2D>();
//     }
//
//     private void Start()
//     {
//         InitializeCinemachine();
//         InitializeAnimator();
//
//         // Start in the WalkingState
//         ChangeState(new Mario.MarioMovement.Old.MarioWalkingState());
//     }
//
//     private void OnEnable()
//     {
//         EnableComponents();
//         SubscribeInputActions();
//     }
//
//     private void OnDisable()
//     {
//         DisableComponents();
//         UnsubscribeInputActions();
//     }
//
//     private void Update()
//     {
//         if (!_inputEnabled && !_isMovingToTarget) return;
//         // Let the current state decide what to do each frame
//         _currentState?.Execute(this);
//     }
//
//     private void FixedUpdate()
//     {
//         // Movement code that must be in FixedUpdate can remain here,
//         // for example actual rigidbody movement
//         MoveMario();
//
//         if (_isMovingToTarget)
//             HandleAutonomousMovement();
//     }
//
//
//     public void ChangeState(IMovementState newState)
//     {
//         // Exit current state
//         _currentState?.Exit(this);
//         // Switch to the new state
//         _currentState = newState;
//         // Enter new state
//         _currentState.Enter(this);
//     }
//
//
//     private void InitializeCinemachine()
//     {
//         _cinemachinePositionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
//         if (_cinemachinePositionComposer == null)
//         {
//             Debug.LogError("CinemachinePositionComposer not found on the Cinemachine Virtual Camera.");
//         }
//     }
//
//     private void InitializeAnimator()
//     {
//         _animator = GetComponent<Animator>();
//         if (_animator == null)
//         {
//             Debug.LogError("Animator component not found on " + gameObject.name);
//         }
//     }
//
//     private void EnableComponents()
//     {
//         _rigidbody.bodyType = RigidbodyType2D.Dynamic;
//         _capsuleCollider.enabled = true;
//         _velocity = Vector2.zero;
//         _isJumping = false;
//         _onJump = false;
//     }
//
//     private void DisableComponents()
//     {
//         _rigidbody.bodyType = RigidbodyType2D.Kinematic;
//         _capsuleCollider.enabled = false;
//         _velocity = Vector2.zero;
//         _inputActionsAxis = 0f;
//         _isJumping = false;
//         _onJump = false;
//     }
//     
//     
//     private void SubscribeInputActions()
//     {
//         PlayerInputActions.Enable();
//         PlayerInputActions.Player.Jump.started += OnJump;
//         PlayerInputActions.Player.Jump.canceled += OnJumpStop;
//     }
//
//     private void UnsubscribeInputActions()
//     {
//         PlayerInputActions.Player.Jump.started -= OnJump;
//         PlayerInputActions.Player.Jump.canceled -= OnJumpStop;
//         PlayerInputActions.Disable();
//     }
//
//     private void OnJump(InputAction.CallbackContext context)
//     {
//         if (Grounded)
//         {
//             Debug.Log("Jump initiated");
//             ChangeState(new MarioJumpingState());
//         }
//     }
//
//     private void OnJumpStop(InputAction.CallbackContext context)
//     {
//         _isJumping = false;
//         // _isJumping = false;
//     }
//     
//     public void HandleMovementInput()
//     {
//         if (_isMovingToTarget) return;
//
//         var input = PlayerInputActions.Player.Move.ReadValue<Vector2>().x;
//         _inputActionsAxis = input;
//
//         // Horizontal acceleration/deceleration
//         if (Mathf.Abs(input) > 0.01f)
//         {
//             _velocity.x = Mathf.MoveTowards(_velocity.x, input * moveSpeed, acceleration * Time.deltaTime);
//         }
//         else
//         {
//             _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);
//         }
//
//         // If grounded and velocity is very small, zero it out
//         if (Grounded && Mathf.Abs(_velocity.x) < 0.1f)
//         {
//             _velocity.x = 0f;
//         }
//
//         FlipSprite();
//     }
//
//     private void FlipSprite()
//     {
//         if (_velocity.x > 0f)
//             transform.eulerAngles = Vector3.zero;
//         else if (_velocity.x < 0f)
//             transform.eulerAngles = new Vector3(0f, 180f, 0f);
//     }
//
//     public void HandleGroundDetection()
//     {
//         Grounded = IsGrounded();
//
//         // If we just landed and we were jumping, go to WalkingState
//         // if (Grounded && _currentState is not MarioWalkingState)
//         // {
//             // ChangeState(new MarioWalkingState());
//         // }
//     }
//
//     public bool IsGrounded()
//     {
//         const float extraHeight = 0.01f;
//         Vector2 leftRayOrigin = new Vector2(_capsuleCollider.bounds.min.x, _capsuleCollider.bounds.min.y);
//         Vector2 rightRayOrigin = new Vector2(_capsuleCollider.bounds.max.x, _capsuleCollider.bounds.min.y);
//
//         RaycastHit2D leftCheck = Physics2D.Raycast(leftRayOrigin, Vector2.down, extraHeight, layerMask);
//         RaycastHit2D rightCheck = Physics2D.Raycast(rightRayOrigin, Vector2.down, extraHeight, layerMask);
//
//         bool grounded = (leftCheck.collider != null && rightCheck.collider != null);
//         Color rayColor = grounded ? Color.green : Color.red;
//
//         Debug.DrawRay(leftRayOrigin, Vector2.down * extraHeight, rayColor);
//         Debug.DrawRay(rightRayOrigin, Vector2.down * extraHeight, rayColor);
//
//         return grounded;
//     }
//
//     public void ApplyGravity()
//     {
//         bool falling = _velocity.y < 0f || !_isJumping;
//         float multiplier = falling ? gravityMultiplier : onAirMultiplier;
//
//         _velocity.y += Gravity * multiplier * Time.deltaTime;
//         // Prevent from falling "forever" if something goes wrong
//         _velocity.y = Mathf.Max(_velocity.y, Gravity / 2f);
//     }
//
//     public void UpdateAnimations()
//     {
//         if (!_animator) return;
//         
//         // var walking = (Mathf.Abs(_velocity.x) > 0.1f || Mathf.Abs(_inputActionsAxis) > 0.1f);
//         var running = Mathf.Abs(_velocity.x) > 4f;
//
//         _animator.SetBool(WalkingAnimation, Walking);
//         _animator.speed = running ? 1f : 0.5f;
//         
//         _animator.SetBool(IsJumping, _onJump);
//
//         var isSliding = (_inputActionsAxis > 0f && _velocity.x < 0f)
//                         || (_inputActionsAxis < 0f && _velocity.x > 0f);
//         _animator.SetBool(IsSliding, isSliding);
//
//         _animator.speed = 1f;
//     }
//
//     private void MoveMario()
//     {
//         var position = _rigidbody.position;
//         position += _velocity * Time.fixedDeltaTime;
//
//         // Clamping to camera edges (optional)
//         Vector2 leftEdge = _mainCamera.ScreenToWorldPoint(Vector2.zero);
//         Vector2 rightEdge = _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
//         position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);
//
//         _rigidbody.MovePosition(position);
//     }
//
//     private void OnCollisionEnter2D(Collision2D other)
//     {
//         Vector2 direction = other.transform.position - transform.position;
//
//         if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
//         {
//             // Example logic: if we land on top of an enemy
//             if (Vector2.Dot(direction.normalized, Vector2.down) > 0.25f)
//             {
//                 _velocity.y = JumpForce / 2f;
//                 _velocity.x = moveSpeed;
//                 _isJumping = true;
//                 if (_animator != null)
//                     _animator.SetBool(IsJumping, true);
//             }
//         }
//         else if (other.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
//         {
//             // Example logic: if we hit something from below
//             if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
//             {
//                 _velocity.y = 0f;
//                 if (_animator != null)
//                     _animator.SetBool(IsJumping, false);
//             }
//         }
//     }
//     
//     public void DisableInput()
//     {
//         _playerInputActions.Disable();
//         _inputEnabled = false;
//     }
//
//     public void EnableInput()
//     {
//         _playerInputActions.Enable();
//         _inputEnabled = true;
//     }
//
//     public void MoveToPosition(Vector3 target)
//     {
//         _targetPosition = new Vector3(target.x, transform.position.y, transform.position.z);
//         _isMovingToTarget = true;
//         DisableInput();
//     }
//
//     private void HandleAutonomousMovement()
//     {
//         Vector3 direction = (_targetPosition - transform.position).normalized;
//         float desiredInput = Mathf.Sign(direction.x);
//         _inputActionsAxis = desiredInput;
//
//         if (Mathf.Abs(desiredInput) > 0.01f)
//         {
//             _velocity.x = Mathf.MoveTowards(_velocity.x, desiredInput * moveSpeed,
//                 acceleration * Time.deltaTime);
//         }
//         else
//         {
//             _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);
//         }
//
//         if (Grounded && Mathf.Abs(_velocity.x) < 0.1f)
//         {
//             _velocity.x = 0f;
//         }
//
//         FlipSprite();
//
//         if (Mathf.Abs(transform.position.x - _targetPosition.x) < 0.1f)
//         {
//             _isMovingToTarget = false;
//         }
//     }
// }