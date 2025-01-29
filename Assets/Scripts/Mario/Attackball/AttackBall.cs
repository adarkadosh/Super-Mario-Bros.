using System.Collections;
using Mario.Attackball.Attackballs;
using Mario.Attackball.Pools;
using UnityEngine;

namespace Mario.Attackball
{
    public class AttackBall : MonoBehaviour, IPoolable
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 2f;
    
        internal Animator Animator;
        internal Rigidbody2D Rigidbody;
        internal Collider2D Collider;
        internal EntityMovement EntityMovement;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Collider = GetComponent<Collider2D>();
            EntityMovement = GetComponent<EntityMovement>();
        }

        private IEnumerator WaitAndDestroy(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Animator.SetTrigger("Explode");
        }

        private void OnEnable()
        {
            StartCoroutine(WaitAndDestroy(lifetime));
        }
    
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        // private void OnTriggerEnter2D(Collider2D collision)
        // {
        //
        // }

        public void Reset()
        {
            if (Rigidbody != null)
            {
                Rigidbody.bodyType = RigidbodyType2D.Dynamic;
                // Reset velocity
                Rigidbody.linearVelocity = Vector2.zero;
                Rigidbody.angularVelocity = 0f;
            }

            // Reset position and rotation
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            // Reset animator to default state
            if (Animator != null)
            {
                Animator.ResetTrigger("Explode");
                Animator.Play("Idle", -1, 0f); // Replace "IdleState" with your default state
            }
        
            if (EntityMovement != null)
            {
                EntityMovement.enabled = true;
                EntityMovement.MovementDirection = Vector2.right;
            }

            // Reset collider
            if (Collider != null)
            {
                Collider.enabled = true;
            }

            // // Stop any ongoing coroutines
            // StopAllCoroutines();
        }
    
        public void SetDirection(Vector2 dir)
        {
            if (Rigidbody != null)
            {
                Rigidbody.linearVelocity = dir * speed;
            }

            EntityMovement.MovementDirection = dir;
        }

        public void Kill()
        {
            // We check if we're a Fireball or IceBall by using "is" or "as".
            if (this is Fireball fire)
            {
                // Return to the FireballPool
                FireballPool.Instance.Return(fire);
            }
            else if (this is IceBall ice)
            {
                // Return to the IceballPool
                IceballPool.Instance.Return(ice);
            }
            else
            {
                // Fallback in case you have other AttackBall subclasses
                gameObject.SetActive(false);
            }
        }

    }
}