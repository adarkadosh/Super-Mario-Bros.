using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameOverType
{
    GameOver,
    TimeUp
}

public class GameOverScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    private GameOverType _gameOverType;


    void Start()
    {
        StartCoroutine(BackFromGameOver());
    }

    private IEnumerator BackFromGameOver()
    {
        yield return new WaitForSeconds(3f);
        switch (_gameOverType)
        {
            case GameOverType.GameOver:
                SceneTransitionManager.Instance.TransitionToScene(SceneName.MainMenu);
                // GameEvents.OnGameRestart?.Invoke();
                break;
            case GameOverType.TimeUp:
                SceneTransitionManager.Instance.TransitionToScene(SceneName.LivesIndicatorScene);
                break;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnGameLost += UpdateGameOverUI;
    }

    private void OnDisable()
    {
        GameEvents.OnGameLost -= UpdateGameOverUI;
    }

    private void UpdateGameOverUI(GameOverType text)
    {
        _gameOverType = text;
        if (gameOverText != null)
            switch (text)
            {
                case GameOverType.GameOver:
                    gameOverText.text = "Game  Over";
                    break;
                case GameOverType.TimeUp:
                    gameOverText.text = "Time  Up";
                    break;
            }
    }
}