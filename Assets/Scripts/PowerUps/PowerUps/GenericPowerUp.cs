using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class GenericPowerUp : MonoBehaviour, IPoolable
{
    protected EntityMovement EntityMovement;
    protected Collider2D Collider2D;
    protected Rigidbody2D Rigidbody2D;
    
    private void Awake()
    {
        EntityMovement = GetComponent<EntityMovement>();
        Collider2D = GetComponent<Collider2D>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    public void Reset()
    {
        if (EntityMovement != null)
        {
            EntityMovement.enabled = false;
        }
        if (Collider2D != null)
        {
            Collider2D.enabled = false;
        }
        if (Rigidbody2D != null)
        {
            Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            Rigidbody2D.linearVelocity = Vector2.zero;
        }
        enabled = false;
    }

    public void Trigger()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Tweener moveUp = gameObject.transform.DOMoveY(gameObject.transform.position.y + 1f, 0.25f)
            .SetEase(Ease.Linear);
        yield return moveUp.WaitForCompletion();
        if (EntityMovement != null)
        {
            EntityMovement.enabled = true;
            EntityMovement.MovementDirection = Vector2.right;
        }
        if (Collider2D != null)
        {
            Collider2D.enabled = true;
        }
        if (Rigidbody2D != null)
        {
            Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
        enabled = true;
    }
}