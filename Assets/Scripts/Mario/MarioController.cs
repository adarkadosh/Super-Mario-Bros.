using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple 2D Platformer controller with basic "1980s-style" horizontal movement and jump.
/// Attach this to a 2D character with a Rigidbody2D and Collider2D.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MarioController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Horizontal movement speed.")]
    public float moveSpeed = 5f;

    [Tooltip("Initial jump velocity (upward force).")]
    public float jumpForce = 7.5f;

    [Tooltip("If true, the player can vary the jump height by releasing the button early.")]
    public bool variableJump = true;

    [Tooltip("Gravity multiplier applied while the player is not pressing jump.")]
    public float fallMultiplier = 2f;

    [Header("Ground Check")]
    [Tooltip("How far below the character to check for ground.")]
    public float groundCheckDistance = 0.05f;

    [Tooltip("The layers considered ground.")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private Collider2D _collider;

    // Flags
    private bool _isGrounded;
    private bool _isJumping;
    private bool _jumpButtonPressed;

    void Awake()
    {
        // Cache components
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Read horizontal input (A/D, Left/Right arrows)
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Move horizontally
        Vector2 vel = _rb.linearVelocity;
        vel.x = moveInput * moveSpeed;
        _rb.linearVelocity = vel;

        // Flip sprite if needed (optional, if you have a sprite to flip)
        if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);

        // Check if jump is pressed
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            // Start a jump
            _isJumping = true;
            _jumpButtonPressed = true;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // If jump is released early and variable jumps are enabled, reduce upward velocity
            _jumpButtonPressed = false;
            if (variableJump && _rb.linearVelocity.y > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
            }
        }

        // Simple ground check by casting down a small distance
        _isGrounded = CheckGrounded();
    }

    void FixedUpdate()
    {
        // Apply higher gravity when falling (for a snappier jump feel)
        if (!_isGrounded && variableJump)
        {
            if (!_jumpButtonPressed && _rb.linearVelocity.y < 0)
            {
                // Increase gravity multiplier
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// Raycast or circle-cast below the player’s feet to see if we’re on the ground.
    /// </summary>
    private bool CheckGrounded()
    {
        Vector2 start = _collider.bounds.center;
        float radius = _collider.bounds.extents.x * 0.5f;
        // Or use the smaller side of the collider as your “foot radius”

        RaycastHit2D hit = Physics2D.Raycast(start, Vector2.down, 
                                             _collider.bounds.extents.y + groundCheckDistance, 
                                             groundLayer);

        // Optionally, draw a debug line (visible in the Scene view)
        Debug.DrawLine(start, start + Vector2.down * (_collider.bounds.extents.y + groundCheckDistance),
                       hit ? Color.green : Color.red);

        return (hit.collider != null);
    }
}