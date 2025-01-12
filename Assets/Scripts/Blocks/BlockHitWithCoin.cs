using UnityEngine;

public class GenericBlockHitWithCoin : GenericBlockHit
{
    [SerializeField] private BlockCoinPool coinPool;

    protected override void TriggerEffect()
    {
        if (coinPool != null)
        {
            var coin = coinPool.Get();
            coin.transform.position = transform.position;
            coin.Trigger();
            Debug.Log("Coin spawned from block hit.");
        }
        else
        {
            Debug.LogWarning("Coin pool is not assigned.");
        }
    }
}