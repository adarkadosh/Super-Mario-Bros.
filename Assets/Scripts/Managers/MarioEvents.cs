using System;
using Mario;
using PowerUps;
using UnityEngine;

public class MarioEvents : MonoSingleton<MarioEvents>
{
    public static Action OnMarioReachedFlagPole;
    public static Action OnMarioGotHit;
    public static Action<PowerUpType> OnMarioGotPowerUp;
    public static Action<MarioState> OnMarioStateChange;
    // public static Action<int> OnGotCoin;
    // public static Action OnGotExtraLife;
    public static Action OnStarCollected;
    public static Action OnMarioDeath;
    public static Action OnMarioWin;
    public static Action OnMarioEnteredPipe;
}