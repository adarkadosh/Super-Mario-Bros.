using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A Mario-like controller that uses manual gravity, coyote time, 
/// and variable jump (hold for higher jump).
/// Rigidbody2D should have Gravity Scale = 0 in Inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Mario : MonoBehaviour
{
    [Header("Input")]
    private PlayerInputActions _inputActions;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 15f;

    [Header("Jump Settings")]
    [Tooltip("Maximum jump height in world units (when holding jump full duration).")]
    [SerializeField] private float maxJumpHeight = 4f;
    [Tooltip("Maximum time (in seconds) the jump can be held to reach maxJumpHeight.")]
    [SerializeField] private float maxJumpTime = 0.5f;

    [Tooltip("Gravity multiplier while falling.")]
    [SerializeField] private float fallMultiplier = 2f;
    [Tooltip("Gravity multiplier while still holding jump.")]
    [SerializeField] private float jumpMultiplier = 1f;

    [Tooltip("Manually set a jump force if desired. If 0, it will be auto-calculated.")]
    [SerializeField] private float customJumpForce = 0f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTimeDuration = 0.1f; 
    private float _coyoteTimeCounter;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("The size of the BoxCast for ground checking.")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);

    // Internal variables
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private bool _isGrounded;
    private bool _isJumping;
    private float _jumpTimeCounter;
    private float _gravity;        // Will be calculated based on maxJumpHeight / maxJumpTime if custom not used
    private float _jumpForce;      // Will be calculated if not custom
    private Vector3 _jumpStartPos; // For debug distance

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        
        // Setup input
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        // If customJumpForce is not set, compute jump force from maxJumpHeight & maxJumpTime
        if (Mathf.Approximately(customJumpForce, 0f))
        {
            // Explanation:
            // Gravity = (-2f * maxJumpHeight) / (maxJumpTime/2f)^2
            // JumpForce = (2f * maxJumpHeight) / (maxJumpTime/2f)
            _gravity = (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);
            _jumpForce = (2f * maxJumpHeight) / (maxJumpTime / 2f);
        }
        else
        {
            // If the user wants to override jumpForce, we still compute gravity
            // so that it’s somewhat consistent with the chosen maxJumpHeight and maxJumpTime.
            _jumpForce = customJumpForce;
            _gravity = (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);
        }

        // By default, we assume the RigidBody2D has Gravity Scale 0
        // so we can apply manual gravity.
        _rigidbody.gravityScale = 0f;
    }

    private void OnEnable()
    {
        _inputActions.Player.Jump.performed += OnJumpPerformed;
        _inputActions.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        _inputActions.Player.Jump.performed -= OnJumpPerformed;
        _inputActions.Player.Jump.canceled -= OnJumpCanceled;
        _inputActions.Disable();
    }

    private void Update()
    {
        // 1) Check if grounded
        CheckGrounded();

        // 2) Horizontal movement input
        float horizontalInput = _inputActions.Player.Move.ReadValue<Vector2>().x;
        HandleHorizontalMovement(horizontalInput);

        // 3) Handle coyote time
        UpdateCoyoteTime();

        // 4) If on ground and not currently jumping, measure jump distance
        if (_isGrounded && !_isJumping)
        {
            float jumpDistance = Vector3.Distance(_jumpStartPos, transform.position);
            // For debugging or analytics:
            // Debug.Log("Mario jumped " + jumpDistance + " units.");
        }

        // 5) Apply manual gravity
        ApplyGravity();

        // 6) Move the Rigidbody in FixedUpdate (so store velocity now)
        // _velocity is updated both in horizontal logic & vertical logic.
    }

    private void FixedUpdate()
    {
        // Actually move the body
        Vector2 newPos = _rigidbody.position + _velocity * Time.fixedDeltaTime;
        _rigidbody.MovePosition(newPos);

        // If we’re still jumping, reduce jump time counter
        if (_isJumping && _jumpTimeCounter > 0f)
        {
            _jumpTimeCounter -= Time.fixedDeltaTime;
        }
    }

    #region Movement Logic

    private void HandleHorizontalMovement(float horizontalInput)
    {
        float targetSpeed = horizontalInput * moveSpeed;

        // Decide if accelerating or decelerating
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            // Accelerate towards the target speed
            _velocity.x = Mathf.MoveTowards(
                _velocity.x,
                targetSpeed,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            // Decelerate to 0
            _velocity.x = Mathf.MoveTowards(
                _velocity.x,
                0f,
                deceleration * Time.deltaTime
            );
        }
    }

    private void ApplyGravity()
    {
        // Are we falling or did the player release jump?
        bool isFallingOrReleased = _velocity.y < 0f || !_inputActions.Player.Jump.inProgress;
        float multiplier = isFallingOrReleased ? fallMultiplier : jumpMultiplier;

        // Add gravity
        _velocity.y += _gravity * multiplier * Time.deltaTime;

        // (Optional) If you want to clamp the downward velocity
        // so Mario doesn't fall infinitely fast, uncomment below:
        // float maxFallSpeed = -2f * moveSpeed; // or another chosen value
        // _velocity.y = Mathf.Max(_velocity.y, maxFallSpeed);
    }

    private void CheckGrounded()
    {
        // BoxCast for ground check
        // We cast a tiny box downward from Mario’s position:
        // The box center is transform.position, size is groundCheckSize, 
        // 0 rotation, direction is Vector2.down, distance 0.05f, groundLayer
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, groundCheckSize, 0f, Vector2.down, 0.05f, groundLayer);

        bool wasGrounded = _isGrounded;
        _isGrounded = hit.collider != null;

        // If we just landed, reset vertical velocity if needed
        if (!wasGrounded && _isGrounded && _velocity.y < 0f)
        {
            _velocity.y = 0f;
        }
    }

    private void UpdateCoyoteTime()
    {
        if (_isGrounded)
        {
            _coyoteTimeCounter = coyoteTimeDuration;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    #endregion

    #region Jump Logic

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        // We can only jump if we are grounded or still within coyote time
        if (_coyoteTimeCounter > 0f)
        {
            _isJumping = true;
            _jumpTimeCounter = maxJumpTime; 
            _velocity.y = _jumpForce;

            // Save jump start position for distance measurement
            _jumpStartPos = transform.position;

            // Reset coyote time so we can’t double jump in midair
            _coyoteTimeCounter = 0f;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        // Stop the jump from sustaining further
        _isJumping = false;
        _jumpTimeCounter = 0f;
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Example:
        // If we hit a power-up block from below, you might handle that differently, etc.
        // For normal collisions, if we’re hitting from above, zero out Y velocity.
        if (other.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            Vector2 direction = other.transform.position - transform.position;
            // If we collided from below (the collision object is "above" Mario),
            // clamp velocity.y to avoid weird bouncing
            if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
            {
                _velocity.y = 0f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the box used for ground check
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, groundCheckSize);

        // If we are jumping, also draw the line from jump start
        if (_isJumping)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_jumpStartPos, transform.position);
        }
    }
}