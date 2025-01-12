using System;

public class GameEvents : MonoSingleton<GameEvents>
{
    public static Action<float> OnResetLevel;
    public static Action OnPlayerDeath;
    public static Action OnPlayerRespawn;
    public static Action<int> OnCoinCollected;
}