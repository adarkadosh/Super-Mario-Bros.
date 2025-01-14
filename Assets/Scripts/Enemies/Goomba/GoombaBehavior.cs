using System;
using UnityEngine;

public class GoombaBehavior : EnemyBehavior, IPoolable
{
    private static readonly int Squished = Animator.StringToHash("IsSquished");

    protected override void GotHit()
    {
        // GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        // Animator.SetBool(Squished, true);
        // GetComponent<Collider2D>().enabled = false;
        // GetComponent<EntityMovement>().enabled = false;
        // // GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero; // Reset velocity to stop movement
        rb.angularVelocity = 0f;    // Reset angular velocity if necessary
        // rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; // Prevent vertical movement and rotation

        Animator.SetBool(Squished, true);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().enabled = false;
    }

    public void Reset()
    {
        Animator.SetBool(Squished, false);
        GetComponent<DeathAnimation>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero; // Reset velocity
        rb.angularVelocity = 0f;    // Reset angular velocity
        // rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Allow vertical movement, but prevent rotation

        GetComponent<Collider2D>().enabled = true;
        GetComponent<EntityMovement>().enabled = true;
    }

    public void Trigger()
    {
        throw new NotImplementedException();
    }
}
