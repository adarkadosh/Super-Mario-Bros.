using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class FreezeMachine : MonoBehaviour
{
    // [Header("Freeze Settings")]
    // [Tooltip("Duration of the freeze in seconds.")]
    // [SerializeField] private float freezeDuration = 2f;
    [SerializeField] private GameObject freezeEffect;
    private int _originalLayer;
    [SerializeField] private string frozenLayerName = "Frozen";

    private Animator _animator;
    private EntityMovement _entityMovement;
    private Rigidbody2D _rigidbody2D;
    private MarioMoveController _marioMoveController;

    private bool _isFrozen;
    private GameObject _freezeEffect;

    private void Awake()
    {
        // Cache references to components
        _animator = GetComponent<Animator>();
        _entityMovement = GetComponent<EntityMovement>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _marioMoveController = GetComponent<MarioMoveController>();
        
        // Store the original layer at startup
        _originalLayer = gameObject.layer;
    }

    public void Freeze(float duration)
    {
        if (_isFrozen) return; // Prevent multiple freeze coroutines
        StartCoroutine(FreezeRoutine(duration));                
    }
    
    public void FreezeAttack(float duration)
    {
        if (_isFrozen) return; // Prevent multiple freeze coroutines
        StartCoroutine(AttackFreezeRoutine(duration));                
    }

    private void OnDisable()
    {
        // Ensure the object is restored to its original state if disabled mid-freeze
        gameObject.layer = _originalLayer;
        StopAllCoroutines();
        EnablePhysics();
    }

    /// <summary>
    /// Coroutine that handles freezing and unfreezing.
    /// </summary>
    /// <param name="duration">Duration of the freeze in seconds.</param>
    private IEnumerator FreezeRoutine(float duration)
    {
        DisablePhysics();
        yield return new WaitForSeconds(duration);
        EnablePhysics();
    }

    private IEnumerator AttackFreezeRoutine(float duration)
    {
        _freezeEffect = Instantiate(freezeEffect, transform.position, Quaternion.identity, transform);
        DisablePhysics();
        yield return new WaitForSeconds(duration);
        EnablePhysics();
        Destroy(_freezeEffect);
    }

    private void EnablePhysics()
    {
        // Restore original layer
        gameObject.layer = _originalLayer;

        // Resume movement
        if (_entityMovement != null)
        {
            _entityMovement.enabled = true;
        }

        // Resume animations
        if (_animator != null && _marioMoveController == null)
        {
            _animator.speed = 1f; // Resume animations
        }

        if (_marioMoveController != null)
        {
            _marioMoveController.enabled = true;
        }

        // Resume physics movement
        if (_rigidbody2D != null)
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic; // Re-enable physics
        }

        _isFrozen = false;
    }

    private void DisablePhysics()
    {
        _isFrozen = true;
        
        // Store the original layer
        _originalLayer = gameObject.layer;
        
        // Change the layer to prevent collision with Mario
        gameObject.layer = LayerMask.NameToLayer(frozenLayerName);
        
        // Stop movement
        if (_entityMovement != null)
        {
            _entityMovement.enabled = false;
        }
        
        // Stop animations only for the Enemy
        if (_animator != null && _marioMoveController == null)
        {
            _animator.speed = 0f; // Pause animations
        }
        
        if (_marioMoveController != null)
        {
            _marioMoveController.enabled = false;
        }
        
        // Stop physics movement
        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic; // Freeze physics
        }
    }
}
