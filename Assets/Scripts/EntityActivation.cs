using UnityEngine;

public class EntityActivation : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null) Debug.LogError("Rigidbody2D is missing on " + gameObject.name);

        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogError("Animator is missing on " + gameObject.name);

        enabled = false; // Start disabled until visible
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void OnEnable()
    {
        _rigidbody2D.WakeUp();
    }

    private void OnDisable()
    {
        _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
        _rigidbody2D.Sleep();
    }
}
