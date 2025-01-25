using UnityEngine;

public class IceFlower : GenericPowerUp
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a powerup");
            MarioEvents.OnMarioGotPowerUp?.Invoke(PowerUpType.IceFlower);
            PowerUpFactory.Instance.Return(this);
        }
    }
}