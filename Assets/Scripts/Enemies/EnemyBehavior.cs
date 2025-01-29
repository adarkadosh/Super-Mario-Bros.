using System.Collections;
using Managers;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyBehavior : MonoBehaviour
    {
        [SerializeField] public ScoresSet enemyScore = ScoresSet.OneHundred;
        [SerializeField] public ScoresSet enemyHitScore = ScoresSet.FiveHundred;
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
            if (other.gameObject.CompareTag("Player") && !gameObject.CompareTag($"ShellKoopa") &&
                gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Vector2 direction = transform.position - other.transform.position;
                if (Vector2.Dot(direction.normalized, Vector2.down) < 0.35f)
                {
                    MarioEvents.OnMarioGotHit?.Invoke();
                }
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag($"LethalShell"))
            {
                if (!gameObject.CompareTag(other.gameObject.tag))
                {
                    GameEvents.OnEventTriggered?.Invoke(enemyHitScore, transform.position);
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

        public abstract void GotHit();

        public void Kill()
        {
            // Return the enemy to the factory
            EnemyFactory.Instance.Return(this);
        }
    }
}