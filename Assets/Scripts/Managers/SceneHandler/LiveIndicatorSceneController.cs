using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LiveIndicatorSceneController : MonoBehaviour
{
    [SerializeField] private ScoreData scoreData;
    [Header("UI Settings")] 
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private float delayBeforeLoadingMainGame = 3f;
    private void Start()
    {
        livesText.text = $"{scoreData.LivesRemaining:D1}";
        stageText.text = $"{scoreData.World:D1}-{scoreData.Level:D1}";
        StartCoroutine(LoadMainGameAfterDelay());
    }

    IEnumerator LoadMainGameAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoadingMainGame);  // Wait for 3 seconds
        SceneTransitionManager.Instance.TransitionToScene(SceneName.SuperMarioBrosMain);
        GameEvents.OnGameStarted?.Invoke();
        // SoundFXManager.Instance.ChangeBackgroundMusic(GameManager.Instance.BackgroundMusic);
    }
}