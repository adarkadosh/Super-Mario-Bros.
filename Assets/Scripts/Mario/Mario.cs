// using Unity.Cinemachine;
// using UnityEngine;
// using UnityEngine.InputSystem;
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
//     private CinemachinePositionComposer _cinemachinePositionComposer;
//     private Animator _animator;
//     private Vector2 _velocity;
//     private float _inputActionsAxis;
//     private bool _isJumping;
//     private float _jumpTimeCounter;
//     private bool _inputEnabled = true;
//     private bool _isMovingToTarget = false;
//     private Vector3 _targetPosition;
//     private float _autonomousSpeed = 2f;
//
//     public bool Grounded { get; private set; }
//     public bool Flipped { get; private set; }
//
//     private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
//     private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);
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
//
//         HandleMovementInput();
//         HandleGroundDetection();
//         ApplyGravity();
//         UpdateAnimations();
//     }
//
//     private void FixedUpdate()
//     {
//         MoveMario();
//         if (_isMovingToTarget) HandleAutonomousMovement();
//     }
//
//     private void InitializeCinemachine()
//     {
//         _cinemachinePositionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
//         if (_cinemachinePositionComposer == null)
//         {
//             Debug.LogError("CinemachineTrackedDolly component not found on the Cinemachine Virtual Camera.");
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
//     }
//
//     private void DisableComponents()
//     {
//         _rigidbody.bodyType = RigidbodyType2D.Kinematic;
//         _capsuleCollider.enabled = false;
//         _velocity = Vector2.zero;
//         _inputActionsAxis = 0f;
//         _isJumping = false;
//     }
//
// v
//
//     private void HandleMovementInput()
//     {
//         if (_isMovingToTarget) return;
//
//         float input = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
//         _inputActionsAxis = input;
//
//         if (Mathf.Abs(input) > 0.01f)
//         {
//             _velocity.x = Mathf.MoveTowards(_velocity.x, input * moveSpeed, acceleration * Time.deltaTime);
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
//         HandleHorizontalAnimation();
//     }
//
//     private void HandleHorizontalAnimation()
//     {
//         if (_velocity.x > 0f)
//         {
//             Flipped = false;
//             transform.eulerAngles = Vector3.zero;
//         }
//         else if (_velocity.x < 0f)
//         {
//             Flipped = true;
//             transform.eulerAngles = new Vector3(0f, 180f, 0f);
//         }
//
//         _animator.SetBool(WalkingAnimation, Mathf.Abs(_velocity.x) > 0.1f);
//     }
//
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
//     private void HandleGroundDetection()
//     {
//         Grounded = IsGrounded();
//
//         if (Grounded)
//         {
//             _animator.SetBool(IsJumping, false);
//             _velocity.y = Mathf.Max(_velocity.y, 0f);
//             _isJumping = _velocity.y > 0f;
//         }
//     }
//
//     private void OnJump(InputAction.CallbackContext context)
//     {
//         if (Grounded)
//         {
//             _isJumping = true;
//             _jumpTimeCounter = maxJumpTime;
//             _velocity.y = JumpForce;
//             _animator.SetBool(IsJumping, true);
//         }
//     }
//
//     private void OnJumpStop(InputAction.CallbackContext context)
//     {
//         _isJumping = false;
//     }
//
//     private void ApplyGravity()
//     {
//         bool falling = _velocity.y < 0f || !_isJumping;
//         float multiplier = falling ? gravityMultiplier : onAirMultiplier;
//
//         _velocity.y += Gravity * multiplier * Time.deltaTime;
//         _velocity.y = Mathf.Max(_velocity.y, Gravity / 2f);
//     }
//
//     private void MoveMario()
//     {
//         Vector2 position = _rigidbody.position;
//         position += _velocity * Time.fixedDeltaTime;
//
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
//         if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
//         {
//             if (Vector2.Dot(direction.normalized, Vector2.down) > 0.25f)
//             {
//                 _velocity.y = JumpForce;
//                 _velocity.x = moveSpeed * Mathf.Sign(direction.x);
//                 _isJumping = true;
//                 _animator.SetBool(IsJumping, true);
//             }
//         }
//         else if (other.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
//         {
//             if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
//             {
//                 _velocity.y = 0f;
//                 _animator.SetBool(IsJumping, false);
//             }
//         }
//     }
//
//     private void UpdateAnimations()
//     {
//         if (_animator == null) return;
//
//         _animator.SetBool(WalkingAnimation, Mathf.Abs(_velocity.x) > 0.1f);
//         _animator.SetBool(IsJumping, _isJumping);
//         _animator.SetBool(IsSliding, (_inputActionsAxis > 0f && _velocity.x < 0f) || (_inputActionsAxis < 0f && _velocity.x > 0f));
//     }
//
//     public void DisableInput()
//     {
//         _inputEnabled = false;
//     }
//
//     public void EnableInput()
//     {
//         _inputEnabled = true;
//     }
//
//     public void MoveToPosition(Vector3 target, float speed = 2f)
//     {
//         _targetPosition = new Vector3(target.x, transform.position.y, transform.position.z);
//         _autonomousSpeed = speed;
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
//             _velocity.x = Mathf.MoveTowards(_velocity.x, desiredInput * moveSpeed, acceleration * Time.deltaTime);
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
//         HandleHorizontalAnimation();
//
//         if (Mathf.Abs(transform.position.x - _targetPosition.x) < 0.1f)
//         {
//             _isMovingToTarget = false;
//         }
//     }
// }