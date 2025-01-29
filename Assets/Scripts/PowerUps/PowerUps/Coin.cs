using Managers;
using UnityEngine;

namespace PowerUps.PowerUps
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int coinValue = 1;
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player collected a coin");
                GameEvents.OnCoinCollected?.Invoke(coinValue);
                Destroy(gameObject);
            }
        }
    }
}