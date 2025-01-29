using System.Collections;
using UnityEngine;

namespace PowerUps.PowerUps
{
    public class Star : GenericPowerUp
    {
        private void Start()
        {
            StartCoroutine(ChangeRenderer());
        }

        private IEnumerator ChangeRenderer()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(1f);
            spriteRenderer.sortingLayerName = "Blocks";
            spriteRenderer.sortingOrder = 1;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player collected a powerup");
                MarioEvents.OnMarioGotPowerUp?.Invoke(PowerUpType.Star);
                PowerUpFactory.Instance.Return(this);
            }
        }
    }
}