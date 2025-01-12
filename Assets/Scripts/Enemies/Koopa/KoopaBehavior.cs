using System;
using Enemies;
using UnityEngine;
using UnityEngine.Serialization;

public class KoopaBehavior : EnemyBehavior
{
    //TODO: make goomba by states
    //TODO: make goomba return to normal state after some time
    //TODO: make goomba not moving when collided with other enemies
    [SerializeField] private float shellSpeed = 12f;
    private static readonly int EnterShell = Animator.StringToHash("EnterShell");

    private bool _isShell;
    private bool _isPushed;
    private bool ShellIsMoving => _isShell && GetComponent<Rigidbody2D>().linearVelocity.magnitude > 0.1f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isShell && other.CompareTag("Player"))
        {
            if (!_isPushed)
            {
                Vector2 direction = new Vector2(transform.position.x - other.transform.position.x, 0);
                PushShell(direction);
            }
            else
            {
                MarioEvents.OnMarioGotHit?.Invoke();
            }
        }
    }

    private void PushShell(Vector2 direction)
    {
        _isPushed = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        EntityMovement movement = GetComponent<EntityMovement>();
        movement.MovementDirection = direction.normalized;
        movement.MovementSpeed = shellSpeed;
        movement.enabled = true;
        
        gameObject.layer = LayerMask.NameToLayer($"LethalEnemies");
    }


    protected override void GotHit()
    {
        if (_isShell)
        {
            return;
        }

        _isShell = true;
        Animator.SetBool(EnterShell, true);
        // GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }
}
