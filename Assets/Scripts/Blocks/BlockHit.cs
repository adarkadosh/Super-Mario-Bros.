using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BlockHit : MonoBehaviour
{
    [SerializeField] private int maxHitsToBlock = 1;
    private Animator _animator;
    private int _hitsToBlock;
    private bool _isHit;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !_isHit)
        {
            Vector2 direction = transform.position - other.transform.position;
            if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
            {
                Hit();
                Debug.Log("Mario hit the block");
            }
            Debug.Log("Player hit the block");
            
        }
    }

    private void Hit()
    {
        _hitsToBlock++;
        if (_hitsToBlock >= maxHitsToBlock)
        {
            
        }

        StartCoroutine(AnimatedBlockBreak());
    }

    private IEnumerator AnimatedBlockBreak()
    {
        _isHit = true;
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + new Vector3(0f, 0.5f, 0f);
        yield return transform.DOMoveY(transform.position.y + 0.5f, 0.125f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.125f);
        yield return transform.DOMoveY(originalPosition.y, 0.075f).SetEase(Ease.Linear);
        // yield return new WaitForSeconds(0.5f);
        _isHit = false;
    }
}
