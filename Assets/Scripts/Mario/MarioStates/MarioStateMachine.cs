using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum MarioState
{
    Small,
    Big,
    Fire,
    GrowShrink,
    Attack,
    Star
}


public class MarioStateMachine : MonoBehaviour
{
    private static readonly int OnCrouch = Animator.StringToHash("OnCrouch");

    [Header("Durations")]
    [SerializeField] private float starDuration = 10f;
    [SerializeField] private float starDurationDelay = 5f;
    [SerializeField] private float untouchableDuration = 2.1f;

    [Header("References")]
    [SerializeField] private PaletteSwapper paletteSwapper;
    [SerializeField] private FlashTransparency flashTransparency;

    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;
    private MarioMoveController _movementController;
    private PlayerInputActions _playerInputActions;
    private FreezeMachine _freezeMachine;

    private IMarioState _currentState;
    private Coroutine _starPowerCoroutine;
    private MarioState _currentMarioState; 

    // ----- Public Properties for State Access -----
    public Animator Animator { get; private set; }
    public FlashTransparency FlashTransparency => flashTransparency;
    public PaletteSwapper PaletteSwapper => paletteSwapper;
    public float StarDuration => starDuration;
    public float StarDurationDelay => starDurationDelay;
    public float UntouchableDurationValue => untouchableDuration;
    public bool IsUntouchable { get; private set; }

    private const string MarioLayer = "Mario";
    private const string PowerUpLayer = "PowerUp";
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
        _movementController = GetComponent<MarioMoveController>();
        _freezeMachine = GetComponent<FreezeMachine>();

        // Set up input
        _playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // Start in SmallMarioState (using the factory).
        ChangeState(MarioState.Small);
    }

    private void OnEnable()
    {
        EnablePhysics();
        SubscribeInputActions();
        SubscribeGameEvents();
    }

    private void OnDisable()
    {
        DisablePhysics();
        UnsubscribeInputActions();
        UnsubscribeGameEvents();

        StopAllCoroutines();
        // gameObject.layer = LayerMask.NameToLayer(MarioLayer);
    }

    private void Update()
    {
        // Let the current state handle any per-frame logic.
        _currentState?.Update(this);
    }
    

    public void ChangeState(MarioState newState)
    {
        _currentMarioState = newState;
        // Exit previous state if any
        _currentState?.ExitState(this);

        // Fetch new state instance from our factory
        _currentState = MarioStateFactory.GetState(newState);

        // Enter the new state
        _currentState.EnterState(this);
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _currentState?.OnCollisionEnter2D(this, collision);

        // Example: enemy collision from above triggers a hit

    }


    private void GotHit()
    {
        _currentState?.GotHit(this);
    }

    public void StopFlashing()
    {
        // Called via Invoke in some states
        flashTransparency?.StopFlashing();
        paletteSwapper?.StopFlashing();
    }

    public IEnumerator UntouchableDurationCoroutine(float duration)
    {
        IsUntouchable = true;
        gameObject.layer = LayerMask.NameToLayer(PowerUpLayer);

        yield return new WaitForSeconds(duration);

        gameObject.layer = LayerMask.NameToLayer(MarioLayer);
        IsUntouchable = false;
    }

    public void SetColliderSize(Vector2 size, Vector2 offset)
    {
        _capsuleCollider.size = size;
        _capsuleCollider.offset = offset;
    }

    private void FreezeMario(float duration)
    {
        _freezeMachine.Freeze(duration);
    }

    // Example: Possibly called by FireMarioState
    public void ShootFireball()
    {
        Debug.Log("Shooting fireball!");
        // Fireball logic here
    }

    // Example: star power from FireMarioState or BigMarioState
    public void ActivateStarPower()
    {
        if (_starPowerCoroutine != null)
            StopCoroutine(_starPowerCoroutine);

        _starPowerCoroutine = StartCoroutine(StarPowerRoutine());
    }

    private IEnumerator StarPowerRoutine()
    {
        var previousState = _currentMarioState;
        // Switch to star state
        ChangeState(MarioState.Star);
        yield return new WaitForSeconds(starDuration);
        
        // Return to previous state
        ChangeState(previousState);
        _starPowerCoroutine = null;
    }

    private void SubscribeInputActions()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Crouch.started += OnCrouchStarted;
        _playerInputActions.Player.Crouch.canceled += OnCrouchCanceled;
        _playerInputActions.Player.Attack.performed += OnAttackPerformed;
    }

    private void UnsubscribeInputActions()
    {
        _playerInputActions.Player.Crouch.started -= OnCrouchStarted;
        _playerInputActions.Player.Crouch.canceled -= OnCrouchCanceled;
        _playerInputActions.Player.Attack.performed -= OnAttackPerformed;

        _playerInputActions.Player.Disable();
    }

    private void OnCrouchStarted(InputAction.CallbackContext context)
    {
        if (_currentState is SmallMarioState) return;
        if (_movementController.Grounded)
            Crouch();

    }

    private void Crouch()
    {
        Debug.Log("Crouching!");
        SetColliderSize(new Vector2(0.75f, 1f), Vector2.zero);
        _movementController.enabled = false;
        Animator.SetBool(OnCrouch, true);
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        if (_currentState is SmallMarioState) return;
        Debug.Log("Standing up!");
        SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
        _movementController.enabled = true;
        Animator.SetBool(OnCrouch, false);
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        // Could tie into "fireball" or "power-up" logic
        _currentState?.OnPickUpPowerUp(this, PowerUpType.FireFlower);
    }
    

    private void SubscribeGameEvents()
    {
        MarioEvents.OnMarioGotPowerUp += HandlePowerUp;
        MarioEvents.OnMarioGotHit += GotHit;
        GameEvents.FreezeAllCharacters += FreezeMario;
    }

    private void UnsubscribeGameEvents()
    {
        MarioEvents.OnMarioGotPowerUp -= HandlePowerUp;
        MarioEvents.OnMarioGotHit -= GotHit;
        GameEvents.FreezeAllCharacters -= FreezeMario;
    }

    private void HandlePowerUp(PowerUpType powerUpType)
    {
        _currentState?.OnPickUpPowerUp(this, powerUpType);
        if (powerUpType == PowerUpType.Star)
            ActivateStarPower();
    }
    
    

    private void EnablePhysics()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _capsuleCollider.enabled = true;
    }

    private void DisablePhysics()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _capsuleCollider.enabled = false;
    }
}
