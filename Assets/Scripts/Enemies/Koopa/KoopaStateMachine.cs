using System;
using Enemies.Koopa;
using UnityEngine;

public class KoopaStateMachine : EnemyBehavior, IPoolable
{
    [SerializeField] private float shellDuration = 3f;
    [SerializeField] private float backToLifeTime = 2f;
    [SerializeField] private float shellSpeed = 12f;
    
    public float ShellDuration => shellDuration;
    public float BackToLifeTime => backToLifeTime;
    public float ShellSpeed => shellSpeed;
    
    
    private IKoopaState _currentState;
    internal readonly WalkingState WalkingState = new WalkingState();
    internal readonly ShellState ShellState = new ShellState();

    void Start()
    {
        _currentState = WalkingState;
        _currentState.EnterState(this);
    }

    void Update()
    {
        _currentState.UpdateState(this);
    }

    internal void ChangeState(IKoopaState newState)
    {
        _currentState.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        _currentState.OnTriggerEnter2D(this, collision);
    }

    protected override void DeathSequenceAnimation()
    {
        ChangeState(ShellState);
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Death";
        spriteRenderer.flipY = true;
    }

    protected override void GotHit()
    {
        _currentState.GotHit(this);
    }

    public void Reset()
    {
        if (_currentState == ShellState)
        {
            _currentState.ExitState(this);
        }
        _currentState = WalkingState;
        _currentState.EnterState(this);
        
        
        var rb = GetComponent<Rigidbody2D>();
        var entityMovement = GetComponent<EntityMovement>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        // GetComponent<DeathAnimation>().enabled = false;
        var colliders = GetComponents<Collider2D>();
        foreach (var colliderObj in colliders)
        {
            colliderObj.enabled = true;
        }
        Animator.enabled = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero; // Reset velocity
        rb.angularVelocity = 0f;    // Reset angular velocity
        // rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Allow vertical movement, but prevent rotation

        entityMovement.enabled = true;
        entityMovement.MovementDirection = Vector2.left; // Reset movement direction
        spriteRenderer.flipY = false; // Reset sprite flip
        spriteRenderer.sortingLayerName = "Enemies"; // Reset sorting layer
    }
}