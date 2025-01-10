using System;

public class GameEvents : MonoSingleton<GameEvents>
{
    public static Action<float> OnResetLevel;
    public static Action<float> FasterButtonClicked;
    public static Action<float> SlowerButtonClicked;
    public static Action<int> ChickenSpawned;
    public static Action<int> UpdateCurrentChickensText;
}
