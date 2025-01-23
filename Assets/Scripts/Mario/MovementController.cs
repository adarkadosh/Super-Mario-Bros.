using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
    // Animator Parameter Hashes
    private static readonly int IsSliding = Animator.StringToHash("IsSliding");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");

    [Header("Camera Settings")]
    [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [SerializeField] private Vector3 cameraOffset = new Vector3(2.5f, 0, 0);
    [SerializeField] private float offsetTransitionSpeed = 2f;
    [SerializeField] private float movementThreshold = 0.1f;

    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 8f;

    [SerializeField] private float maxJumpHeight = 4f; // Reduced for lower jump
    [SerializeField] private float maxJumpTime = 1f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float onAirMultiplier = 1f;
    [SerializeField] private float acceleration = 8f; // Acceleration rate
    [SerializeField] private float deceleration = 16f; // Deceleration rate

    [Header("Ground Detection")]
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField] private float groundRadius = 0.25f;
    [SerializeField] private float groundDistance = 0.375f;

    [Header("Jump Enhancements")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Camera _mainCamera;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private PlayerInputActions _playerInputActions;

    private CinemachineFramingTransposer _cinemachineFramingTransposer;

    private Animator _animator;

    private Vector2 _velocity;
    private float _inputActionsAxis;
    private bool _isJumping;
    private float _jumpTimeCounter;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Cached Layer Masks
    private int enemyLayer;
    private int powerUpLayer;

    public bool Grounded { get; private set; }
    private bool Jumping { get; set; }
    private bool Walking => Mathf.Abs(_velocity.x) > 0.1f || Mathf.Abs(_inputActionsAxis) > 0.1f;
    private bool Running => Mathf.Abs(_velocity.x) > 4f;

    private bool Sliding => (_inputActionsAxis > 0f && _velocity.x < 0f) ||
                            (_inputActionsAxis < 0f && _velocity.x > 0f);

    private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _playerInputActions = new PlayerInputActions();

        // Cache Layer Masks
        enemyLayer = LayerMask.NameToLayer("Enemy");
        powerUpLayer = LayerMask.NameToLayer("PowerUp");
    }

    private void Start()
    {
        // Initialize Cinemachine Framing Transposer
        // _cinemachineFramingTransposer = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (_cinemachineFramingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer component not found on the Cinemachine Virtual Camera.");
        }

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
        _boxCollider.enabled = true;
        _velocity = Vector2.zero;
        Jumping = false;
        _isJumping = false;

        _playerInputActions.Enable();
        _playerInputActions.Player.Jump.started += OnJump;
        _playerInputActions.Player.Jump.canceled += OnJumpStop;
    }

    private void OnDisable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _boxCollider.enabled = false;
        _velocity = Vector2.zero;
        _inputActionsAxis = 0f;
        Jumping = false;
        _isJumping = false;

        _playerInputActions.Player.Jump.started -= OnJump;
        _playerInputActions.Player.Jump.canceled -= OnJumpStop;
        _playerInputActions.Disable();
    }

    private void Update()
    {
        HandleMovementInput();
        // HandleJumpBuffer();
        HandleGroundDetection();
        ApplyGravity();
        HandleCameraOffset();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        MoveMario();
    }

    private void HandleMovementInput()
    {
        float input = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
        _inputActionsAxis = input;

        if (Mathf.Abs(input) > 0.01f)
        {
            // Player is providing input: accelerate towards target speed
            _velocity.x = Mathf.MoveTowards(_velocity.x, input * moveSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // No input: decelerate to zero quickly
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);
        }

        // Check if running into a wall
        if (Physics2D.CircleCast(_rigidbody.position, groundRadius, Vector2.right * _velocity.x,
                groundDistance, layerMask))
        {
            _velocity.x = 0f;
        }

        HorizontalAnimationHandler();
    }

    private void HandleJumpBuffer(InputAction.CallbackContext context)
    {
        if (_playerInputActions.Player.Jump.triggered)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            OnJump(context);
            jumpBufferCounter = 0f;
        }
    }

    private void HandleGroundDetection()
    {
        bool wasGrounded = Grounded;
        Grounded = IsGrounded();

        if (Grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Grounded)
        {
            _animator.SetBool(IsJumping, false);
            _velocity.y = Mathf.Max(_velocity.y, 0f);
            Jumping = _velocity.y > 0f;

            // Snap Mario to the ground to prevent floating or slight falls
            RaycastHit2D hit = Physics2D.Raycast(_rigidbody.position, Vector2.down, groundDistance + groundCheckRadius, layerMask);
            if (hit.collider != null)
            {
                float snapDistance = (hit.point.y + hit.collider.bounds.extents.y) - _rigidbody.position.y;
                if (snapDistance > 0.05f)
                {
                    _rigidbody.position += Vector2.up * snapDistance;
                }
            }
        }
        else
        {
            _animator.SetBool(IsJumping, true);
        }
    }

    private bool IsGrounded()
    {
        Vector2 position = _rigidbody.position;
        float distance = groundCheckRadius;

        // Define multiple points for ground checking
        Vector2[] groundCheckPoints = new Vector2[]
        {
            groundCheck.position + Vector3.left * (groundRadius * 0.9f),
            groundCheck.position,
            groundCheck.position + Vector3.right * (groundRadius * 0.9f)
        };

        foreach (var point in groundCheckPoints)
        {
            if (Physics2D.OverlapCircle(point, groundCheckRadius, layerMask))
                return true;
        }

        return false;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (Grounded || coyoteTimeCounter > 0f)
        {
            _isJumping = true;
            _jumpTimeCounter = maxJumpTime;
            _velocity.y = JumpForce;
            _animator.SetBool(IsJumping, true);
            Jumping = true;
        }
    }

    private void OnJumpStop(InputAction.CallbackContext context)
    {
        _isJumping = false;
    }

    private void ApplyGravity()
    {
        bool falling = _velocity.y < 0f || !_isJumping;
        float multiplier = falling ? gravityMultiplier : onAirMultiplier;

        _velocity.y += Gravity * multiplier * Time.deltaTime;
        _velocity.y = Mathf.Max(_velocity.y, Gravity / 2f);
    }

    private void MoveMario()
    {
        // Move Mario based on his velocity
        Vector2 position = _rigidbody.position;
        position += _velocity * Time.fixedDeltaTime;

        // Clamp within the screen bounds
        Vector2 leftEdge = _mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        _rigidbody.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == enemyLayer)
        {
            foreach (var contact in other.contacts)
            {
                // Check if collision is from above (player hitting enemy from above)
                if (contact.normal.y > 0.25f)
                {
                    _velocity.y = JumpForce;
                    _velocity.x = moveSpeed;
                    Jumping = true;

                    if (_animator != null)
                    {
                        _animator.SetBool(IsJumping, true);
                    }
                    break;
                }
            }
        }
        else if (other.gameObject.layer != powerUpLayer)
        {
            foreach (var contact in other.contacts)
            {
                // Check if collision is from below (player hitting a brick from below)
                if (contact.normal.y < -0.25f)
                {
                    _velocity.y = 0f;

                    if (_animator != null)
                    {
                        _animator.SetBool(IsJumping, false);
                    }
                    break;
                }
            }
        }
    }

    private void HandleCameraOffset()
    {
        if (_cinemachineFramingTransposer == null) return;

        float targetOffsetX = Mathf.Sign(_velocity.x) * cameraOffset.x;
        Vector3 targetOffset = new Vector3(targetOffsetX, cameraOffset.y, cameraOffset.z);

        _cinemachineFramingTransposer.m_TrackedObjectOffset = Vector3.Lerp(
            _cinemachineFramingTransposer.m_TrackedObjectOffset,
            targetOffset,
            Time.deltaTime * offsetTransitionSpeed
        );
    }

    private void HorizontalAnimationHandler()
    {
        // Flip sprite to face direction
        if (_velocity.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (_velocity.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        // Walk animation
        _animator.SetBool(IsWalking, Walking);
    }

    private void UpdateAnimations()
    {
        if (_animator == null) return;

        // Update running state
        _animator.SetBool(IsRunning, Running);

        // Update sliding state
        _animator.SetBool(IsSliding, Sliding);

        // Ensure animator speed is reset
        _animator.speed = 1f;

        // Update jumping state is handled in HandleGroundDetection and OnCollisionEnter2D
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check points
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Vector2[] groundCheckPoints = new Vector2[]
        {
            groundCheck.position + Vector3.left * (groundRadius * 0.9f),
            groundCheck.position,
            groundCheck.position + Vector3.right * (groundRadius * 0.9f)
        };

        foreach (var point in groundCheckPoints)
        {
            Gizmos.DrawWireSphere(point, groundCheckRadius);
        }
    }
}