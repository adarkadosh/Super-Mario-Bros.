using UnityEngine;

public enum SceneName
{
    StartGame,
    MainMenu,
    LivesIndicatorScene,
    SuperMarioBrosMain,
    EndGameScene,
    WonGameScene
}

public static class SceneHelper
{
    public static string GetSceneName(SceneName scene)
    {
        switch (scene)
        {
            case SceneName.StartGame:
                return "Start Game";
            case SceneName.MainMenu:
                return "Main Menu";
            case SceneName.LivesIndicatorScene:
                return "Lives Indicator Scene";
            case SceneName.SuperMarioBrosMain:
                return "Super Mario Bros. (Main) Scene";
            case SceneName.EndGameScene:
                return "EndGame Scene";
            case SceneName.WonGameScene:
                return "Won Game Scene";
            default:
                Debug.LogError("Undefined scene!");
                return string.Empty;
        }
    }
}