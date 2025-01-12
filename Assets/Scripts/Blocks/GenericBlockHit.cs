using System.Collections;
using UnityEngine;

public class GenericBlockHit : MonoBehaviour
{
    private static readonly int GotHit = Animator.StringToHash("GotHit");
    [SerializeField] private int maxHitsToBlock = 1;


    [Header("Power-Up Settings")]
    [Tooltip("Type of power-up to spawn.")]
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private bool isBlockCoin;

    private Animator _animator;
    private bool _isHit;
    private static bool _isAnyBlockHit;
    

    
    public enum PowerUpType
    {
        SuperMashroom,
        OneUpMashroom,
        Star,
        Nothing
        // Add other power-up types here, e.g., Star, FireFlower
    }
    private PowerUpFactory _powerUpFactory;
    

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _powerUpFactory = PowerUpFactory.Instance;

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
        // GetComponent<SpriteRenderer>().enabled = true;
        if (maxHitsToBlock > 0)
        {
            TriggerEffect();
        }
        maxHitsToBlock--;
        if (maxHitsToBlock == 0 && _animator != null)
        {
            _animator.SetBool(GotHit, true);
        }
        
        StartCoroutine(AnimatedBlockGotHitCoroutine());
    }

    private void TriggerEffect()
    {
        // Default behavior: No special effect
        if (_powerUpFactory != null && powerUpType != PowerUpType.Nothing)
        {
            if (isBlockCoin)
            {
                // Spawn a coin
                var coin = _powerUpFactory.GetBlockCoin(transform.position);
                coin.Trigger();
                Debug.Log("Coin spawned from block hit.");
            }
            var powerUp = _powerUpFactory.GetInstance(powerUpType, transform.position);
            powerUp.Trigger();
            Debug.Log($"{powerUpType} spawned from block hit.");
        }
        else
        {
            Debug.LogWarning("Power-up factory is not assigned.");
        }
    }


    private IEnumerator AnimatedBlockGotHitCoroutine()
    {
        // Start the animation coroutine
        yield return Extensions.AnimatedBlockGotHit(gameObject);

        // Reset hit states
        _isHit = false;
        _isAnyBlockHit = false;
    }
}