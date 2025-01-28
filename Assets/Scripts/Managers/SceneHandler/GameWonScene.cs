using UnityEngine;

public class GameWonScene : MonoBehaviour
{
    // i want that if i press Enter key it will load the next scene
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Load the next scene
            SceneTransitionManager.Instance.TransitionToScene(SceneName.MainMenu);
        }
    }
}
