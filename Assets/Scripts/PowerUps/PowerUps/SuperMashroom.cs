using System.Collections;
using DG.Tweening;
using PowerUps;
using UnityEngine;


public class SuperMashroom : GenericPowerUp
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a powerup");
            MarioEvents.OnMarioGotPowerUp?.Invoke();
            SuperMashroomPool.Instance.Return(this);
        }
    }
    
}