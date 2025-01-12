using PowerUps;
using UnityEngine;

namespace Blocks
{
    public class BlockHitWithPowerUp : GenericBlockHit
    {
        [Header("Power-Up Settings")]
        [Tooltip("Type of power-up to spawn.")]
        [SerializeField] private PowerUpType powerUpType;

        // [Tooltip("Offset position where the power-up will spawn relative to the block.")]
        // [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1, 0);

        // [Tooltip("Duration to wait before spawning the power-up.")]
        // [SerializeField] private float spawnDelay = 0.1f;

        /// <summary>
        /// Enum to define different types of power-ups.
        /// Extend this enum as you add more power-up types.
        /// </summary>
        public enum PowerUpType
        {
            Mashroom,
            Star,
            OneUp,
            // Add other power-up types here, e.g., Star, FireFlower
        }
        [SerializeField] private PowerUpPool powerUpPool;

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