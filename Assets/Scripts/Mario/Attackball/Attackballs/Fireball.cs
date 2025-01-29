using Enemies;
using Managers;
using UnityEngine;

namespace Mario.Attackball.Attackballs
{
    public class Fireball : AttackBall
    {
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if the fireball collides with an enemy layer
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // Apply damage to the enemy
                EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
                GameEvents.OnEventTriggered?.Invoke(ScoresSet.OneHundred, transform.position);
                EntityMovement.enabled = false;
                Collider.enabled = false;
                Rigidbody.linearVelocity = Vector2.zero;
                Rigidbody.bodyType = RigidbodyType2D.Kinematic;
                Animator.SetTrigger("Explode");
                if (enemy != null)
                {
                    StartCoroutine(enemy.DeathSequence());
                }
            }
        }
    }
}