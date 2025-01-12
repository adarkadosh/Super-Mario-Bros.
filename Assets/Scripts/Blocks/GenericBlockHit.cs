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

        if (maxHitsToBlock == 0)
        {
            _animator?.SetBool(GotHit, true);
        }

        TriggerEffect();

        AnimatedBlockGotHit();
    }

    protected virtual void TriggerEffect()
    {
        // Default behavior: No special effect
        Debug.Log("Block hit with no special effect.");
    }

    private void AnimatedBlockGotHit()
    {
        // Replace with your animation logic
        // yield return new WaitForSeconds(0.5f);
        StartCoroutine(Extensions.AnimatedBlockGotHit(gameObject));
        _isHit = false;
        isAnyBlockHit = false;
    }
}