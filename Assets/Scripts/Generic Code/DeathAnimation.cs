// using System;
// using System.Collections;
// using UnityEngine;
//
// public class DeathAnimation : MonoBehaviour
// {
//     private static readonly int Dead = Animator.StringToHash("Die");
//     [SerializeField] private float duration = 3f;
//     [SerializeField] private float jumpVelocity = 10f;
//     [SerializeField] private float gravity = -36f;
//     
//     private SpriteRenderer _spriteRenderer;
//     private Coroutine _deathAnimationCoroutine;
//     private int _originalSortingOrder;
//     private Animator _animator;
//
//     private void Awake()
//     {
//         _animator = GetComponent<Animator>();
//         _spriteRenderer = GetComponent<SpriteRenderer>();
//         _originalSortingOrder = _spriteRenderer.sortingOrder;
//     }
//
//     private void Reset()
//     {
//         _spriteRenderer = GetComponent<SpriteRenderer>();
//         _animator = GetComponent<Animator>();
//     }
//
//     // private void OnEnable()
//     // {
//     //     UpdateAnimation();
//     //     DisableEntityPhysics();
//     //     StartCoroutine(AnimateDeath());
//     // }
//     //
//     // private void OnDisable()
//     // {
//     //     _spriteRenderer.sortingOrder = _originalSortingOrder;
//     //     StopAllCoroutines();
//     //     ActivateEntityPhysics();
//     // }
//     
//     public void TriggerDeathAnimation()
//     {
//         if (_deathAnimationCoroutine != null)
//         {
//             StopCoroutine(_deathAnimationCoroutine);
//         }
//         _deathAnimationCoroutine = StartCoroutine(AnimateDeath());
//     }
//
//
//     private void UpdateAnimation()
//     {
//         _spriteRenderer.enabled = true;
//         _spriteRenderer.sortingOrder = 10;
//         if (_animator != null)
//         {
//             _animator.SetBool(Dead, true);
//         }
//     }
//
//     private void DisableEntityPhysics()
//     {
//         var colliders = GetComponents<Collider2D>();
//         foreach (var colliderObj in colliders)
//         {
//             colliderObj.enabled = false;
//         }
//
//         if (TryGetComponent(out Rigidbody2D rb))
//         {
//             rb.bodyType = RigidbodyType2D.Kinematic;
//         }
//
//         if (TryGetComponent(out PlayerMovement playerMovement))
//         {
//             playerMovement.enabled = false;
//         }
//
//         if (TryGetComponent(out EntityMovement entityMovement))
//         {
//             entityMovement.enabled = false;
//         }
//     }
//
//     private void ActivateEntityPhysics()
//     {
//         var colliders = GetComponents<Collider2D>();
//         foreach (var colliderObj in colliders)
//         {
//             colliderObj.enabled = true;
//         }
//
//         if (TryGetComponent(out Rigidbody2D rb))
//         {
//             rb.bodyType = RigidbodyType2D.Dynamic;
//         }
//
//         if (TryGetComponent(out PlayerMovement playerMovement))
//         {
//             playerMovement.enabled = true;
//         }
//
//         if (TryGetComponent(out EntityMovement entityMovement))
//         {
//             entityMovement.enabled = true;
//         }
//     }
//
//     private IEnumerator AnimateDeath()
//     {
//         var elapsed = 0f;
//         // const float duration = 3f;
//         // const float jumpVelocity = 10f;
//         // const float gravity = -36f;
//
//         var velocity = Vector3.up * jumpVelocity;
//
//         while (elapsed < duration)
//         {
//             transform.position += velocity * Time.deltaTime;
//             velocity.y += gravity * Time.deltaTime;
//             elapsed += Time.deltaTime;
//             yield return null;
//         }
//     }
// }

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
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

    private void Awake()
    {
        // If Rigidbody is not assigned in the Inspector, try to get it from the GameObject
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    
    public void TriggerDeathAnimation()
    {
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

        if (TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.enabled = false;
        }

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