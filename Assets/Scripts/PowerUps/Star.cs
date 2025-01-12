using PowerUps;
using UnityEngine;

public class Star : PowerUp
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a powerup");
            MarioEvents.OnGotStar?.Invoke();
            StarPool.Instance.Return(this);
        }
    }
}