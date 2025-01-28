using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ScoreData scoreData;
    
    [Header("UI Settings")] 
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText; 
    
    private void Start()
    {
        UpdateScoreUI(scoreData.CurrentScore);
        UpdateCoinsUI(scoreData.CoinsCollected);
        UpdateTimeUI(scoreData.TimeRemaining);
        UpdateWorldUI(scoreData.World, scoreData.Level);
    }
    private void OnEnable()
    {
        GameEvents.OnScoreChanged += UpdateScoreUI;
        GameEvents.OnCoinsChanged += UpdateCoinsUI;
        GameEvents.OnTimeChanged += UpdateTimeUI;
        GameEvents.OnWorldChanged += UpdateWorldUI;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= UpdateScoreUI;
        GameEvents.OnCoinsChanged -= UpdateCoinsUI;
        GameEvents.OnTimeChanged -= UpdateTimeUI;
        GameEvents.OnWorldChanged -= UpdateWorldUI;
    }
    
    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"{score:D6}";
        
    }
    
    private void UpdateCoinsUI(int coins)
    {
        if (coinText != null) 
            coinText.text = $"{coins:D2}";
    }
    
    private void UpdateTimeUI(int time)
    {
        if (timeText != null) 
            timeText.text = $"{time:D3}";
    }
    
    private void UpdateWorldUI(int world, int level)
    {
        if (stageText != null) 
            stageText.text = $"{world:D1}-{level:D1}";
    }
    

}