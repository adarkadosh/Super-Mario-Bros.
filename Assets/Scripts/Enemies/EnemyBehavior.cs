using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    protected Animator Animator;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Check if the player is above the Goomba
            Vector2 direction = transform.position - other.transform.position;
            if (Vector2.Dot(direction.normalized, Vector2.down) > 0.25f)
            {
                GotHit();
            }
            else
            {
                MarioEvents.OnMarioGotHit?.Invoke();
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("LethalEnemies"))
        {
            GotHitByLethalEnemy();
        }
    }

    private void GotHitByLethalEnemy()
    {
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

    protected abstract void GotHit();

    protected void Kill()
    {
        Destroy(gameObject);
    }
}