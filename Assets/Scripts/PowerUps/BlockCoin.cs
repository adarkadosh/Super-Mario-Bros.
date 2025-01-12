// BlockCoin.cs
using System.Collections;
using UnityEngine;

public class BlockCoin : MonoBehaviour, IPoolable
{
    [SerializeField] private int coinsToGive = 1;

    public void Trigger()
    {
        GameEvents.OnCoinCollected?.Invoke(coinsToGive);
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        yield return Extensions.AnimatedBlockGotHit(gameObject, 2f, 0.25f, 1f);
        MonoPool<BlockCoin>.Instance.Return(this);
    }

    public void Reset()
    {
        // Reset any additional state if necessary
        // For example, reset position, animations, etc.
        // Currently disabling the component as in your original code
        enabled = false;
    }
}