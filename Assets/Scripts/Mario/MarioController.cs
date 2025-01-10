using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MarioController : MonoBehaviour
{
    // Input
    private PlayerInputActions inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // Movement parameters
    [Header("Movement Settings")]
    public float moveSpeed = 5f;             // Maximum horizontal speed
    public float acceleration = 50f;         // How quickly Mario accelerates
    public float deceleration = 50f;         // How quickly Mario decelerates
    public float jumpForce = 12f;            // Initial jump impulse
    public float sustainedJumpForce = 20f;   // Additional force while holding jump
    public float gravityScale = 3f;          // Gravity scale for Mario

    [Header("Sliding Settings")]
    public float slideFriction = 10f;        // Sliding friction when no input is detected

    // Ground check
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    private bool isGrounded;

    // Coyote Time
    [Header("Coyote Time")]
    public float coyoteTimeDuration = 0.2f;
    private float coyoteTimeCounter;

    // Input variables
    private float moveInput;
    private bool isJumping;
    private float jumpTimer;
    public float maxJumpDuration = 1f;       // Maximum duration to hold jump

    private void Awake()
    {
        // Initialize input actions
        inputActions = new PlayerInputActions();

        moveAction = inputActions.Player.Move;
        jumpAction = inputActions.Player.Jump;

        // Initialize components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Enable input actions
        inputActions.Player.Enable();

        // Subscribe to jump events
        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        // Disable input actions
        inputActions.Player.Disable();

        // Unsubscribe from jump events
        jumpAction.performed -= OnJumpPerformed;
        jumpAction.canceled -= OnJumpCanceled;
    }

    private void Start()
    {
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        // Read movement input
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        moveInput = inputVector.x;

        // Ground check
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update Animator parameters
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        AdjustGravity();
    }

    private void HandleMovement()
    {
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            // Movement with input
            float targetSpeed = moveInput * moveSpeed;
            float speedDifference = targetSpeed - rb.linearVelocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, 0.9f) * Mathf.Sign(speedDifference);
            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

            // Limit speed
            if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            // Sliding when no input
            float friction = slideFriction * Time.fixedDeltaTime;
            if (Mathf.Abs(rb.linearVelocity.x) > friction)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x - Mathf.Sign(rb.linearVelocity.x) * friction, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    private void HandleJump()
    {
        if (isJumping && !isGrounded)
        {
            if (jumpTimer < maxJumpDuration)
            {
                // Apply sustained jump force
                rb.AddForce(Vector2.up * sustainedJumpForce * Time.fixedDeltaTime);
                jumpTimer += Time.fixedDeltaTime;
            }
            else
            {
                // Reached max jump duration
                isJumping = false;
            }
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (coyoteTimeCounter > 0f)
        {
            // Initial jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset Y velocity
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Start sustained jump
            isJumping = true;
            jumpTimer = 0f;

            // Reset coyote time
            coyoteTimeCounter = 0f;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        // Stop applying sustained jump force
        isJumping = false;
    }

    private void AdjustGravity()
    {
        if (rb.linearVelocity.y > 0 && !isJumping)
        {
            rb.gravityScale = gravityScale * 2; // Increase gravity when falling
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }
    
    
    // Ground Check
    private void LateUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // Optional: Visualize ground check in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}