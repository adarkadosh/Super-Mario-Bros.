using UnityEngine;

namespace PowerUps.PowerUps
{
    public class FireFlower : GenericPowerUp
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player collected a powerup");
                MarioEvents.OnMarioGotPowerUp?.Invoke(PowerUpType.FireFlower);
                FireFlowerPool.Instance.Return(this);
            }
        }
        
    }
}