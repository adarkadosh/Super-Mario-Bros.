using System.Collections;
using UnityEngine;

public class GenericBlockHit : MonoBehaviour
{
    private static readonly int GotHit = Animator.StringToHash("GotHit");
    // Change SpriteRenderer to Renderer
    [SerializeField] private Sprite emptyBlockSprite;
    [Header("Block Settings")]
    [SerializeField] private int maxHitsToBlock = 1;
    
    [Header("Power-Up Settings")]
    [Tooltip("Type of power-up to spawn.")]
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private bool isBlockCoin;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _isHit;
    private static bool _isAnyBlockHit;
    
    private PowerUpFactory _powerUpFactory;
    
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _powerUpFactory = PowerUpFactory.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isAnyBlockHit || _isHit || maxHitsToBlock == 0)
            // if (_isHit || maxHitsToBlock == 0)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 direction = transform.position - other.transform.position;
            if (Vector2.Dot(direction.normalized, Vector2.up) > 0.35f)
            {
                Hit();
                Debug.Log("Player hit the block");
            }
            else
            {
                Debug.Log("Player collided with the block but did not hit it upwards");
            }
        }
    }

    private void Hit()
    {
        _isAnyBlockHit = true;
        _isHit = true;
        GetComponent<SpriteRenderer>().enabled = true;
        if (maxHitsToBlock > 0)
        {
            BlockCoin();
        }
        maxHitsToBlock--;
        if (maxHitsToBlock == 0)
        {
            if (_animator == null && emptyBlockSprite != null)
            {
                _spriteRenderer.sprite = emptyBlockSprite;
            }
            else
            {
                _animator.SetBool(GotHit, true);
            }
        }
        
        StartCoroutine(AnimatedBlockGotHitCoroutine());
    }

    private void TriggerEffect()
    {
        // Default behavior: No special effect
        if (_powerUpFactory != null && powerUpType != PowerUpType.Nothing)
        {
            var powerUp = _powerUpFactory.GetInstance(powerUpType, transform.position);
            powerUp.Trigger();
            Debug.Log($"{powerUpType} spawned from block hit.");
        }
        // BlockCoin();
    }

    private void BlockCoin()
    {
        if (!isBlockCoin) return;
        // Spawn a coin
        var coin = _powerUpFactory.GetBlockCoin(transform.position + Vector3.up);
        coin.Trigger();
        Debug.Log("Coin spawned from block hit.");
    }


    private IEnumerator AnimatedBlockGotHitCoroutine()
    {
        // Start the animation coroutine
        yield return Extensions.AnimatedBlockGotHit(gameObject);

        if (maxHitsToBlock >= 0)
        {
            TriggerEffect();
        }
        
        // Reset hit states
        _isHit = false;
        _isAnyBlockHit = false;
    }
}