using UnityEngine;

public class IceBall : AttackBall
{
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the fireball collides with an enemy layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Apply damage to the enemy
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            FreezeMachine freezeMachine = collision.GetComponent<FreezeMachine>();
            GameEvents.OnEventTriggered?.Invoke(ScoresSet.OneHundred, transform.position);
            if (freezeMachine != null)
            {
                freezeMachine.FreezeAttack(2);
            }

            EntityMovement.enabled = false;
            Collider.enabled = false;
            Rigidbody.linearVelocity = Vector2.zero;
            Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Animator.SetTrigger("Explode");
        }
    }
}