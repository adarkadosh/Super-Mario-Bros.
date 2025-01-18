using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // If you're using Cinemachine for camera control

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class MarioMovement : MonoBehaviour
{
    #region Serialized Fields

    [Header("Movement Settings")]
    [Tooltip("Maximum horizontal speed while walking.")]
    [SerializeField] private float walkSpeed = 5f;

    [Tooltip("Maximum horizontal speed while running.")]
    [SerializeField] private float runSpeed = 8f;

    [Tooltip("Acceleration rate when starting to move.")]
    [SerializeField] private float acceleration = 20f;

    [Tooltip("Deceleration rate when stopping movement.")]
    [SerializeField] private float deceleration = 30f;

    [Tooltip("Threshold to determine running state.")]
    [SerializeField] private float runThreshold = 6f;

    [Header("Jump Settings")]
    [Tooltip("Maximum jump height.")]
    [SerializeField] private float maxJumpHeight = 4f;

    [Tooltip("Total time from jump start to landing (seconds).")]
    [SerializeField] private float maxJumpTime = 1f;

    [Tooltip("Duration of coyote time (seconds).")]
    [SerializeField] private float coyoteTime = 0.2f;

    [Tooltip("Time window to buffer jump input before landing (seconds).")]
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Gravity Settings")]
    [Tooltip("Multiplier for gravity when Mario is falling.")]
    [SerializeField] private float fallMultiplier = 2.5f;

    [Tooltip("Multiplier for gravity when Mario releases the jump button early.")]
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Tooltip("Maximum downward velocity.")]
    [SerializeField] private float maxFallSpeed = -20f;

    [Header("Ground Detection")]
    [Tooltip("Radius of the circle to check for ground.")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Tooltip("Offset from the center of the collider to check for ground.")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);

    [Tooltip("Layers considered as ground.")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Camera Settings")]
    [Tooltip("Reference to the Cinemachine Virtual Camera.")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Tooltip("Offset for the camera following Mario.")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(2.5f, 0, 0);

    [Tooltip("Speed at which the camera transitions to the new offset.")]
    [SerializeField] private float cameraTransitionSpeed = 2f;

    #endregion

    #region Private Variables

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Collider2D _collider;

    private PlayerInputActions _playerInputActions;

    private CinemachinePositionComposer _cinemachineTransposer;

    private Vector2 _velocity;
    private float _inputHorizontal;

    private bool _isFacingRight = true;

    // Jumping and Coyote Time
    private bool _isJumping;
    private bool _isGrounded;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    // Jump calculations
    private float _initialJumpVelocity;
    private float _gravity;

    #endregion

    #region Properties

    private bool IsRunning => Mathf.Abs(_velocity.x) > runThreshold;

    private bool IsSliding => (_inputHorizontal > 0f && _velocity.x < 0f) ||
                              (_inputHorizontal < 0f && _velocity.x > 0f);

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache components
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        // Initialize input actions
        _playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // Calculate initial jump velocity and gravity based on kinematic equations
        float timeToApex = maxJumpTime / 2f;
        _gravity = (-2f * maxJumpHeight) / Mathf.Pow(timeToApex, 2f);
        _initialJumpVelocity = (2f * maxJumpHeight) / timeToApex;

        // Set Rigidbody2D gravityScale to 0 to handle gravity manually
        _rigidbody.gravityScale = 0f;

        // Initialize Cinemachine
        if (cinemachineCamera != null)
        {
            // _cinemachineTransposer = cinemachineCamera.GetCinemachineComponent<CinemachinePositionComposer>();
            if (_cinemachineTransposer == null)
            {
                Debug.LogError("CinemachineTransposer component not found on the Cinemachine Virtual Camera.");
            }
        }
        else
        {
            Debug.LogWarning("Cinemachine Virtual Camera not assigned.");
        }
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Jump.started += OnJump;
        _playerInputActions.Player.Jump.canceled += OnJumpStop;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Jump.started -= OnJump;
        _playerInputActions.Player.Jump.canceled -= OnJumpStop;
        _playerInputActions.Disable();
    }

    private void Update()
    {
        HandleInput();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleVariableJump();
        UpdateAnimations();
        // HandleCameraOffset();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyGravity();
        MoveCharacter();
    }

    #endregion

    #region Input Handling

    private void HandleInput()
    {
        // Read horizontal movement input
        _inputHorizontal = _playerInputActions.Player.Move.ReadValue<Vector2>().x;

        // Read jump input buffer
        if (_playerInputActions.Player.Jump.triggered)
        {
            _jumpBufferCounter = jumpBufferTime;
        }
    }

    #endregion

    #region Jump Handling

    private void OnJump(InputAction.CallbackContext context)
    {
        _jumpBufferCounter = jumpBufferTime;
    }

    private void OnJumpStop(InputAction.CallbackContext context)
    {
        if (_rigidbody.linearVelocity.y > 0)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * 0.5f);
        }
    }

    private void HandleCoyoteTime()
    {
        if (_isGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void HandleJumpBuffer()
    {
        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
        {
            PerformJump();
            _jumpBufferCounter = 0f;
        }
    }

    private void PerformJump()
    {
        _isJumping = true;
        _velocity.y = _initialJumpVelocity;
        _animator.SetBool("IsJumping", true);
    }

    private void HandleVariableJump()
    {
        if (_isJumping)
        {
            if (_playerInputActions.Player.Jump.ReadValue<float>() > 0.1f)
            {
                // Continue rising if jump button is held and within jump time
                _velocity.y = _initialJumpVelocity;
            }
            else
            {
                // Apply additional gravity if jump button is released early
                _velocity.y += _gravity * lowJumpMultiplier * Time.deltaTime;
                _isJumping = false;
            }
        }
    }

    #endregion

    #region Movement Handling

    private void HandleMovement()
    {
        float targetSpeed = _inputHorizontal > 0f ? walkSpeed :
                            _inputHorizontal < 0f ? -walkSpeed : 0f;

        if (IsRunning)
        {
            targetSpeed = _inputHorizontal > 0f ? runSpeed :
                          _inputHorizontal < 0f ? -runSpeed : 0f;
        }

        float speedDifference = targetSpeed - _velocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, 0.9f) * Mathf.Sign(speedDifference);

        _velocity.x += movement * Time.fixedDeltaTime;
    }

    private void ApplyGravity()
    {
        if (!_isGrounded)
        {
            if (_rigidbody.linearVelocity.y < 0)
            {
                // Falling down: apply stronger gravity
                _velocity.y += _gravity * fallMultiplier * Time.fixedDeltaTime;
            }
            else if (_rigidbody.linearVelocity.y > 0 && !_isJumping)
            {
                // Rising but jump button released: apply increased gravity
                _velocity.y += _gravity * lowJumpMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                // Normal gravity while rising
                _velocity.y += _gravity * Time.fixedDeltaTime;
            }

            // Clamp the downward velocity to prevent excessive speeds
            if (_velocity.y < maxFallSpeed)
            {
                _velocity.y = maxFallSpeed;
            }
        }
    }

    private void MoveCharacter()
    {
        _rigidbody.linearVelocity = _velocity;
    }

    #endregion

    #region Ground Detection

    private void HandleGroundDetection()
    {
        Vector2 origin = _rigidbody.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.CircleCast(origin, groundCheckRadius, Vector2.down, 0.1f, groundLayer);

        _isGrounded = hit.collider != null;

        if (_isGrounded)
        {
            _isJumping = false;
            _animator.SetBool("IsJumping", false);
        }
    }

    private void FixedUpdateGroundDetection()
    {
        HandleGroundDetection();
    }

    #endregion

    #region Animation Handling

    private void UpdateAnimations()
    {
        // Walking animation
        bool isWalking = Mathf.Abs(_velocity.x) > 0.1f && _isGrounded;
        _animator.SetBool("IsWalking", isWalking);

        // Running animation
        bool isRunning = IsRunning && _isGrounded;
        _animator.SetBool("IsRunning", isRunning);

        // Sliding animation
        _animator.SetBool("IsSliding", IsSliding);

        // Facing direction
        if (_velocity.x > 0f && !_isFacingRight)
        {
            // Flip();
            transform.Rotate(0f, 180f, 0f);

        }
        else if (_velocity.x < 0f && _isFacingRight)
        {
            // Flip();
            transform.Rotate(0f, 180f, 0f);

        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0f, 180f, 0f);
        // Vector3 scaler = transform.localScale;
        // scaler.x *= -1;
        // transform.localScale = scaler;
    }

    #endregion

    #region Camera Handling

    // private void HandleCameraOffset()
    // {
    //     if (_cinemachineTransposer == null) return;
    //
    //     Vector3 targetOffset = _isFacingRight ? cameraOffset : new Vector3(-cameraOffset.x, cameraOffset.y, cameraOffset.z);
    //     _cinemachineTransposer.m_FollowOffset = Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, targetOffset, Time.deltaTime * cameraTransitionSpeed);
    // }

    #endregion

    #region Collision Handling

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleGroundDetection();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleGroundDetection();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _isGrounded = false;
    }

    #endregion
}
