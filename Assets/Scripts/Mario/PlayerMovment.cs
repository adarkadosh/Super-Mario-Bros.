using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer; // Layer for the ground
    [SerializeField] private float moveSpeed = 8f; // Horizontal movement speed
    [SerializeField] private float maxJumpHeight = 5f; // Maximum jump height
    [SerializeField] private float maxJumpTime = 1f; // Maximum time the jump can be sustained
    [SerializeField] private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    [SerializeField] private float fallMultiplier = 2f;
    [SerializeField] private float jumpMultiplier = 1f;


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
    private Vector3 jumpStartPosition;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        _inputActions = new InputSystem_Actions();
    }

    private void Update()
    {
        HorizontalMovement();
        // _isGrounded = Physics2D.Raycast(transform.position + Vector3.left * 0.51f, Vector2.down, 0.05f, groundLayer) ||
                      // Physics2D.Raycast(transform.position + Vector3.right * 0.51f, Vector2.down, 0.05f, groundLayer);
        _isGrounded = Physics2D.BoxCast(transform.position, new Vector2(1f, 0.1f), 0f, Vector2.down, 0.1f, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * 0.1f, Color.green); // Visualize BoxCast
        // Debug.DrawRay(transform.position + Vector3.left * 0.4f, Vector2.down * 0.375f, Color.green);
        // Debug.DrawRay(transform.position + Vector3.right * 0.4f, Vector2.down * 0.375f, Color.green);
        
        // Check if Mario is on the ground
        if (_isGrounded)
        {
            GroundMovement();
        }
        if (!_isJumping && _isGrounded)
        {
            float jumpDistance = Vector3.Distance(jumpStartPosition, transform.position);
            Debug.Log("Mario jumped " + jumpDistance + " units.");
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
            jumpStartPosition = transform.position; // Save the starting position
            velocity.y = JumpForce;
            _isJumping = true;
        }
    }

    private void ApplyGravity()
    {
        // Apply gravity to the velocity if Mario is not on the ground
        bool falling = velocity.y < 0f || !_inputActions.Player.Jump.inProgress;
        // Apply gravity based on the falling state (falling or jumping)
        float multiplier = falling ? fallMultiplier : jumpMultiplier;

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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            Vector2 direction = other.transform.position - transform.position;
            if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
            {
                velocity.y = 0f;
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (_isJumping)
        {
            // Draw a line from the starting position to Mario's current position
            Gizmos.color = Color.green;
            Gizmos.DrawLine(jumpStartPosition, transform.position);

            // Optionally, display the distance
            float distance = Vector3.Distance(jumpStartPosition, transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f); // Highlight Mario's position
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(jumpStartPosition, 0.1f); // Highlight starting position
        }
    }
}