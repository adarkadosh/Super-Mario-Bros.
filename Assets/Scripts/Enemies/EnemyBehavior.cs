using System.Collections;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    protected Animator Animator;
    private DeathAnimation _deathAnimation;


    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _deathAnimation = GetComponent<DeathAnimation>();
        if (_deathAnimation == null)
        {
            Debug.LogError("DeathAnimation component not found on " + gameObject.name);
        }
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
        // else if (other.gameObject.layer == LayerMask.NameToLayer($"StarMario"))
        // {
        //     StartCoroutine(DeathSequence());
        // }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("LethalEnemies"))
        {
            if (gameObject.layer != LayerMask.NameToLayer("LethalEnemies"))
            {
                StartCoroutine(DeathSequence());
            }
        }
    }


    public IEnumerator DeathSequence()
    {
        DeathSequenceAnimation();
        // Trigger the death animation
        _deathAnimation.TriggerDeathAnimation();

        // Wait for the duration of the death animation
        yield return new WaitForSeconds(3f); // Match this to your animation's duration

        // Execute the Kill method
        Kill();
    }
    
    protected abstract void DeathSequenceAnimation();

    protected abstract void GotHit();
    public abstract void Kill();
}