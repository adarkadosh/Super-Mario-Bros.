using PowerUps;
using UnityEngine;

namespace Blocks
{
    public class BlockHitWithPowerUp : GenericBlockHit
    {
        [SerializeField] private PowerupMashrromPool powerUpPool;

        protected override void TriggerEffect()
        {
            if (powerUpPool != null)
            {
                var powerUp = powerUpPool.Get();
                powerUp.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                powerUp.Trigger();
                Debug.Log("Power-up spawned from block hit.");
            }
            else
            {
                Debug.LogWarning("Power-up pool is not assigned.");
            }
        }
    }
}