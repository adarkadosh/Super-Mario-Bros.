using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered by: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            // GameEvents.OnEventTriggered?.Invoke(ScoresSet.OneThousand, transform.position)
            GameEvents.OnLevelCompleted?.Invoke();
        }
    }
}