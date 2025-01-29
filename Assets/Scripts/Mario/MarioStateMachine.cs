using System.Collections;
using Enemies;
using Managers;
using Mario.Attackball.Pools;
using Mario.MarioAnimations;
using Mario.MarioStates;
using PowerUps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mario
{
    public enum MarioState
    {
        Small,
        Big,
        Fire,
        GrowShrink,
        Star,
        Ice
    }


    public class MarioStateMachine : MonoBehaviour
    {
        private static readonly int OnCrouch = Animator.StringToHash("OnCrouch");
        private static readonly int Fire = Animator.StringToHash("Fire");
    
        [SerializeField] private MarioState initialMarioState = MarioState.Small;
        // [SerializeField] private ScoresSet scoreForPowerUp = ScoresSet.OneThousand;

        [SerializeField] private FireballPool fireballPool;
        [SerializeField] private IceballPool iceballPool;

        [Header("Durations")] [SerializeField] private float starDuration = 10f;
        [SerializeField] private float starDurationDelay = 5f;
        [SerializeField] private float untouchableDuration = 2.1f;
    
        [Header("Audio")] 
        [SerializeField] private AudioClip starPowerAudioClip;
        [SerializeField] private AudioClip powerUpAudioClip;
        [SerializeField] private AudioClip hitAudioClip;
        [SerializeField] private AudioClip dieAudioClip;
        [SerializeField] private AudioClip onGotBigAudioClip;
        [SerializeField] private AudioClip onGotSmallAudioClip;
        [SerializeField] private AudioClip attackSound;

        [Header("References")] [SerializeField]
        private PaletteSwapper paletteSwapper;

        [SerializeField] private FlashTransparency flashTransparency;

        private Rigidbody2D _rigidbody;
        private CapsuleCollider2D _capsuleCollider;
        private MarioMovementControl _movementController;
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
        public AudioClip PowerDownClip => onGotSmallAudioClip;

        private const string MarioLayer = "Mario";
        private const string PowerUpLayer = "PowerUp";

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            Animator = GetComponent<Animator>();
            _movementController = GetComponent<MarioMovementControl>();
            _freezeMachine = GetComponent<FreezeMachine>();

            // Set up input
            _playerInputActions = _movementController.PlayerInputActions;
        }

        private void Start()
        {
            // Start in SmallMarioState (using the factory).
            ChangeState(initialMarioState);
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
            // check if this is enemy by layer:
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !collision.gameObject.CompareTag($"ShellKoopa"))
            {
                if (collision.collider.GetComponent<Collider2D>() && collision.contacts[0].normal.y > 0)
                {
                    SoundFXManager.Instance.PlaySound(hitAudioClip, transform);
                    EnemyBehavior enemy = collision.collider.GetComponent<EnemyBehavior>();
                    GameEvents.OnEnemyHit?.Invoke(enemy.enemyScore, transform.position);
                    enemy.GotHit();
                }
            }

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
            gameObject.layer = LayerMask.NameToLayer(PowerUpLayer);

            yield return new WaitForSeconds(duration);

            gameObject.layer = LayerMask.NameToLayer(MarioLayer);
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

        private void ShootFireball()
        {
            Animator.SetTrigger(Fire);
            SoundFXManager.Instance.PlaySound(attackSound, transform);
            Debug.Log("Shooting fireball!");

            // Determine the direction Mario is facing
            Vector2 shootDirection = transform.right;
            Debug.Log("Shoot direction: " + shootDirection);

            // Define the offset distance
            const float offsetDistance = 0.5f; // Adjust this value as needed

            // Calculate the spawn position with the offset
            var spawnPosition = (Vector2)transform.position + shootDirection * offsetDistance;

            // Get a fireball from the pool
            var fireball = fireballPool.Get();
            if (fireball == null)
            {
                Debug.LogWarning("No fireball available in the pool.");
                return;
            }

            fireball.transform.position = spawnPosition;
            fireball.SetDirection(new Vector2(shootDirection.x, -0.5f));
        }
    
        private void ShootIceBall()
        {
            Animator.SetTrigger(Fire);
            SoundFXManager.Instance.PlaySound(attackSound, transform);
            Debug.Log("Shooting fireball!");

            // Determine the direction Mario is facing
            Vector2 shootDirection = transform.right;
            Debug.Log("Shoot direction: " + shootDirection);

            // Define the offset distance
            const float offsetDistance = 0.5f; // Adjust this value as needed

            // Calculate the spawn position with the offset
            var spawnPosition = (Vector2)transform.position + shootDirection * offsetDistance;

            // Get a fireball from the pool
            var fireball = iceballPool.Get();
            if (fireball == null)
            {
                Debug.LogWarning("No fireball available in the pool.");
                return;
            }

            fireball.transform.position = spawnPosition;
            fireball.SetDirection(new Vector2(shootDirection.x, -0.5f));
        }

        // Example: star power from FireMarioState or BigMarioState
        private void ActivateStarPower()
        {
            if (_starPowerCoroutine != null)
                StopCoroutine(_starPowerCoroutine);
        
            // Play audio
            SoundFXManager.Instance.ChangeBackgroundMusic(starPowerAudioClip);
            _starPowerCoroutine = StartCoroutine(StarPowerRoutine());
        }

        private IEnumerator StarPowerRoutine()
        {
            var previousState = _currentMarioState;
            // Switch to star state
            ChangeState(MarioState.Star);
            yield return new WaitForSeconds(starDuration + starDurationDelay);

            // Return to previous state
            ChangeState(previousState);
            // _starPowerCoroutine = null;
            SoundFXManager.Instance.ResetBackgroundMusic();
        }

        private void SubscribeInputActions()
        {
            _playerInputActions.Player.Crouch.started += OnCrouchStarted;
            _playerInputActions.Player.Crouch.canceled += OnCrouchCanceled;
            _playerInputActions.Player.Attack.performed += OnAttackPerformed;
        }

        private void UnsubscribeInputActions()
        {
            _playerInputActions.Player.Crouch.started -= OnCrouchStarted;
            _playerInputActions.Player.Crouch.canceled -= OnCrouchCanceled;
            _playerInputActions.Player.Attack.performed -= OnAttackPerformed;
        }

        private void OnCrouchStarted(InputAction.CallbackContext context)
        {
            if (_currentState is SmallMarioState) return;
            if (_movementController.Grounded)
            {
                Crouch();
            }
        }

        private void Crouch()
        {
            Debug.Log("Crouching!");
            SetColliderSize(new Vector2(0.75f, 1f), Vector2.zero);
            _movementController.DisableMovement();
            Animator.SetBool(OnCrouch, true);
        }

        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            if (_currentState is SmallMarioState) return;
            Debug.Log("Standing up!");
            SetColliderSize(new Vector2(0.75f, 2f), new Vector2(0f, 0.5f));
            _movementController.EnableMovement();
            Animator.SetBool(OnCrouch, false);
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            // Could tie into "fireball" or "power-up" logic
            // _currentState?.OnPickUpPowerUp(this, PowerUpType.FireFlower);
            if (_currentMarioState == MarioState.Fire)
            {
                ShootFireball();
            } else if (_currentMarioState == MarioState.Ice)
            {
                ShootIceBall();
            }
        }
    
        private void SubscribeGameEvents()
        {
            MarioEvents.OnMarioGotPowerUp += HandlePowerUp;
            MarioEvents.OnMarioGotHit += GotHit;
            MarioEvents.OnMarioDeath += MarioDie;
            GameEvents.OnTimeUp += MarioDie;
            GameEvents.FreezeAllCharacters += FreezeMario;
        }

        private void MarioDie()
        {
            // SoundFXManager.Instance.PlaySpatialSound(dieAudioClip, transform);
            DeathAnimation deathAnim = GetComponent<DeathAnimation>();
            if (deathAnim != null)
                deathAnim.TriggerDeathAnimation(1);
        }

        private void UnsubscribeGameEvents()
        {
            MarioEvents.OnMarioGotPowerUp -= HandlePowerUp;
            MarioEvents.OnMarioGotHit -= GotHit;
            MarioEvents.OnMarioDeath -= MarioDie;
            GameEvents.OnTimeUp -= MarioDie;
            GameEvents.FreezeAllCharacters -= FreezeMario;
        }

        private void HandlePowerUp(PowerUpType powerUpType)
        {
            _currentState?.OnPickUpPowerUp(this, powerUpType);
            SoundFXManager.Instance.PlaySound(powerUpAudioClip, transform);
            GameEvents.OnEventTriggered?.Invoke(ScoresSet.OneThousand, transform.position);
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
}