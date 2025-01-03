using System;

public class ResetEvents : MonoSingleton<ResetEvents>
{
    public static Action ResetSpawnRate;
    public static Action DestroyAllChickens;
    public static Action ResetCameraPosition;
}