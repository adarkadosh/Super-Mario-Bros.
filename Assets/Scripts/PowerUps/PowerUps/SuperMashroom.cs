using System.Collections;
using DG.Tweening;
using PowerUps;
using UnityEngine;


public class SuperMashroom : GenericPowerUp
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a powerup");
            MarioEvents.OnMarioGotPowerUp?.Invoke(PowerUpType.SuperMashroom);
            PowerUpFactory.Instance.ReturnPowerUp(this);

        }
    }
    
}