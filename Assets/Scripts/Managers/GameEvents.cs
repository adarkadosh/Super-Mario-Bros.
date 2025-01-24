using System;
using UnityEngine;

public class GameEvents : MonoSingleton<GameEvents>
{
    public static Action OnLevelCompleted;
    public static Action<ScoresSet, Vector3> OnEnemyHit;
    public static Action<ScoresSet, Vector3> OnEventTriggered;
    public static Action<float> OnResetLevel;
    public static Action OnPlayerDeath;
    public static Action OnPlayerRespawn;
    public static Action<int> OnCoinCollected;
    public static Action OnGotExtraLife;
    public static Action<float> FreezeAllCharacters;
}