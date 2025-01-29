using UnityEngine;

namespace Enemies.Goomba
{
    public class GoombaBehavior : EnemyBehavior, IPoolable
    {
        private static readonly int Squished = Animator.StringToHash("IsSquished");
        [SerializeField] private ScoresSet goombaScore = ScoresSet.OneHundred;

        public override void GotHit()
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero; // Reset velocity to stop movement
            Animator.SetBool(Squished, true);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<EntityMovement>().enabled = false;
        }

        public void Reset()
        {
            var rb = GetComponent<Rigidbody2D>();
            var entityMovement = GetComponent<EntityMovement>();
            var spriteRenderer = GetComponent<SpriteRenderer>();
            GetComponent<Collider2D>().enabled = true;
            Animator.enabled = true;
            Animator.SetBool(Squished, false);

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero; // Reset velocity
            rb.angularVelocity = 0f;    // Reset angular velocity

            entityMovement.enabled = true;
            entityMovement.MovementDirection = Vector2.left; // Reset movement direction
            spriteRenderer.flipY = false; // Reset sprite flip
            spriteRenderer.sortingLayerName = "Enemies"; // Reset sorting layer
        }
    
        protected override void DeathSequenceAnimation()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Death";
            spriteRenderer.flipY = true;
        }
    }
}
