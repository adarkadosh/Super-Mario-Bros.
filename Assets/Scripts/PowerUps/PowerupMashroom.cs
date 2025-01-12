using System.Collections;
using DG.Tweening;
using UnityEngine;


public class PowerupMashroom : MonoBehaviour, IPoolable
{
    public void Reset()
    {
        GetComponent<EntityMovement>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        enabled = false;
    }

    public void Trigger()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        // yield return Extensions.AnimatedBlockGotHit(gameObject, 2f, 0.5f);
        yield return gameObject.transform.DOMoveY(gameObject.transform.position.y + 0.5f, 0.25f)
            .SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.25f);
        GetComponent<EntityMovement>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<EntityMovement>().MovementDirection = Vector2.right;
    } 
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a powerup");
            MarioEvents.OnPowerupCollected?.Invoke();
            MonoPool<PowerupMashroom>.Instance.Return(this);
        }
    }
    
}