using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class MarioMoveController : MonoBehaviour
{
    private static readonly int IsSliding = Animator.StringToHash("IsSliding");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int WalkingAnimation = Animator.StringToHash("Walking");

    [Header("Camera Settings")] [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [SerializeField] private Vector3 cameraOffset = new Vector3(2.5f, 0, 0);
    [SerializeField] private float offsetTransitionSpeed = 2f;
    [SerializeField] private float movementThreshold = 0.1f;

    [Header("Movement Settings")] [SerializeField]
    private float moveSpeed = 8f;

    [SerializeField] private float maxJumpHeight = 4f; // Reduced for lower jump
    [SerializeField] private float maxJumpTime = 1f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float onAirMultiplier = 1f;
    [SerializeField] private float acceleration = 8f; // Acceleration rate
    [SerializeField] private float deceleration = 16f; // Deceleration rate

    [Header("Ground Detection")] [SerializeField]
    private LayerMask layerMask;

    [SerializeField] private float groundRadius = 0.25f;
    [SerializeField] private float groundDistance = 0.375f;


    private Camera _mainCamera;
    private Rigidbody2D _rigidbody;
    private Collider2D _capsuleCollider;
    private PlayerInputActions _playerInputActions;

    private CinemachinePositionComposer _cinemachinePositionComposer;

    private Animator _animator;

    private Vector2 _velocity;
    private float _inputActionsAxis;
    private bool _isJumping;
    private float _jumpTimeCounter;


    // private float _multiplier;

    // public float VelocityX => _velocity.x;

    public bool Grounded { get; private set; }
    private bool Jumping { get; set; }
    private bool Walking => Mathf.Abs(_velocity.x) > 0.1f || Mathf.Abs(_inputActionsAxis) > 0.1f;
    private bool Running => Mathf.Abs(_velocity.x) > 4f;

    private bool Sliding => (_inputActionsAxis > 0f && _velocity.x < 0f) ||
                            (_inputActionsAxis < 0f && _velocity.x > 0f);

    private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);

    // private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);
    private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);
    public PlayerInputActions PlayerInputActions => _playerInputActions;

    // mario is flipped?
    public bool Flipped { get; private set; }
    
    private bool _inputEnabled = true;

    // Variables for autonomous movement
    private bool isMovingToTarget = false;
    private Vector3 targetPosition;
    private float autonomousSpeed = 2f;


    private void Awake()
    {
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<Collider2D>();
        _playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // Initialize Cinemachine Composer
        _cinemachinePositionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
        if (_cinemachinePositionComposer == null)
        {
            Debug.LogError("CinemachineTrackedDolly component not found on the Cinemachine Virtual Camera.");
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
        _capsuleCollider.enabled = true;
        _velocity = Vector2.zero;
        Jumping = false;

        _playerInputActions.Enable();
        _playerInputActions.Player.Jump.started += OnJump;
        _playerInputActions.Player.Jump.canceled += OnJumpStop;
    }

    private void OnDisable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _capsuleCollider.enabled = false;
        _velocity = Vector2.zero;
        _inputActionsAxis = 0f;
        Jumping = false;

        _playerInputActions.Player.Jump.started -= OnJump;
        _playerInputActions.Player.Jump.canceled -= OnJumpStop;
        _playerInputActions.Disable();
    }

    private void Update()
    {
        if (!_inputEnabled && !isMovingToTarget)
            return;
        
        HandleMovementInput();
        HandleGroundDetection();
        ApplyGravity();
        // HandleCameraOffset();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        MoveMario();
        if (isMovingToTarget)
        {
            HandleAutonomousMovement();
        }
    }


    // private void HandleMovementInput()
    // {
    //     float input = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
    //     _inputActionsAxis = input;
    //
    //     if (Mathf.Abs(input) > 0.01f)
    //     {
    //         // Player is providing input: accelerate towards target speed
    //         _velocity.x = Mathf.MoveTowards(_velocity.x, input * moveSpeed, acceleration * Time.deltaTime);
    //     }
    //     else
    //     {
    //         // No input: decelerate to zero quickly
    //         _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);
    //     }
    //
    //     // Check if running into a wall
    //     if (Physics2D.CircleCast(_rigidbody.position, groundRadius, Vector2.right * _velocity.x,
    //             groundDistance, layerMask))
    //     {
    //         _velocity.x = 0f;
    //     }
    //
    //     HorizontalAnimationHandler();
    // }
    private void HandleMovementInput()
    {
        if (isMovingToTarget)
            return;
        
        float input = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
        _inputActionsAxis = input;

        if (Mathf.Abs(input) > 0.01f)
        {
            // Accelerate towards target speed
            _velocity.x = Mathf.MoveTowards(_velocity.x, input * moveSpeed, acceleration * Time.deltaTime);
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

        HorizontalAnimationHandler();
    }

    private void HorizontalAnimationHandler()
    {
        // Flip sprite to face direction
        if (_velocity.x > 0f)
        {
            Flipped = false;
            transform.eulerAngles = Vector3.zero;
        }
        else if (_velocity.x < 0f)
        {
            Flipped = true;
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        // Walk animation
        if (Walking)
        {
            _animator.SetBool(WalkingAnimation, true);
        }
        else
        {
            _animator.SetBool(WalkingAnimation, false);
        }
    }


    // private void HandleGroundDetection()
    // {
    //     Grounded = Physics2D.CircleCast(
    //         _rigidbody.position, groundRadius, Vector2.down, groundDistance, layerMask);
    //
    //
    //     if (Grounded)
    //     {
    //         _animator.SetBool(IsJumping, false);
    //
    //         // Prevent gravity from building up infinitely
    //         _velocity.y = Mathf.Max(_velocity.y, 0f);
    //         // Jumping = Mathf.Abs(_velocity.y) > 0f;
    //         Jumping = _velocity.y > 0f;
    //     }
    // }

    private bool IsGrounded()
    {
        const float extraHeight = 0.1f;
        var leftRayOrigin = new Vector2(_capsuleCollider.bounds.min.x, _capsuleCollider.bounds.min.y);
        var rightRayOrigin = new Vector2(_capsuleCollider.bounds.max.x, _capsuleCollider.bounds.min.y);

        var leftCheck = Physics2D.Raycast(leftRayOrigin, Vector2.down, extraHeight, layerMask);
        var rightCheck = Physics2D.Raycast(rightRayOrigin, Vector2.down, extraHeight, layerMask);

        var rayColor = (leftCheck.collider != null || rightCheck.collider != null) ? Color.green : Color.red;
        Debug.DrawRay(leftRayOrigin, Vector2.down * extraHeight, rayColor);
        Debug.DrawRay(rightRayOrigin, Vector2.down * extraHeight, rayColor);

        return leftCheck.collider != null || rightCheck.collider != null;
    }


    private void HandleGroundDetection()
    {
        Grounded = IsGrounded();

        if (Grounded)
        {
            _animator.SetBool(IsJumping, false);

            // Prevent gravity from building up infinitely
            _velocity.y = Mathf.Max(_velocity.y, 0f);
            Jumping = _velocity.y > 0f;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (Grounded)
        {
            _isJumping = true;
            _jumpTimeCounter = maxJumpTime;
            _velocity.y = JumpForce;
            _animator.SetBool(IsJumping, true);
        }
    }

    private void OnJumpStop(InputAction.CallbackContext context)
    {
        // _velocity.y -= JumpForce;
        _isJumping = false;
    }

    private void ApplyGravity()
    {
        // Check if falling
        bool falling = _velocity.y < 0f | !_isJumping;
        float multiplier = falling ? gravityMultiplier : onAirMultiplier;

        // Apply gravity and terminal velocity
        _velocity.y += Gravity * multiplier * Time.deltaTime;
        _velocity.y = Mathf.Max(_velocity.y, Gravity / 2f);
    }

    private void MoveMario()
    {
        // Move Mario based on his velocity
        Vector2 position = _rigidbody.position;
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
                // _velocity.y = JumpForce / 2f;
                _velocity.y = JumpForce;
                _velocity.x = moveSpeed;
                Jumping = true;

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
                    _animator.SetBool("IsJumping", false);
                }
            }
        }
    }

    private void HandleCameraOffset()
    {
        if (_cinemachinePositionComposer == null) return;

        // Get Mario's velocity
        float marioVelocityX = _velocity.x;

        // Determine if Mario is moving based on a threshold
        bool isMoving = Mathf.Abs(marioVelocityX) > movementThreshold;

        // Determine the direction Mario is moving
        Vector3 targetOffset;

        if (isMoving)
        {
            // Offset the camera based on movement direction
            targetOffset = new Vector3(Mathf.Sign(marioVelocityX) * cameraOffset.x, cameraOffset.y,
                cameraOffset.z);
        }
        else
        {
            // If not moving, center the camera
            targetOffset = Vector3.zero;
        }

        // Smoothly transition the camera offset
        _cinemachinePositionComposer.TargetOffset.x = Mathf.Lerp(
            _cinemachinePositionComposer.TargetOffset.x,
            0.5f + (Mathf.Abs(targetOffset.x) > movementThreshold
                ? (targetOffset.x / cameraOffset.x) * 0.1f
                : 0f),
            Time.deltaTime * offsetTransitionSpeed
        );
    }

    private void UpdateAnimations()
    {
        if (_animator == null) return;

        // Update speed
        // _animator.SetFloat(Speed, Mathf.Abs(velocity.x));
        if (Walking)
        {
            _animator.SetBool("Walking", true);
            _animator.speed = 0.5f;
            if (Running)
            {
                _animator.speed = 1f;
            }

            _animator.SetBool(IsSliding, Sliding);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }

        _animator.speed = 1f;
        // _animator.SetBool("Walking", Walking);

        // Update jumping state
        _animator.SetBool(IsJumping, Jumping);

        // Update sliding state
        // _animator.SetBool(IsSliding, Sliding);
    }

    public void DisableInput()
    {
        _playerInputActions.Disable();
        
        // _inputEnabled = false;
    }

    public void EnableInput()
    {
        _inputEnabled = true;
    }
    
    public void MoveToPosition(Vector3 target, float speed = 2f)
    {
        targetPosition = new Vector3(target.x, transform.position.y, transform.position.z); // Assuming horizontal movement
        autonomousSpeed = speed;
        isMovingToTarget = true;
        DisableInput(); // Ensure player input is disabled during autonomous movement
    }
    
    private void HandleAutonomousMovement()
    {
        // Calculate direction towards the target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Determine desired input axis based on direction
        float desiredInput = Mathf.Sign(direction.x);

        // Update the input axis to simulate player input
        _inputActionsAxis = desiredInput;

        // Apply movement logic based on desired input
        if (Mathf.Abs(desiredInput) > 0.01f)
        {
            // Accelerate towards target speed
            _velocity.x = Mathf.MoveTowards(_velocity.x, desiredInput * moveSpeed, acceleration * Time.deltaTime);
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
        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.1f)
        {
            isMovingToTarget = false;
            // EnableInput(); // Re-enable player input if needed
        }
    }
}