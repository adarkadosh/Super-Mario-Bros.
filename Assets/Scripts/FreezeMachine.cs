using System.Collections;
using UnityEngine;

public class FreezeMachine : MonoBehaviour
{
    // [Header("Freeze Settings")]
    // [Tooltip("Duration of the freeze in seconds.")]
    // [SerializeField] private float freezeDuration = 2f;

    private Animator _animator;
    private EntityMovement _entityMovement;
    private Rigidbody2D _rigidbody2D;
    private MarioMoveController _marioMoveController;

    private bool _isFrozen;

    private void Awake()
    {
        // Cache references to components
        _animator = GetComponent<Animator>();
        _entityMovement = GetComponent<EntityMovement>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _marioMoveController = GetComponent<MarioMoveController>();

        // if (_animator == null)
        //     Debug.LogWarning($"{gameObject.name} is missing an Animator component.");
        //
        // if (_entityMovement == null)
        //     Debug.LogWarning($"{gameObject.name} is missing an EntityMovement component.");
        //
        // if (_rigidbody2D == null)
        //     Debug.LogWarning($"{gameObject.name} is missing a Rigidbody2D component.");
    }

    /// <summary>
    /// Public method to trigger the freeze.
    /// </summary>
    /// <param name="duration">Optional duration to override the default.</param>
    public void Freeze(float duration)
    {
        if (_isFrozen)
            return; // Prevent multiple freeze coroutines

        StartCoroutine(FreezeRoutine(duration));                
    }

    /// <summary>
    /// Coroutine that handles freezing and unfreezing.
    /// </summary>
    /// <param name="duration">Duration of the freeze in seconds.</param>
    private IEnumerator FreezeRoutine(float duration)
    {
        _isFrozen = true;

        // Stop movement
        if (_entityMovement != null)
        {
            _entityMovement.enabled = false;
        }

        // Stop animations only for the Enemy
        if (_animator != null && _marioMoveController == null)
        {
            _animator.speed = 0f; // Pause animations
            // Alternatively, you can disable the Animator
            // _animator.enabled = false;
        }
        
        if (_marioMoveController != null)
        {
            _marioMoveController.enabled = false;
        }

        // Optionally, stop physics movement
        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            // _rigidbody2D.bodyType = RigidbodyType2D.Kinematic; // Prevent physics from affecting the enemy
        }

        // Visual Feedback (Optional)
        // You can add visual cues here, like changing the sprite color or playing a freeze effect

        yield return new WaitForSeconds(duration);

        // Resume movement
        if (_entityMovement != null)
        {
            _entityMovement.enabled = true;
        }

        // Resume animations
        if (_animator != null && _marioMoveController == null)
        {
            _animator.speed = 1f; // Resume animations
            // If you disabled the Animator, re-enable it here
            // _animator.enabled = true;
        }
        
        if (_marioMoveController != null)
        {
            _marioMoveController.enabled = true;
        }

        // Resume physics movement
        if (_rigidbody2D != null)
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic; // Re-enable physics
            // If needed, you can set velocity to a default value or keep it zero
        }

        _isFrozen = false;
    }
}
