using System;

public class MarioEvents : MonoSingleton<MarioEvents>
{
    public static Action OnMarioGotHit;
    public static Action<PowerUpType> OnMarioGotPowerUp;
    public static Action OnGotStar;
    public static Action OnEnterSmallMario;
    public static Action OnEnterBigMario;
    public static Action OnEnterFireMario;
    public static Action OnGotExtraLife;
    public static Action OnStarCollected;
    public static Action OnMarioDeath;
    public static Action<int> OnGotCoin;
    public static Action OnMarioWin;
}