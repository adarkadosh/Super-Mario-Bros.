using System;
using UnityEngine;

public class GoombaBehavior : EnemyBehavior
{
    private static readonly int Squished = Animator.StringToHash("IsSquished");

    protected override void GotHit()
    {
        Animator.SetBool(Squished, true);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        // GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }
}
