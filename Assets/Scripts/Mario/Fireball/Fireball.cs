using System;
using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour, IPoolable
{
    public float speed = 10f;
    public float lifetime = 3f;
    public Vector2 direction = new Vector2(-0.5f, -0.5f);
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private EntityMovement _entityMovement;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _entityMovement = GetComponent<EntityMovement>();
    }

    void Start()
    {
        // Move the fireball in the direction it's facing
        // _rigidbody.linearVelocity = direction * speed;
        // Destroy the fireball after 'lifetime' seconds
        // StartCoroutine(WaitAndDestroy(lifetime));
    }

    private IEnumerator WaitAndDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _animator.SetTrigger("Explode");
    }

    private void OnEnable()
    {
        StartCoroutine(WaitAndDestroy(lifetime));

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the fireball collides with an enemy layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Apply damage to the enemy
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                StartCoroutine(enemy.DeathSequence());
            }
            _animator.SetTrigger("Explode");
        }
    }

    public void Reset()
    {
        if (_rigidbody != null)
        {
            // Reset velocity
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }

        // Reset position and rotation
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // Reset animator to default state
        if (_animator != null)
        {
            _animator.ResetTrigger("Explode");
            _animator.Play("Idle", -1, 0f); // Replace "IdleState" with your default state
        }

        // Reset collider
        if (_collider != null)
        {
            _collider.enabled = true;
        }

        // Stop any ongoing coroutines
        StopAllCoroutines();
    }

    public void Kill()
    {
        FiraballPool.Instance.Return(this);
    }
}