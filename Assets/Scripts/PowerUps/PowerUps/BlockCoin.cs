// BlockCoin.cs

using System.Collections;
using UnityEngine;

public class BlockCoin : MonoBehaviour, IPoolable
{
    [SerializeField] private int coinsToGive = 1;
    [SerializeField] private float animationHeight = 3.5f;
    [SerializeField] private float animationDuration = 0.25f;

    public void Trigger()
    {
        GameEvents.OnCoinCollected?.Invoke(coinsToGive);
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        yield return Extensions.AnimatedBlockGotHit(gameObject, animationHeight, animationDuration,
            animationHeight / 2);
        MonoPool<BlockCoin>.Instance.Return(this);
    }

    public void Reset()
    {
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
    }
}