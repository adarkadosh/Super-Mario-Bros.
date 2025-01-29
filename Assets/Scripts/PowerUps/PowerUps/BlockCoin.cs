// BlockCoin.cs
using System.Collections;
using Managers;
using UnityEngine;

namespace PowerUps.PowerUps
{
    public class BlockCoin : MonoBehaviour, IPoolable
    {
        [SerializeField] private int coinsToGive = 1;
        [SerializeField] private float animationHeight = 3.5f;
        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private ScoresSet scoreSet = ScoresSet.TwoHundred;

        public void Trigger()
        {
            GameEvents.OnCoinCollected?.Invoke(coinsToGive);
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            yield return Extensions.AnimatedBlockGotHit(gameObject, animationHeight, animationDuration,
                animationHeight / 2);
            GameEvents.OnEventTriggered?.Invoke(scoreSet, transform.position);
            PowerUpFactory.Instance.ReturnBlockCoin(this);
        }

        public void Reset()
        {
            transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }
    
        public void Kill()
        {
            PowerUpFactory.Instance.ReturnBlockCoin(this);
        }
    }
}