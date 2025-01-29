using UnityEngine;
using UnityEngine.InputSystem;

namespace Mario
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class MarioMovementControl : MonoBehaviour
    {
        private static readonly int IsSliding = Animator.StringToHash("IsSliding");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int WalkingAnimation = Animator.StringToHash("Walking");

        [Header("Audio Settings")] 
        [SerializeField] private AudioClip smallJumpSound;
        [SerializeField] private AudioClip bigJumpSound;

        [Header("Movement Settings")] 
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float maxJumpHeight = 4.8f; // Reduced for lower jump
        [SerializeField] private float maxJumpTime = 1.15f;
        [SerializeField] private float gravityMultiplier = 5f;
        [SerializeField] private float onAirMultiplier = 1f;
        [SerializeField] private float acceleration = 17f; // Acceleration rate
        [SerializeField] private float deceleration = 25f; // Deceleration rate
        // [SerializeField] private float jumpBufferTime = 0.1f; // Buffer time for jump input
        // [SerializeField] private float jumpCoyoteTime = 0.1f; // Coyote time for jump input
        [SerializeField] private float gravityScaleOnAir = 5f; // Gravity scale when on air

        [Header("Ground Detection Settings")] 
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.75f, 0.1f);
        [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private Camera _mainCamera;
        private Rigidbody2D _rigidbody;
        private Collider2D _capsuleCollider;
        private PlayerInputActions _playerInputActions;
        private Animator _animator;
        private Vector2 _velocity;
        private float _inputActionsAxis;
        private bool _isJumping;
        private bool _jumping;
        private bool _inputEnabled = true;
        private bool _isMovingToTarget;
        private bool _isMarioBig;
        private Vector3 _targetPosition;

        public bool Grounded { get; private set; }
        private bool Walking => Mathf.Abs(_velocity.x) > 0.1f || Mathf.Abs(_inputActionsAxis) > 0.1f;
        private bool Running => Mathf.Abs(_velocity.x) > 4f;
        private bool Sliding => (_inputActionsAxis > 0f && _velocity.x < 0f) ||
                                (_inputActionsAxis < 0f && _velocity.x > 0f);
        private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
        private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

        public PlayerInputActions PlayerInputActions
        {
            get
            {
                if (_playerInputActions == null)
                {
                    _playerInputActions = new PlayerInputActions();
                }

                return _playerInputActions;
            }
        }

        private void Awake()
        {
            // _mainCamera = Camera.main;
            _rigidbody = GetComponent<Rigidbody2D>();
            _capsuleCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            // Initialize Animator
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }
        }

        private void OnEnable()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _capsuleCollider.enabled = true;
            _velocity = Vector2.zero;
            _jumping = false;
            // _inputEnabled = true;

            _playerInputActions.Enable();
            _playerInputActions.Player.Jump.started += OnJump;
            _playerInputActions.Player.Jump.canceled += OnJumpStop;
            MarioEvents.OnMarioStateChange += OnMarioStateChange;
        }

        private void OnDisable()
        {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _capsuleCollider.enabled = false;
            _velocity = Vector2.zero;
            _inputActionsAxis = 0f;
            _jumping = false;

            _playerInputActions.Player.Jump.started -= OnJump;
            _playerInputActions.Player.Jump.canceled -= OnJumpStop;
            _playerInputActions.Disable();
            MarioEvents.OnMarioStateChange -= OnMarioStateChange;
        }

        private void Update()
        {
            if (_isMovingToTarget)
                return;

            if (!_inputEnabled)
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);
                _animator.SetBool(WalkingAnimation, false);
                return;
            }


            Grounded = IsGrounded();
            if (Grounded)
            {
                HandleGroundDetection();
            }
            else
            {
                ApplyGravity();
            }

            HandleMovementInput();
            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            MoveMario();
            _rigidbody.gravityScale = Grounded ? 0f : gravityScaleOnAir;
            if (_isMovingToTarget)
            {
                HandleAutonomousMovement();
            }
        }

        private void HandleMovementInput()
        {
            if (_isMovingToTarget)
                return;

            var input = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
            _inputActionsAxis = input;

            _velocity.x = Mathf.Abs(input) > 0.01f
                // Accelerate towards target speed
                ? Mathf.MoveTowards(_velocity.x, input * moveSpeed, acceleration * Time.deltaTime)
                :
                // Decelerate to zero quickly
                Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);

            // Prevent sliding
            if (Grounded && Mathf.Abs(_velocity.x) < 0.01f)
            {
                _velocity.x = 0f;
            }

            HorizontalAnimationHandler();
        }

        private void HorizontalAnimationHandler()
        {
            transform.eulerAngles = _velocity.x switch
            {
                // Flip sprite to face direction
                > 0f => Vector3.zero,
                < 0f => new Vector3(0f, 180f, 0f),
                _ => transform.eulerAngles
            };

            // Walk animation
            _animator.SetBool(WalkingAnimation, Walking);
        }

        private bool IsGrounded()
        {
            // Vector2 origin = (Vector2)transform.position + _capsuleCollider.offset - new Vector2(0, _capsuleCollider.s / 2);
            var origin = (Vector2)transform.position + _capsuleCollider.offset -
                         new Vector2(0, _capsuleCollider.bounds.extents.y);
            var hit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.down, groundCheckDistance,
                groundLayer);

            // Debug visualization
            var rayColor = hit.collider != null ? Color.green : Color.red;
            Debug.DrawRay(origin + new Vector2(-groundCheckSize.x / 2, 0),
                Vector2.down * (groundCheckDistance),
                rayColor);
            Debug.DrawRay(origin + new Vector2(groundCheckSize.x / 2, 0),
                Vector2.down * (groundCheckDistance),
                rayColor);
            Debug.DrawRay(origin + new Vector2(-groundCheckSize.x / 2, -groundCheckDistance),
                Vector2.right * groundCheckSize.x, rayColor);

            return hit.collider != null;
        }


        private void HandleGroundDetection()
        {
            // Grounded = IsGrounded();

            if (!Grounded) return;
            // prevent mario from falling of the edge
            _animator.SetBool(IsJumping, false);

            // Prevent gravity from building up infinitely
            _velocity.y = Mathf.Max(_velocity.y, 0f);
            _jumping = _velocity.y > 0f;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!Grounded) return;
            var jumpSound = _isMarioBig ? bigJumpSound : smallJumpSound;
            SoundFXManager.Instance.PlaySound(jumpSound, transform);
            _isJumping = true;
            _velocity.y = JumpForce;
            _animator.SetBool(IsJumping, true);
        }

        private void OnJumpStop(InputAction.CallbackContext context)
        {
            _isJumping = false;
        }

        private void ApplyGravity()
        {
            // Check if falling
            var falling = _velocity.y < 0f | !_isJumping;
            var multiplier = falling ? gravityMultiplier : onAirMultiplier;

            // Apply gravity and terminal velocity
            _velocity.y += Gravity * multiplier * Time.deltaTime;
            _velocity.y = Mathf.Max(_velocity.y, Gravity / 2f);
        }

        private void MoveMario()
        {
            // Move Mario based on his velocity
            var position = _rigidbody.position;
            position += _velocity * Time.fixedDeltaTime;
            // _rigidbody.linearVelocity = new Vector2(_velocity.x, _rigidbody.linearVelocity.y);


            // Clamp within the screen bounds
            Vector2 leftEdge = _mainCamera.ScreenToWorldPoint(Vector2.zero);
            Vector2 rightEdge = _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

            _rigidbody.MovePosition(position);
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            Vector2 direction = other.transform.position - transform.position;
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // Bounce off enemy head
                if (Vector2.Dot(direction.normalized, Vector2.down) > 0.25f)
                {
                    _velocity.y = JumpForce;
                    _velocity.x = moveSpeed;
                    _jumping = true;

                    // Trigger jump animation
                    if (_animator != null)
                    {
                        _animator.SetBool(IsJumping, true);
                    }
                }
            }
            else if (other.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
            {
                // Stop vertical movement if Mario bonks his head
                if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
                {
                    _velocity.y = 0f;

                    // If Mario was jumping, set IsJumping to false
                    if (_animator != null)
                    {
                        _animator.SetBool(IsJumping, false);
                    }
                }
            }
        }

        private void UpdateAnimations()
        {
            if (_animator == null) return;
            if (Walking)
            {
                _animator.SetBool(WalkingAnimation, true);
                _animator.speed = 0.5f;
                if (Running)
                {
                    _animator.speed = 1f;
                }

                _animator.SetBool(IsSliding, Sliding);
            }
            else
            {
                _animator.SetBool(WalkingAnimation, false);
            }

            _animator.speed = 1f;
            // _animator.SetBool("Walking", Walking);

            // Update jumping state
            _animator.SetBool(IsJumping, _jumping);
        }

        public void DisableInputSystem()
        {
            _playerInputActions.Disable();
            // _inputEnabled = false;
        }

        public void EnableInputSystem()
        {
            _playerInputActions.Enable();
            // _inputEnabled = true;
        }

        public void DisableMovement()
        {
            _inputEnabled = false;
        }

        public void EnableMovement()
        {
            _inputEnabled = true;
        }

        public void MoveToPosition(Vector3 target, float speed = 2f)
        {
            _targetPosition =
                new Vector3(target.x, transform.position.y,
                    transform.position.z); // Assuming horizontal movement
            _isMovingToTarget = true;
            DisableInputSystem(); // Ensure player input is disabled during autonomous movement
        }

        private void HandleAutonomousMovement()
        {
            // Calculate direction towards the target
            var direction = (_targetPosition - transform.position).normalized;

            // Determine desired input axis based on direction
            var desiredInput = Mathf.Sign(direction.x);

            // Update the input axis to simulate player input
            _inputActionsAxis = desiredInput;

            // Apply movement logic based on desired input
            if (Mathf.Abs(desiredInput) > 0.01f)
            {
                // Accelerate towards target speed
                _velocity.x = Mathf.MoveTowards(_velocity.x, desiredInput * moveSpeed,
                    acceleration * Time.deltaTime);
            }
            else
            {
                // Decelerate to zero quickly
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);
            }

            // Prevent sliding
            if (Grounded && Mathf.Abs(_velocity.x) < 0.1f)
            {
                _velocity.x = 0f;
            }

            // Update animations based on autonomous movement
            HorizontalAnimationHandler();

            // Check if Mario has reached the target
            if (Mathf.Abs(transform.position.x - _targetPosition.x) < 0.1f)
            {
                _isMovingToTarget = false;
            }
        }

        private void OnMarioStateChange(MarioState marioState)
        {
            if (marioState == MarioState.Star)
                return;
            _isMarioBig = marioState != MarioState.Small;
        }
    }
}