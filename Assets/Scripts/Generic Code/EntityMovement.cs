// // using UnityEngine;
// //
// //
// // public class EntityMovement : MonoBehaviour
// // {
// //     // TODO: Make them flip when they hit a (if nessesary) -> GetComponent<SpriteRenderer>().flipX = true;
// //
// //     [SerializeField] private float movementSpeed = 2f;
// //     [SerializeField] private Vector2 movementDirection = Vector2.left;
// //     [SerializeField] private LayerMask layerMask;
// //     // [SerializeField] private LayerMask sameLayerMask;
// //     // private Animator _animator;
// //     private Rigidbody2D _rigidbody2D;
// //     private Vector2 _velocity;
// //     
// //     public Vector2 MovementDirection
// //     {
// //         get => movementDirection;
// //         set => movementDirection = value;
// //     }
// //     public float MovementSpeed
// //     {
// //         get => movementSpeed;
// //         set => movementSpeed = value;
// //     }
// //
// //     private void Awake()
// //     {
// //         _rigidbody2D = GetComponent<Rigidbody2D>();
// //         // _animator = GetComponent<Animator>();
// //         // enabled = false;
// //     }
// //
// //     // private void OnBecameVisible()
// //     // {
// //     //     enabled = true;
// //     //     // show me the 
// //     // }
// //     //
// //     // private void OnBecameInvisible()
// //     // {
// //     //     enabled = false;
// //     // }
// //     //
// //     // private void OnEnable()
// //     // {
// //     // _rigidbody2D.WakeUp();
// //     // // GetOrthographicCameraBounds();
// //     // }
// //     //
// //     // private void OnDisable()
// //     // {
// //     // _rigidbody2D.linearVelocity = Vector2.zero;
// //     // _rigidbody2D.Sleep();
// //     // }
// //     
// //     private void FixedUpdate()
// //     {
// //         // Normalize movement direction to ensure consistent behavior
// //         // movementDirection = movementDirection.normalized;
// //
// //         // Update horizontal velocity based on movement direction and speed
// //         _velocity.x = movementDirection.x * movementSpeed;
// //
// //         // Apply gravity to vertical velocity
// //         _velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
// //
// //         // Move the Goomba
// //         _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * Time.fixedDeltaTime);
// //         Debug.DrawRay(_rigidbody2D.position, movementDirection * 0.55f, Color.red);
// //
// //         // Check for obstacles ahead
// //         if (IsObstacleAhead())
// //         {
// //             // Reverse movement direction upon collision
// //             movementDirection = -movementDirection.normalized;
// //         }
// //
// //         // Check for ground below
// //         if (IsGrounded())
// //         {
// //             // Prevent downward acceleration when grounded
// //             _velocity.y = 0f;
// //         }
// //     }
// //
// //     private bool IsObstacleAhead()
// //     {
// //         Vector2 origin = _rigidbody2D.position;
// //
// //         // Perform a raycast to detect obstacles
// //         RaycastHit2D hit = Physics2D.Raycast(origin, movementDirection, 0.55f, layerMask);
// //
// //         // Check for entities on the same layer
// //         // RaycastHit2D sameLayerHit = Physics2D.Raycast(origin, movementDirection, 0.55f);
// //         // if (sameLayerHit.collider != null && sameLayerHit.collider.gameObject != gameObject 
// //         //                                   && sameLayerHit.collider.CompareTag("Goomba"))
// //         // {
// //         //     Debug.Log("Same Layer Hit: " + sameLayerHit.collider.name);
// //         //     return true;
// //         // }
// //
// //         // Return true if any obstacle is detected
// //         return hit.collider != null;
// //     }
// //     
// //     private void OnCollisionEnter2D(Collision2D collision)
// //     {
// //         // If we collided with another enemy on the same layer (or by tag)
// //         if (collision.gameObject.CompareTag("Goomba") && collision.gameObject != gameObject)
// //         {
// //             // Reverse direction
// //             movementDirection = -movementDirection.normalized;
// //         } else if (collision.gameObject.CompareTag("Koopa"))
// //         {
// //             // Kill the player
// //             // GameEvents.OnPlayerDeath?.Invoke();
// //             movementDirection = -movementDirection.normalized;
// //         } 
// //          
// //     }
// //
// //     private bool IsGrounded()
// //     {
// //         // Define the origin point slightly below the Goomba's center
// //         Vector2 origin = _rigidbody2D.position + Vector2.down * 0.1f;
// //
// //         // Perform a raycast to detect ground directly below
// //         RaycastHit2D hit = Physics2D.Raycast(_rigidbody2D.position, Vector2.down, 0.55f, layerMask);
// //         
// //
// //         // Return true if ground is detected
// //         return hit;
// //     }
// // }
//
// using System;
// using UnityEngine;
//
// public enum FlipBehavior
// {
//     AlwaysFlip,
//     NeverFlip,
//     ConditionalFlip
// }
//
// public class EntityMovement : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     [SerializeField] private float movementSpeed = 2f;
//     [SerializeField] private Vector2 movementDirection = Vector2.left;
//     [SerializeField] private LayerMask layerMask;
//     
//     [Header("Sprite Settings")]
//     [SerializeField] private FlipBehavior flipBehavior = FlipBehavior.AlwaysFlip; // Using enum
//
//     private Rigidbody2D _rigidbody2D;
//     private Vector2 _velocity;
//     private SpriteRenderer _spriteRenderer; // Cached reference
//
//     public Vector2 MovementDirection
//     {
//         get => movementDirection;
//         set
//         {
//             if (movementDirection != value)
//             {
//                 movementDirection = value;
//                 FlipSprite(); // Flip when direction changes
//             }
//         }
//     }
//
//     public float MovementSpeed
//     {
//         get => movementSpeed;
//         set => movementSpeed = value;
//     }
//
//     private void Awake()
//     {
//         _rigidbody2D = GetComponent<Rigidbody2D>();
//         _spriteRenderer = GetComponent<SpriteRenderer>(); // Cache SpriteRenderer
//         if (_spriteRenderer == null)
//         {
//             Debug.LogError("SpriteRenderer component missing from the GameObject.");
//         }
//
//         // Normalize movementDirection to ensure consistent speed
//         movementDirection = movementDirection.normalized;
//     }
//
//     private void FixedUpdate()
//     {
//         // Update horizontal velocity based on movement direction and speed
//         _velocity.x = movementDirection.x * movementSpeed;
//
//         // Apply gravity to vertical velocity
//         _velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
//
//         // Move the entity
//         // _rigidbody2D.linearVelocity = _velocity;
//         _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * Time.fixedDeltaTime);
//         Debug.DrawRay(_rigidbody2D.position, movementDirection * 0.55f, Color.red);
//
//         // Check for obstacles ahead
//         if (IsObstacleAhead())
//         {
//             // Reverse movement direction upon collision
//             MovementDirection = -MovementDirection.normalized;
//         }
//
//         // Check for ground below
//         if (IsGrounded())
//         {
//             // Prevent downward acceleration when grounded
//             _velocity.y = 0f;
//         }
//     }
//
//     private bool IsObstacleAhead()
//     {
//         Vector2 origin = _rigidbody2D.position;
//
//         // Perform a raycast to detect obstacles
//         RaycastHit2D hit = Physics2D.Raycast(origin, movementDirection, 0.55f, layerMask);
//
//         // Return true if any obstacle is detected
//         return hit.collider != null;
//     }
//
//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         // If we collided with another enemy on the same layer (or by tag)
//         if (collision.gameObject.CompareTag("Goomba") && collision.gameObject != gameObject)
//         {
//             // Reverse direction
//             MovementDirection = -MovementDirection.normalized;
//         }
//         else if (collision.gameObject.CompareTag("Koopa"))
//         {
//             // Kill the player
//             // GameEvents.OnPlayerDeath?.Invoke();
//             MovementDirection = -MovementDirection.normalized;
//         }
//     }
//
//     private bool IsGrounded()
//     {
//         // Perform a raycast to detect ground directly below
//         RaycastHit2D hit = Physics2D.Raycast(_rigidbody2D.position, Vector2.down, 0.55f, layerMask);
//
//         // Return true if ground is detected
//         return hit.collider != null;
//     }
//
//     private void FlipSprite()
//     {
//         if (_spriteRenderer == null)
//             return;
//
//         switch (flipBehavior)
//         {
//             case FlipBehavior.AlwaysFlip:
//                 _spriteRenderer.flipX = movementDirection.x > 0;
//                 break;
//
//             case FlipBehavior.NeverFlip:
//                 // Do nothing; keep the sprite as is
//                 break;
//
//             case FlipBehavior.ConditionalFlip:
//                 // I Can implement custom conditions here
//                 // for example: Flip only when moving right
//                 _spriteRenderer.flipX = movementDirection.x > 0;
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
// }

using UnityEngine;

public enum FlipBehavior
{
    AlwaysFlip,
    NeverFlip,
    ConditionalFlip
}

public class EntityMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private Vector2 movementDirection = Vector2.left;
    [SerializeField] private LayerMask layerMask;

    [Header("Sprite Settings")]
    [SerializeField] private FlipBehavior flipBehavior = FlipBehavior.AlwaysFlip;

    [Header("Detection Settings")]
    [SerializeField] private float obstacleDetectionDistance = 0.55f;
    [SerializeField] private float groundDetectionDistance = 0.55f;


    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    // private Animator _animator; // Uncomment if using animations

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
}