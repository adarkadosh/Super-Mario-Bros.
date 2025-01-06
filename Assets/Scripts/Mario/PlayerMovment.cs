using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    // public float initialJumpForce = 10f; // Force applied at the start of the jump
    // public float sustainedJumpForce = 5f; // Force applied while holding the button
    // public float maxJumpTime = 0.5f; // Maximum time the jump can be sustained
    //

    [SerializeField] LayerMask groundLayer; // Layer for the ground
    [SerializeField] private float moveSpeed = 8f; // Horizontal movement speed
    [SerializeField] private float maxJumpHeight = 5f; // Maximum jump height
    [SerializeField] private float maxJumpTime = 1f; // Maximum time the jump can be sustained
    [SerializeField] private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);

    [SerializeField]
    private float Gravity =>
        (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2); // Gravity

    private float _jumpTimeCounter; // Timer for holding jump

    // private float jumpTimeCounter; // How long the jump has been sustained
    private bool _isGrounded = true; // Is Mario on the ground?
    private bool _isJumping = false; // Is Mario currently jumping?
    private Rigidbody2D _rigidbody; // Mario's Rigidbody2D component
    private Camera _mainCamera; // Reference to the main camera


    private InputSystem_Actions _inputActions; // Reference to the input actions
    [SerializeField] private Vector2 velocity;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        _inputActions = new InputSystem_Actions();
        // InputActions.Player.Attack.performed += ctx => Shoot();
        // _inputActions.Player.Jump.performed += ctx => OnJumpStarted();
        // get if the key is pressed or released
        // _inputActions.Player.Jump.canceled += ctx => OnJumpCanceled();
    }

    private void Update()
    {
        HorizontalMovement();
        _isGrounded = Physics2D.Raycast(transform.position + Vector3.left * 0.1f, Vector2.down, 0.01f, groundLayer) ||
                      Physics2D.Raycast(transform.position + Vector3.right * 0.1f, Vector2.down, 0.01f, groundLayer);

        Debug.DrawRay(transform.position + Vector3.left * 0.1f, Vector2.down * 0.375f, Color.green);
        Debug.DrawRay(transform.position + Vector3.right * 0.1f, Vector2.down * 0.375f, Color.green);
        // Check if Mario is on the ground
        // _isGrounded = Physics2D.OverlapCircle(_rigidbody.position, groundCheckRadius, groundLayer);
        if (_isGrounded)
        {
            GroundMovement();
        }

        ApplyGravity();
    }

    private void HorizontalMovement()
    {
        velocity.x = Mathf.MoveTowards(velocity.x,
            _inputActions.Player.Move.ReadValue<Vector2>().x * moveSpeed, moveSpeed * Time.deltaTime);
    }

    private void GroundMovement()
    {
        // Move Mario Vertically
        velocity.y = Mathf.Max(velocity.y, 0f);
        _isJumping = velocity.y > 0;
        if (_inputActions.Player.Jump.triggered)
        {
            velocity.y = JumpForce;
            _isJumping = true;
        }
    }

    private void ApplyGravity()
    {
        // bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f);
        // Apply gravity to the velocity if Mario is not on the ground
        bool falling = velocity.y < 0f || !_inputActions.Player.Jump.inProgress;
        // Apply gravity based on the falling state (falling or jumping)
        float multiplier = falling ? 2f : 1f;

        velocity.y += Gravity * multiplier * Time.deltaTime;
        // limit the fall speed to half of the jump speed
        velocity.y = Mathf.Max(velocity.y, Gravity / 2f);
    }


    private void OnEnable()
    {
        // Raycast to check if Mario is on the ground
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    void FixedUpdate()
    {
        Vector2 position = _rigidbody.position;
        position += velocity * Time.fixedDeltaTime;
        _rigidbody.MovePosition(position);

        // Vector2 screenLeftEdge = _mainCamera.ScreenToWorldPoint(Vector3.zero);
        // Vector2 screenRightEdge = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
        // position.x = Mathf.Clamp(position.x, screenLeftEdge.x + (position.x / 2f),
        // screenRightEdge.x - (position.x / 2f));
        // _rigidbody.MovePosition(position);

        // Sustain the jump
        if (_isJumping && _jumpTimeCounter > 0)
        {
            // _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, sustainedJumpForce);
            _jumpTimeCounter -= Time.fixedDeltaTime;
        }
    }

    // private void OnJumpStarted()
    // {
    //     if (_isGrounded)
    //     {
    //         _isJumping = true;
    //         // _jumpTimeCounter = maxJumpTime;
    //         // rb.linearVelocity = new Vector2(rb.linearVelocity.x, initialJumpForce);
    //     }
    // }
    //
    // private void OnJumpCanceled()
    // {
    //     _isJumping = false;
    // }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // Check if Mario is on the ground
    //     if (collision.gameObject.layer == groundLayer)
    //     {
    //         _isGrounded = true;
    //     }
    //     else if (collision.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
    //     {
    //         // GameEvents.OnPowerUpCollected?.Invoke();
    //     }
    //     else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //     {
    //         // GameEvents.OnEnemyCollision?.Invoke();
    //     }
    // }
    //
    // private void OnCollisionExit2D(Collision2D collision)
    // {
    //     // Check if Mario leaves the ground
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         _isGrounded = false;
    //     }
    // }


    // private void OnDrawGizmosSelected()
    // {
    // Visualize ground check in the Scene view
    // if (groundCheck != null)
    // {
    // Gizmos.color = Color.green;
    // Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    // }
    // }
}