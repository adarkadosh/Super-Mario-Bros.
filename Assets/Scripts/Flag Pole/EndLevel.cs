using UnityEngine;

namespace Flag_Pole
{
    public class EndLevel : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Trigger Entered by: " + other.gameObject.name);
            if (other.CompareTag("Player"))
            {
                other.gameObject.SetActive(false);
                GameEvents.OnLevelCompleted?.Invoke();
            }
        }
    }
}