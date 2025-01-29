using System.Collections;
using Mario;
using PowerUps;
using UnityEngine;

// TODO: Check if collision is good (sometimes its weird)
namespace Blocks
{
    public sealed class GenericBlockHit : MonoBehaviour
    {
        private static readonly int GotHit = Animator.StringToHash("GotHit");
    
        [Header("Sound Settings")]
        [SerializeField] private AudioClip blockHitSound;
        [SerializeField] private AudioClip blockBreakSound;
        [SerializeField] private AudioClip blockCoinSound;
        [SerializeField] private AudioClip blockPowerUpSound;
    
        [Header("Block Settings")]
        // Change SpriteRenderer to Renderer
        [SerializeField] private Sprite emptyBlockSprite;
        [SerializeField] private int maxHitsToBlock = 1;

        [Header("Power-Up Settings")] [Tooltip("Type of power-up to spawn.")] 
        [SerializeField] private PowerUpType powerUpType;
        [SerializeField] private bool isBlockCoin;
        [SerializeField] private GameObject brokenBlock;

        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private PowerUpFactory _powerUpFactory;

        private static bool _isAnyBlockHit;
        private bool _isHit;
        private bool _isMarioBig;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _powerUpFactory = PowerUpFactory.Instance;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isAnyBlockHit) return;  // Prevent simultaneous block hits
            if (_isHit) return;          // Block already hit
            if (maxHitsToBlock == 0) return;  // No hits remaining

            if (other.gameObject.CompareTag("Player"))
            {
                // Vector2 direction = transform.position - other.transform.position;
                // if (Vector2.Dot(direction.normalized, Vector2.up) > 0.35f)
                // if (other.collider.GetComponent<Collider2D>() && other.contacts[0].normal.y > 0)
                Vector2 direction = transform.position - other.transform.position;
                if (Vector2.Dot(direction.normalized, Vector2.up) > 0.45f)
                {
                    Hit();
                    Extensions.Log("Player hit the block");
                }
                else
                {
                    Extensions.Log("Player collided with the block but did not hit it upwards");
                }
            }
        }

        // private void Hit()
        // {
        //     _isAnyBlockHit = true;
        //     _isHit = true;
        //     GetComponent<SpriteRenderer>().enabled = true;
        //     if (maxHitsToBlock < 0)
        //     {
        //         GameObject a = Instantiate(brokenBlock, transform.position, Quaternion.Euler(0, 0, 45));
        //         gameObject.SetActive(false);
        //         _isAnyBlockHit = false;
        //         // Destroy(gameObject);
        //         return;
        //     }
        //     if (maxHitsToBlock > 0)
        //     {
        //         BlockCoin();
        //     }
        //
        //     maxHitsToBlock--;
        //     if (maxHitsToBlock == 0)
        //     {
        //         if (_animator == null && emptyBlockSprite != null)
        //         {
        //             _spriteRenderer.sprite = emptyBlockSprite;
        //         }
        //         else
        //         {
        //             _animator.SetBool(GotHit, true);
        //         }
        //     }
        //
        //     StartCoroutine(AnimatedBlockGotHitCoroutine());
        // }
        private void Hit()
        {
            _isAnyBlockHit = true;
            _isHit = true;
            _spriteRenderer.enabled = true;

            if (_isMarioBig && maxHitsToBlock < 0)
            {
                SoundFXManager.Instance.PlaySpatialSound(blockBreakSound, transform);
                DestroyBlock();
                return;
            }

            SoundFXManager.Instance.PlaySpatialSound(blockHitSound, transform);
            ProcessHit();
            StartCoroutine(AnimatedBlockGotHitCoroutine());
        }

        private void DestroyBlock()
        {
            Extensions.KillEnemiesOnBlock(gameObject);
            Instantiate(brokenBlock, transform.position, Quaternion.Euler(0, 0, 45));
            StartCoroutine(Extensions.WaitForSeconds(0.4f));
            gameObject.SetActive(false);
            _isAnyBlockHit = false;
        }

        private void ProcessHit()
        {
            BlockCoin();
            maxHitsToBlock--;
            UpdateBlockAppearance();
        }

        private void UpdateBlockAppearance()
        {
            if (maxHitsToBlock == 0)
            {
                if (_animator == null && emptyBlockSprite != null)
                {
                    _spriteRenderer.sprite = emptyBlockSprite;
                }
                else if (_animator != null)
                {
                    _animator.SetBool(GotHit, true);
                }
            }
        }

        private void TriggerEffect()
        {
            // Default behavior: No special effect
            if (_powerUpFactory != null && powerUpType != PowerUpType.Nothing)
            {
                SoundFXManager.Instance.PlaySpatialSound(blockPowerUpSound, transform);
                var powerUp = _powerUpFactory.Spawn(powerUpType);
                powerUp.transform.position = transform.position;
                powerUp.Trigger();
                Extensions.Log($"{powerUpType} spawned from block hit.");
            }
            // BlockCoin();
        }

        private void BlockCoin()
        {
            if (!isBlockCoin) return;
            // Spawn a coin
            SoundFXManager.Instance.PlaySpatialSound(blockCoinSound, transform);
            var coin = _powerUpFactory.GetBlockCoin(transform.position + Vector3.up);
            coin.Trigger();
            Extensions.Log("Coin spawned from block hit.");
        }


        private IEnumerator AnimatedBlockGotHitCoroutine()
        {
            try
            {
                yield return Extensions.AnimatedBlockGotHit(gameObject);
                if (maxHitsToBlock >= 0)
                {
                    TriggerEffect();
                }
            }
            finally
            {
                _isHit = false;
                _isAnyBlockHit = false;
            }
        }

        private void OnEnable()
        {
            MarioEvents.OnMarioStateChange += UpdateMarioState;
        }
    
        private void OnDisable()
        {
            MarioEvents.OnMarioStateChange -= UpdateMarioState;
        }

        private void UpdateMarioState(MarioState newState)
        {
            if (newState == MarioState.Star)
                return;
            _isMarioBig = newState != MarioState.Small;
        }
    }
}