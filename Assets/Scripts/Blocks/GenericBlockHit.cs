using System.Collections;
using UnityEngine;

public class GenericBlockHit : MonoBehaviour
{
    private static readonly int GotHit = Animator.StringToHash("GotHit");
    [SerializeField] private int maxHitsToBlock = 1;

    private Animator _animator;
    private bool _isHit;
    private static bool isAnyBlockHit;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isAnyBlockHit || _isHit || maxHitsToBlock == 0)
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
        isAnyBlockHit = true;
        _isHit = true;
        // GetComponent<SpriteRenderer>().enabled = true;
        maxHitsToBlock--;

        if (maxHitsToBlock == 0 && _animator != null)
        {
            _animator.SetBool(GotHit, true);
        }
        
        StartCoroutine(AnimatedBlockGotHitCoroutine());
    }

    protected virtual void TriggerEffect()
    {
        // Default behavior: No special effect
        Debug.Log("Block hit with no special effect.");
    }


    private IEnumerator AnimatedBlockGotHitCoroutine()
    {
        // Start the animation coroutine
        yield return Extensions.AnimatedBlockGotHit(gameObject);

        // Reset hit states
        _isHit = false;
        isAnyBlockHit = false;

        // Trigger the effect (e.g., spawn mushroom)
        if (maxHitsToBlock >= 0)
        {
            TriggerEffect();
        }
    }
}