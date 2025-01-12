using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour, IPoolable
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
        Tweener moveUp = gameObject.transform.DOMoveY(gameObject.transform.position.y + 1f, 0.25f)
            .SetEase(Ease.Linear);
        yield return moveUp.WaitForCompletion();
        GetComponent<EntityMovement>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<EntityMovement>().MovementDirection = Vector2.right;
    }
}