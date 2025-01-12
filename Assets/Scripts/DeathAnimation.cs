using System;
using System.Collections;
using UnityEngine;

public class DeathAnimation : MonoBehaviour
{
    private static readonly int Dead = Animator.StringToHash("Die");
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        UpdateAnimation();
        DisableEntityPhysics();
        // AnimateDeath();
        StartCoroutine(AnimateDeath());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void UpdateAnimation()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sortingOrder = 10;
        if (_animator == null) return;
        _animator.SetBool(Dead, true);
    }

    private void DisableEntityPhysics()
    {
        var colliders = GetComponents<Collider2D>();

        foreach (var colliderObj in colliders)
        {
            colliderObj.enabled = false;
        }

        if (TryGetComponent(out Rigidbody2D component)) {
            component.bodyType = RigidbodyType2D.Kinematic;
        }

        if (TryGetComponent(out PlayerMovement playerMovement)) {
            playerMovement.enabled = false;
        }

        if (TryGetComponent(out EntityMovement entityMovement)) {
            entityMovement.enabled = false;
        }
    }

    private IEnumerator AnimateDeath()
    {
        var elapsed = 0f;
        const float duration = 3f;

        const float jumpVelocity = 10f;
        const float gravity = -36f;

        var velocity = Vector3.up * jumpVelocity;

        while (elapsed < duration)
        {
            transform.position += velocity * Time.deltaTime;
            velocity.y += gravity * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}