using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Assuming you use TextMeshPro for your UI text

public class ScoreManager : MonoSingleton<ScoreManager>
{
    [Header("Combo Timing")] [SerializeField]
    private float doubleKillTime = 2f;

    // [Header("Score UI")] [SerializeField] private bool _canDoubleNextKill;
    private float _lastKillTime;

    [Header("Popup Settings")] [SerializeField]
    private ScoreFactory scoreFactory;

    private int _currentScore = 0;
    private int _bestScore;

    private const string BestScoreKey = "BestScore"; // Key for PlayerPrefs

    private void Start()
    {
        // Load Best Score from PlayerPrefs
        _bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        // Initialize UI
        GameEvents.OnScoreChanged?.Invoke(_currentScore);
        GameEvents.OnBestScoreChanged?.Invoke(_bestScore);
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyHit += OnEnemyKilled;
        GameEvents.OnEventTriggered += AddScore;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyHit -= OnEnemyKilled;
        GameEvents.OnEventTriggered -= AddScore;
    }
    

    private void OnEnemyKilled(ScoresSet amount, Vector3 popupPosition = default)
    {
        // 1. Get the base score for the enemy type
        int intAmount = (int)amount;

        // 2. Check if the kill is a double
        var isDouble = Time.time - _lastKillTime < doubleKillTime;
        if (isDouble)
        {
            intAmount *= 2;
        }

        // 3. Add the score
        AddScore((ScoresSet)intAmount, popupPosition);

        // 4. Update the last kill time
        _lastKillTime = Time.time;
    }

    private void AddScore(ScoresSet amount, Vector3 popupPosition = default)
    {
        // Add to the total score
        _currentScore += (int)amount;
        
        // Check if the current score is a new best score
        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;

            // Save the new best score in PlayerPrefs
            PlayerPrefs.SetInt(BestScoreKey, _bestScore);
            PlayerPrefs.Save();

            // Invoke event to update UI or handle best score changes
            GameEvents.OnBestScoreChanged?.Invoke(_bestScore);
        }

        // Update the UI text
        GameEvents.OnScoreChanged?.Invoke(_currentScore);

        // Show a popup text at the given position
        if (scoreFactory != null && popupPosition != default)
        {
            var scorePopUp = scoreFactory.Spawn((int)amount);
            scorePopUp.transform.position = popupPosition + Vector3.up;
        }
    }
    
    public int GetBestScore()
    {
        return _bestScore;
    }

    public void ResetBestScore()
    {
        _bestScore = 0;
        PlayerPrefs.DeleteKey(BestScoreKey);
        GameEvents.OnBestScoreChanged?.Invoke(_bestScore);
    }
}