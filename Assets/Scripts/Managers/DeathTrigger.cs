using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MarioEvents.OnMarioGotHit?.Invoke();
            other.gameObject.SetActive(false);
            // other.GetComponent(PlayerInputActions).enabled = false;
            GameManager.Instance.ResetLevel(3f);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
