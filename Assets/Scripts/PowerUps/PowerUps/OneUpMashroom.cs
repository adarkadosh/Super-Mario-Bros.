using PowerUps;
using UnityEngine;

public class OneUpMashroom : GenericPowerUp
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected a OneUp");
            GameEvents.OnGotExtraLife?.Invoke();
            OneUpMashroomPool.Instance.Return(this);
        }
    }
}