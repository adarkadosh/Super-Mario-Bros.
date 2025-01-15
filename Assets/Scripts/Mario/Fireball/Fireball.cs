using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public int damage = 1;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Move the fireball in the direction it's facing
        rb.linearVelocity = transform.right * speed;
        // Destroy the fireball after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the fireball collides with an enemy
        if (collision.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                StartCoroutine(enemy.DeathSequence());
            }
            // Destroy the fireball upon collision
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            // Destroy the fireball if it hits the ground
            Destroy(gameObject);
        }
    }
}