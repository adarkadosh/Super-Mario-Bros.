using PowerUps;
using UnityEngine;

public class Star : GenericPowerUp
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a powerup");
            MarioEvents.OnMarioGotPowerUp?.Invoke(PowerUpType.Star);
            PowerUpFactory.Instance.Return(this);

            // PowerUpFactory.Instance.Return(this);
        }
    }
}