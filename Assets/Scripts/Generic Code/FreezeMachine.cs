using System.Collections;
using Mario;
using UnityEngine;

public class FreezeMachine : MonoBehaviour
{
    [SerializeField] private string frozenLayerName = "Frozen";
    [SerializeField] private GameObject freezeEffect;
    private int _originalLayer;

    private Animator _animator;
    private EntityMovement _entityMovement;
    private Rigidbody2D _rigidbody2D;
    private MarioMovementControl _marioMovementControl;

    private bool _isFrozen;
    private GameObject _freezeEffect;

    private void Awake()
    {
        // Cache references to components
        _animator = GetComponent<Animator>();
        _entityMovement = GetComponent<EntityMovement>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _marioMovementControl = GetComponent<MarioMovementControl>();
        
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
        if (_animator != null && _marioMovementControl == null)
        {
            _animator.speed = 1f; // Resume animations
        }

        if (_marioMovementControl != null)
        {
            _marioMovementControl.enabled = true;
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
        if (_animator != null && _marioMovementControl == null)
        {
            _animator.speed = 0f; // Pause animations
        }
        
        if (_marioMovementControl != null)
        {
            _marioMovementControl.enabled = false;
        }
        
        // Stop physics movement
        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic; // Freeze physics
        }
    }
}
