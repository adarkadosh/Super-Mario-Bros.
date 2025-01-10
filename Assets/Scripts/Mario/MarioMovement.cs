using System;
using UnityEngine;

public class MarioMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Horizontal movement speed
    [SerializeField] private float smallJumpForce = 10f; // Initial jump force
    [SerializeField] private float bigJumpForce = 15f; // Force for a big jump
    [SerializeField] private float maxJumpHoldTime = 0.2f; // Time allowed for holding the jump
    [SerializeField] private float maxSpeed = 10f;

    private Rigidbody2D _rigidbody;
    private float jumpTimeCounter; // Timer for holding jump
    private bool isGrounded = false; // Check if Mario is grounded
    private bool isJumping = false; // Flag to check if jumping
    private float jumpHoldTime = 0f; // Timer to track how long jump is held

    private Animator _animator;
    private Vector3 _initialPosition;

    private Vector3 _movement;
    private float _currentSpeed;


    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _initialPosition = transform.position;
        _currentSpeed = moveSpeed;
    }

    void Update()
    {
        GetMovement();
        SetAnimation();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        LimitVelocity();
    }

    private void GetMovement()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");

        // Handle jump input in Update for responsiveness
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            jumpHoldTime = 0f; // Reset jump hold timer
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false; // Stop adding force when the key is released
        }
    }
    
    private void HandleJump()
    {
        if (isJumping && isGrounded)
        {
            // Initial jump force
            _rigidbody.AddForce(Vector2.up * smallJumpForce);
            isGrounded = false; // Prevent double-jumping
        }
        else if (isJumping && jumpHoldTime < maxJumpHoldTime)
        {
            // Add additional force for a big jump
            // _rigidbody.AddForce(Vector2.up * (bigJumpForce * Time.fixedDeltaTime));
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, bigJumpForce);
            jumpHoldTime += Time.fixedDeltaTime;
        }
    }
    
    private void HandleMovement()
    {
        // _rigidbody.AddForce(Vector3.up * (_movement.y * _currentSpeed));
        _rigidbody.AddForce(Vector3.right * (_movement.x * moveSpeed));
    }

    private void SetAnimation()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (_movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (_movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        _animator.SetFloat("Horizontal", _rigidbody.linearVelocity.x);
        _animator.SetFloat("Vertical", _movement.y);
        _animator.SetFloat("Speed", Math.Abs(_rigidbody.linearVelocity.x));
    }

    private void LimitVelocity()
    {
        var velocity = _rigidbody.linearVelocity;
        // velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        // velocity.y = Mathf.Clamp(velocity.y, -maxSpeed, maxSpeed);

        // if (_movement == Vector3.zero)
        // {
        //     velocity = Vector2.zero;
        // }

        _rigidbody.linearVelocity = velocity;
    }

    private void OnEnable()
    {
        // ResetEvents.ResetPlayerPosition += ResetPosition;
        // GameEvents.OnSmileDeath += StopMovement;
        // ResetEvents.ResetPlayerHealth += ResumeMovement;
    }

    private void OnDisable()
    {
        // ResetEvents.ResetPlayerPosition -= ResetPosition;
        // GameEvents.OnSmileDeath -= StopMovement;
        // ResetEvents.ResetPlayerHealth -= ResumeMovement;
    }

    private void ResetPosition()
    {
        transform.position = _initialPosition;
    }

    private void StopMovement()
    {
        _currentSpeed = 0;
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private void ResumeMovement()
    {
        // _currentSpeed = speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if grounded by detecting collision with ground layers
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}