using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PowerUps
{
    public abstract class GenericPowerUp : MonoBehaviour, IPoolable
    {
        private EntityMovement _entityMovement;
        private Collider2D _collider2D;
        private Rigidbody2D _rigidbody2D;
    
        private void Awake()
        {
            _entityMovement = GetComponent<EntityMovement>();
            _collider2D = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
    
        public void Reset()
        {
            if (_entityMovement != null)
            {
                _entityMovement.enabled = false;
            }
            if (_collider2D != null)
            {
                _collider2D.enabled = false;
            }
            if (_rigidbody2D != null)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _rigidbody2D.linearVelocity = Vector2.zero;
            }
            enabled = false;
        }

        public void Kill()
        {
            PowerUpFactory.Instance.Return(this);
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
            if (_entityMovement != null)
            {
                _entityMovement.enabled = true;
                _entityMovement.MovementDirection = Vector2.right;
            }
            if (_collider2D != null)
            {
                _collider2D.enabled = true;
            }
            if (_rigidbody2D != null)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            }
            enabled = true;
        }
    }
}