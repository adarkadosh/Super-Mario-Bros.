using System;

public class MarioEvents : MonoSingleton<MarioEvents>
{
    public static Action OnMarioGotHit;
    public static Action OnMarioGotPowerUp;
    public static Action OnGotStar;
    public static Action OnGotExtraLife;
    public static Action OnStarCollected;
    public static Action OnMarioDeath;
    public static Action<int> OnGotCoin;
    public static Action OnMarioWin;
}