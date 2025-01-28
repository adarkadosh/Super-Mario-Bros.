using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LiveIndicatorSceneController : MonoBehaviour
{
    [Header("UI Settings")] 
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private float delayBeforeLoadingMainGame = 3f;
    private void Start()
    {
        StartCoroutine(LoadMainGameAfterDelay());
    }
    
    private void OnEnable()
    {
        GameEvents.OnLivesChanged += UpdateLivesUI;
        GameEvents.OnWorldChanged += UpdateWorldUI;
    }
    
    private void OnDisable()
    {
        GameEvents.OnLivesChanged -= UpdateLivesUI;
        GameEvents.OnWorldChanged -= UpdateWorldUI;
    }
    
    private void UpdateLivesUI(int lives)
    {
        if (livesText != null)
            livesText.text = $"{lives:D1}";
    }
    
    private void UpdateWorldUI(int world, int level)
    {
        if (stageText != null)
            stageText.text = $"{world:D1}-{level:D1}";
    }

    IEnumerator LoadMainGameAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoadingMainGame);  // Wait for 3 seconds
        SceneTransitionManager.Instance.TransitionToScene(SceneName.SuperMarioBrosMain);
    }
}