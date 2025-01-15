using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D), typeof(MarioMoveController), typeof(Animator))]
public class MarioStateMachine : MonoBehaviour
{
    private static readonly int OnCrouch = Animator.StringToHash("OnCrouch");
    private static readonly int Attack = Animator.StringToHash("Fire");
    [SerializeField] public float starDuration = 10f;
    [SerializeField] public float untouchableDuration = 2.1f;
    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;


    // private MarioMovementControl _movementController;

    internal MarioMoveController MovementController { get; private set; }
    internal PlayerInputActions PlayerInputActions { get; private set; }
    internal Animator Animator { get; private set; }


    // internal MarioAnimationController MarioAnimationController { get; private set; }

    private IMarioState _currentState;
    internal readonly BigMarioState BigMarioState = new BigMarioState();
    internal SmallMarioState SmallMarioState = new SmallMarioState();
    internal readonly FireMarioState FireMarioState = new FireMarioState();
    internal readonly StarMarioState StarMarioState = new StarMarioState();
    public FlashTransparency flashTransparency;

    [FormerlySerializedAs("StarPowerEffect")]
    public StarPowerEffect starPowerEffect;

    private FreezeMachine _freezeMachine;

    [FormerlySerializedAs("IsUntouchable")]
    public bool isUntouchable;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        // MarioAnimationController = GetComponent<MarioAnimationController>();
        Animator = GetComponent<Animator>();
        MovementController = GetComponent<MarioMoveController>();
        // get the son of Mario
        flashTransparency = GetComponentInChildren<FlashTransparency>();
        starPowerEffect = GetComponentInChildren<StarPowerEffect>();
        _freezeMachine = GetComponent<FreezeMachine>();

        // flashTransparency = GetComponent<FlashTransparency>();

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

        PlayerInputActions.Enable();
        PlayerInputActions.Player.Crouch.started += Crouch;
        PlayerInputActions.Player.Crouch.canceled += StandUp;
        PlayerInputActions.Player.Attack.performed += ShootFireball;
        MarioEvents.OnMarioGotPowerUp += OnPickUpPowerUp; // Assuming you have this input
        GameEvents.FreezeAllCharacters += FreezeMario;
    }

    private void OnDisable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _capsuleCollider.enabled = false;

        MarioEvents.OnMarioGotPowerUp -= OnPickUpPowerUp;
        PlayerInputActions.Player.Crouch.started -= Crouch;
        PlayerInputActions.Player.Crouch.canceled -= StandUp;
        PlayerInputActions.Player.Attack.performed -= ShootFireball;
        PlayerInputActions.Player.Disable();


        StopAllCoroutines();
        gameObject.layer = LayerMask.NameToLayer("Mario");
        GameEvents.FreezeAllCharacters -= FreezeMario;
    }

    private void Update()
    {
        // Update current state
        _currentState?.DoAction(this);
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (_currentState is SmallMarioState)
        {
            return;
        }

        Debug.Log("Crouching");
        SetColliderSize(new Vector2(0.75f, 1f), Vector2.zero);
        MovementController.enabled = false;
        Animator.SetBool(OnCrouch, true);
    }

    private void StandUp(InputAction.CallbackContext context)
    {
        if (_currentState is SmallMarioState)
        {
            return;
        }

        Debug.Log("Standing up");
        MovementController.enabled = true;
        SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
        Animator.SetBool(OnCrouch, false);
    }


    private void FixedUpdate()
    {
        // Movement is handled by MovementController
    }


    private void OnPickUpPowerUp(PowerUpType powerUpType)
    {
        // get power-up type from context and change state accordingly
        // e.g., if MarioEvent.PowerUpType == PowerUpType.FireFlower, ChangeState(new FireMarioState());
        _currentState.OnPickUpPowerUp(this, powerUpType);
        if (powerUpType == PowerUpType.Star)
        {
            starPowerEffect.StartStarPower();
            ChangeState(StarMarioState);
            StartCoroutine(OnStarPower());
        }
    }

    private IEnumerator OnStarPower()
    {
        yield return new WaitForSeconds(starDuration);
        starPowerEffect.StopStarPower();
        ChangeState(SmallMarioState);
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

    private void OnCollisionEnter2D(Collision2D other)
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
        // StartCoroutine(UntouchableDuration(untouchableDuration));
    }

    public void SetColliderSize(Vector2 size, Vector2 offset)
    {
        // _capsuleCollider.enabled = false;
        _capsuleCollider.size = size;
        _capsuleCollider.offset = offset;
        // _capsuleCollider.enabled = true;
    }

    public void StopFlashing()
    {
        flashTransparency?.StopFlashing();
    }

    public void ShootFireball(InputAction.CallbackContext context)
    {
        if (_currentState is FireMarioState)
        {
            Debug.Log("Shooting fireball");
            Animator.SetTrigger(Attack);
            // Shoot fireball
            // Instantiate fireball prefab
            // Set fireball position to Mario's position
            // Set fireball velocity to Mario's velocity
            // Set fireball direction to Mario's direction
        }
    }

    internal IEnumerator UntouchableDuration(float duration)
    {
        isUntouchable = true;
        gameObject.layer = LayerMask.NameToLayer("PowerUp");
        yield return new WaitForSeconds(duration);
        gameObject.layer = LayerMask.NameToLayer("Mario");
        isUntouchable = false;
    }

    public void FreezeMario(float duration)
    {
        _freezeMachine.Freeze(duration);
    }
}