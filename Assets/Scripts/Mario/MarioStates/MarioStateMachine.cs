using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D), typeof(MarioAnimationController), typeof(MovementController))]
public class MarioStateMachine : MonoBehaviour
{
    [SerializeField] public float starDuration = 10f;
    private Rigidbody2D _rigidbody;
    private Collider2D _capsuleCollider;


    // private MarioMovementControl _movementController;

    internal MarioMovementControl MovementController { get; private set; }
    internal PlayerInputActions PlayerInputActions { get; private set; }
    internal Animator Animator { get; private set; }


    // internal MarioAnimationController MarioAnimationController { get; private set; }

    private IMarioState _currentState;
    internal readonly BigMarioState BigMarioState = new BigMarioState();
    internal SmallMarioState SmallMarioState = new SmallMarioState();
    internal readonly FireMarioState FireMarioState = new FireMarioState();
    internal readonly StarMarioState StarMarioState = new StarMarioState();


    private void Awake()
    {
        
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<Collider2D>();
        // MarioAnimationController = GetComponent<MarioAnimationController>();
        Animator = GetComponent<Animator>();
        MovementController = GetComponent<MarioMovementControl>();

        PlayerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // Initialize to Small Mario
        ChangeState(new SmallMarioState());
    }

    private void OnEnable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _capsuleCollider.enabled = true;

        PlayerInputActions.Player.Enable();
        MarioEvents.OnMarioGotPowerUp += OnPickUpPowerUp; // Assuming you have this input
    }

    private void OnDisable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _capsuleCollider.enabled = false;

        MarioEvents.OnMarioGotPowerUp -= OnPickUpPowerUp;
        PlayerInputActions.Player.Disable();

        // Reset animation parameters
        // if (MarioAnimationController != null)
        // {
            // MarioAnimationController.ResetAnimations();
        // }
    }

    private void Update()
    {
        // Update current state
        _currentState?.DoAction(this);
    }

    private void FixedUpdate()
    {
        // Movement is handled by MovementController
    }


    private void OnPickUpPowerUp(PowerUpType powerUpType)
    {
        // get power-up type from context and change state accordingly
        // e.g., if MarioEvent.PowerUpType == PowerUpType.FireFlower, ChangeState(new FireMarioState());
        if (powerUpType == PowerUpType.FireFlower)
        {
            ChangeState(FireMarioState);
        } else if (powerUpType == PowerUpType.SuperMashroom)
        {
            ChangeState(BigMarioState);
        } else if (powerUpType == PowerUpType.Star)
        {
            ChangeState(StarMarioState);
        } else
        {
            Debug.LogWarning("Invalid power-up type.");
        }
        
    }

    public void ChangeState(IMarioState newState)
    {
        _currentState = newState;
        _currentState.EnterState(this);

        // Invoke corresponding event based on new state type
        switch (newState)
        {
            case SmallMarioState _:
                MarioEvents.OnEnterSmallMario?.Invoke();
                break;
            case BigMarioState _:
                MarioEvents.OnEnterBigMario?.Invoke();
                break;
            case FireMarioState _:
                MarioEvents.OnEnterFireMario?.Invoke();
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        _currentState.OnCollisionEnter2D(this, other);
        Vector2 direction = other.transform.position - transform.position;
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Bounce off enemy head
            if (Vector2.Dot(direction.normalized, Vector2.down) < 0.25f)
            {
                GotHit();
                MarioEvents.OnMarioGotHit?.Invoke();
            }
        }
    }

    private void GotHit()
    {
        _currentState.GotHit(this);
    }

    public void SetColliderSize(Vector2 size, Vector2 offset)
    {
        _capsuleCollider.transform.localScale = size;
        _capsuleCollider.offset = offset;
    }

    public void ShootFireball()
    {
        // Instantiate fireball prefab
        // Set fireball position to Mario's position
        // Set fireball velocity to Mario's velocity
        // Set fireball direction to Mario's direction
    }
}