using System;
using System.Collections;
using UnityEngine;

public enum FlipBehavior
{
    AlwaysFlip,
    NeverFlip,
    ConditionalFlip
}

public class EntityMovement : MonoBehaviour
{
    public const float InitialMovementSpeed = 2f;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = InitialMovementSpeed;
    [SerializeField] private Vector2 movementDirection = Vector2.left;
    [SerializeField] private LayerMask layerMask;

    [Header("Sprite Settings")]
    [SerializeField] private FlipBehavior flipBehavior = FlipBehavior.AlwaysFlip;

    [Header("Detection Settings")]
    [SerializeField] private float obstacleDetectionDistance = 0.55f;
    [SerializeField] private float groundDetectionDistance = 0.55f;


    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private FreezeMachine _freezeMachine;
    // private Animator _animator; // Uncomment if using animations

    private void OnEnable()
    {
        GameEvents.FreezeAllCharacters += FreezeMovement;
    }
    
    private void OnDisable()
    {
        GameEvents.FreezeAllCharacters -= FreezeMovement;
    }

    public Vector2 MovementDirection
    {
        get => movementDirection;
        set
        {
            if (movementDirection != value)
            {
                movementDirection = value;
                FlipSprite(); // Flip when direction changes
            }
        }
    }

    public float MovementSpeed
    {
        get => movementSpeed;
        set => movementSpeed = value;
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _freezeMachine = GetComponent<FreezeMachine>();
        // _animator = GetComponent<Animator>(); // Uncomment if using animations

        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing from the GameObject.");
        }

        // Normalize movementDirection to ensure consistent speed
        movementDirection = movementDirection.normalized;
    }

    private void FixedUpdate()
    {
        // Set horizontal velocity based on movement direction and speed
        Vector2 currentVelocity = _rigidbody2D.linearVelocity;
        currentVelocity.x = movementDirection.x * movementSpeed;
        _rigidbody2D.linearVelocity = currentVelocity;

        // Debug ray for obstacle detection
        Debug.DrawRay(_rigidbody2D.position, movementDirection * obstacleDetectionDistance, Color.red);

        // Check for obstacles ahead
        if (IsObstacleAhead())
        {
            // Reverse movement direction upon collision
            MovementDirection = -MovementDirection.normalized;
        }

        // Check for ground below
        // if (IsGrounded())
        // {
            // Optionally, reset vertical velocity if needed
            // currentVelocity.y = Mathf.Max(currentVelocity.y, 0f);
            // _rigidbody2D.velocity = currentVelocity;
        // }
    }

    private bool IsObstacleAhead()
    {
        Vector2 origin = _rigidbody2D.position;

        // Perform a raycast to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(origin, movementDirection, obstacleDetectionDistance, layerMask);

        // Optionally, handle same layer hits or specific tags

        // Return true if any obstacle is detected
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collisions with other enemies or objects based on tags
        if (collision.gameObject.CompareTag("Goomba") && collision.gameObject != gameObject)
        {
            // Reverse direction
            MovementDirection = -MovementDirection.normalized;
        }
        else if (collision.gameObject.CompareTag("Koopa"))
        {
            // Kill the player or handle accordingly
            // GameEvents.OnPlayerDeath?.Invoke();
            MovementDirection = -MovementDirection.normalized;
        }
    }

    // private bool IsGrounded()
    // {
    //     Vector2 origin = _rigidbody2D.position + Vector2.down * 0.1f; // Slightly below the entity
    //
    //     // Perform a raycast to detect ground directly below
    //     RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundDetectionDistance, layerMask);
    //
    //     // Optionally, visualize the ground detection ray
    //     Debug.DrawRay(origin, Vector2.down * groundDetectionDistance, Color.green);
    //
    //     // Return true if ground is detected
    //     return hit.collider != null;
    // }

    private void FlipSprite()
    {
        if (_spriteRenderer == null)
            return;

        switch (flipBehavior)
        {
            case FlipBehavior.AlwaysFlip:
                _spriteRenderer.flipX = movementDirection.x > 0;
                break;

            case FlipBehavior.NeverFlip:
                // Do nothing; keep the sprite as is
                break;

            case FlipBehavior.ConditionalFlip:
                // Implement custom conditions here
                // Example: Flip only when moving right
                _spriteRenderer.flipX = movementDirection.x > 0;
                break;
        }
    }
    
    public void FreezeMovement(float duration)
    {
        if (_freezeMachine != null)
        {
            _freezeMachine.Freeze(duration);
        }
    }
}