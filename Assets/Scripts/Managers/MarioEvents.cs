using System;

public class MarioEvents : MonoSingleton<MarioEvents>
{
    public static Action OnMarioGotHit;
    public static Action OnPowerupCollected;
    public static Action OnMarioDeath;
    public static Action OnMarioGotPowerUp;
    public static Action OnGotStar;
    public static Action OnGotCoin;
    public static Action OnGotExtraLife;
    public static Action OnMarioWin;
    
    
}