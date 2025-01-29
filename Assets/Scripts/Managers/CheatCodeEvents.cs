using System;

public class CheatCodeEvents : MonoSingleton<CheatCodeEvents>
{
    public static Action ResetSpawnRate;
    public static Action DestroyAllChickens;
    public static Action ResetCameraPosition;
}