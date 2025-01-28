using UnityEngine;

public class StartGameScene : MonoBehaviour
{
    private void Start()
    {
        SceneTransitionManager.Instance.TransitionToScene(SceneName.MainMenu);
    }
    
}