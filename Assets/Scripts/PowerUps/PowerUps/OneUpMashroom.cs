using PowerUps;
using UnityEngine;

public class OneUpMashroom : GenericPowerUp
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a OneUp");
            GameEvents.OnGotExtraLife?.Invoke();
            OneUpMashroomPool.Instance.Return(this);
        }
    }
}