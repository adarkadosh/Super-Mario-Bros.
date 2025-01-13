using UnityEngine;


public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a coin");
            MarioEvents.OnGotCoin?.Invoke(coinValue);
            Destroy(gameObject);
        }
    }
}