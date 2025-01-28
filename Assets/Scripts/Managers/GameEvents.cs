using System;
using UnityEngine;

public class GameEvents : MonoSingleton<GameEvents>
{
    public static Action<ScoresSet, Vector3> OnEnemyHit;
    public static Action<ScoresSet, Vector3> OnEventTriggered;

    public static Action OnLevelCompleted;
    public static Action OnTimeUp;
    public static Action<GameOverType> OnGameLost;
    public static Action OnGameWon;
    public static Action OnGameStarted;
    public static Action OnGamePaused;
    public static Action OnGameResumed;
    public static Action OnGameRestart;
    public static Action<float> OnResetLevel;


    // public static Action OnPlayerDeath;

    // public static Action OnPlayerRespawn;
    public static Action OnGotExtraLife;
    public static Action<int> OnCoinCollected;
    public static Action<float> FreezeAllCharacters;
    public static Action<int> OnScoreChanged;
    public static Action<int> OnCoinsChanged;
    public static Action<int> OnLivesChanged;
    public static Action<int> OnTimeChanged;
    public static Action<int,int> OnWorldChanged;
    public static Action<int> OnBestScoreChanged;
}