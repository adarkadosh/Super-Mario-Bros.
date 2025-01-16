using System;

public class MarioEvents : MonoSingleton<MarioEvents>
{
    public static Action OnMarioGotHit;
    public static Action<PowerUpType> OnMarioGotPowerUp;
    public static Action<MarioState> OnMarioStateChange;
    public static Action OnStarCollected;
    public static Action OnGotExtraLife;
    public static Action OnMarioDeath;
    public static Action<int> OnGotCoin;
    public static Action OnMarioWin;
}