using UnityEngine;

public enum SceneName
{
    StartScene,
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
            case SceneName.StartScene:
                return "Start Scene";
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