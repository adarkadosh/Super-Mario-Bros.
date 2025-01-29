using Enemies;
using PowerUps;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Invoke the event signaling that Mario got hit
            MarioEvents.OnMarioDeath?.Invoke();

            // Freeze all characters for 3 seconds
            GameEvents.FreezeAllCharacters?.Invoke(3f);

            // Deactivate the player GameObject
            other.gameObject.SetActive(false);

            // Reset the level after 3 seconds
            GameManager.Instance.ResetLevel(3f);
        }
        // Check if the colliding object's layer is "Enemy" or "LethalEnemies"
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                 other.gameObject.layer == LayerMask.NameToLayer("LethalEnemies"))
        {
            // Attempt to get the EnemyBehavior component
            var enemyBehavior = other.GetComponent<EnemyBehavior>();
            if (enemyBehavior != null)
            {
                // Call the Kill method on the enemy
                enemyBehavior.Kill();
            }
            else
            {
                Debug.LogWarning($"EnemyBehavior component missing on {other.gameObject.name}");
            }
        }
        // Check if the colliding object's layer is "PowerUp"
        else if (other.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
        {
            // Attempt to get the GenericPowerUp component
            var powerUp = other.GetComponent<GenericPowerUp>();
            if (powerUp != null)
            {
                // Return the power-up to the factory
                PowerUpFactory.Instance.Return(powerUp);
            }
            else
            {
                Debug.LogWarning($"GenericPowerUp component missing on {other.gameObject.name}");
            }
        }
    }
}