using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DeathAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Total duration of the death animation in seconds.")]
    [SerializeField]
    private float duration = 3f;

    [Tooltip("Initial upward velocity applied to the GameObject.")] [SerializeField]
    private float jumpVelocity = 10f;

    [Tooltip("Downward acceleration applied to the GameObject.")] [SerializeField]
    private float gravity = -36f;
    

    private Rigidbody _rigidbody;

    private Coroutine _deathAnimationCoroutine;
    private Vector3 _velocity;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        // If Rigidbody is not assigned in the Inspector, try to get it from the GameObject
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    
    public void TriggerDeathAnimation(float waitTime = 0f)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingLayerName = "Death";
        }
        StartCoroutine(Extensions.WaitForSeconds(waitTime));
        if (_deathAnimationCoroutine != null)
        {
            StopCoroutine(_deathAnimationCoroutine);
        }

        _deathAnimationCoroutine = StartCoroutine(AnimateDeath());
    }

    private void DisableEntityPhysics()
    {
        var colliders = GetComponents<Collider2D>();
        foreach (var colliderObj in colliders)
        {
            colliderObj.enabled = false;
        }

        if (TryGetComponent(out Rigidbody2D component))
        {
            component.bodyType = RigidbodyType2D.Kinematic;
        }

        // if (TryGetComponent(out PlayerMovement playerMovement))
        // {
            // playerMovement.enabled = false;
        // }

        if (TryGetComponent(out EntityMovement entityMovement))
        {
            entityMovement.enabled = false;
        }
    }


    /// <summary>
    /// Coroutine that handles the death animation by moving the GameObject upward and then letting it fall under gravity.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator AnimateDeath()
    {
        DisableEntityPhysics();
        float elapsed = 0f;
        _velocity = Vector3.up * jumpVelocity;

        while (elapsed < duration)
        {
            // Calculate movement based on velocity
            Vector3 movement = _velocity * Time.deltaTime;

            if (_rigidbody != null)
            {
                // If Rigidbody is present, use MovePosition for smooth physics-based movement
                _rigidbody.MovePosition(transform.position + movement);
            }
            else
            {
                // Otherwise, directly modify the transform's position
                transform.position += movement;
            }

            // Apply gravity to the velocity
            _velocity.y += gravity * Time.deltaTime;

            // Increment elapsed time
            elapsed += Time.deltaTime;

            yield return null;
        }
    }
    
    
}
//
// using UnityEngine;
// using System.Collections;
//
// [RequireComponent(typeof(Collider2D))]
// public class DeathAnimation : MonoBehaviour
// {
//     [Header("Animation Settings")]
//     [Tooltip("Total duration of the death animation in seconds.")]
//     [SerializeField]
//     private float duration = 3f;
//
//     [Tooltip("Initial upward velocity applied to the GameObject.")]
//     [SerializeField]
//     private float jumpVelocity = 10f;
//
//     [Tooltip("Downward acceleration applied to the GameObject.")]
//     [SerializeField]
//     private float gravity = -36f;
//
//     private Rigidbody _rigidbody;
//     private Coroutine _deathAnimationCoroutine;
//     private Vector3 _velocity;
//     private SpriteRenderer _spriteRenderer;
//
//     private bool _isAnimating; // To prevent duplicate triggers
//
//     private void Awake()
//     {
//         // Cache components
//         _rigidbody = GetComponent<Rigidbody>();
//         _spriteRenderer = GetComponent<SpriteRenderer>();
//     }
//
//     public void TriggerDeathAnimation(float waitTime = 0f)
//     {
//         if (_isAnimating) return; // Prevent duplicate triggers
//         _isAnimating = true;
//
//         // Change sorting layer for visibility
//         if (_spriteRenderer != null)
//         {
//             _spriteRenderer.sortingLayerName = "Death";
//         }
//
//         if (_deathAnimationCoroutine != null)
//         {
//             StopCoroutine(_deathAnimationCoroutine);
//         }
//
//         _deathAnimationCoroutine = StartCoroutine(DeathSequence(waitTime));
//     }
//
//     private IEnumerator DeathSequence(float waitTime)
//     {
//         if (waitTime > 0)
//         {
//             yield return new WaitForSeconds(waitTime);
//         }
//
//         if (this == null) yield break; // Ensure object is not destroyed
//
//         DisableEntityPhysics();
//
//         yield return AnimateDeath();
//
//         // Destroy the object after animation completes
//         Destroy(gameObject);
//     }
//
//     private void DisableEntityPhysics()
//     {
//         if (this == null) return; // Ensure object is not destroyed
//
//         // Disable colliders
//         var colliders = GetComponents<Collider2D>();
//         foreach (var colliderObj in colliders)
//         {
//             colliderObj.enabled = false;
//         }
//
//         // Disable Rigidbody2D
//         if (TryGetComponent(out Rigidbody2D component))
//         {
//             component.bodyType = RigidbodyType2D.Kinematic;
//         }
//
//         // Disable movement scripts if any
//         if (TryGetComponent(out EntityMovement entityMovement))
//         {
//             entityMovement.enabled = false;
//         }
//     }
//
//     private IEnumerator AnimateDeath()
//     {
//         float elapsed = 0f;
//         _velocity = Vector3.up * jumpVelocity;
//
//         while (elapsed < duration)
//         {
//             // Check if object still exists
//             if (this == null) yield break;
//
//             // Calculate movement
//             Vector3 movement = _velocity * Time.deltaTime;
//
//             if (_rigidbody != null)
//             {
//                 _rigidbody.MovePosition(transform.position + movement);
//             }
//             else
//             {
//                 transform.position += movement;
//             }
//
//             // Apply gravity
//             _velocity.y += gravity * Time.deltaTime;
//
//             // Increment elapsed time
//             elapsed += Time.deltaTime;
//
//             yield return null;
//         }
//     }
// }