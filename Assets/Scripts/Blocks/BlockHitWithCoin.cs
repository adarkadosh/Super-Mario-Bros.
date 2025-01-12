using UnityEngine;

public class GenericBlockHitWithCoin : GenericBlockHit
{
    [SerializeField] private BlockCoinPool coinPool;

    protected override void TriggerEffect()
    {
        if (coinPool != null)
        {
            var coin = coinPool.Get();
            coin.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            coin.Trigger();
            Debug.Log("Coin spawned from block hit.");
        }
        else
        {
            Debug.LogWarning("Coin pool is not assigned.");
        }
    }
}