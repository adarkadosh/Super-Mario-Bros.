using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Assuming you use TextMeshPro for your UI text

public class ScoreManager : MonoSingleton<ScoreManager>
{
    [Header("Combo Timing")] [SerializeField]
    private float doubleKillTime = 2f;

    [Header("Score UI")] [SerializeField]
    private TextMeshProUGUI scoreText; // Link to a UI TextMeshPro element in Inspector


    private float lastKillTime;
    private bool canDoubleNextKill = false; // If true, the next kill is doubled

    [Header("Popup Settings")] [SerializeField]
    private ScoreFactory scoreFactory; // A prefab for the popup text (optional)

    private int _currentScore = 0;

    private void Start()
    {
        // Initialize UI
        UpdateScoreUI();
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


    public void OnEnemyKilled(ScoresSet amount, Vector3 popupPosition = default)
    {
        // 1. Get the base score for the enemy type
        int intAmount = (int)amount;

        // 2. Check if the kill is a double
        var isDouble = Time.time - lastKillTime < doubleKillTime;
        if (isDouble)
        {
            intAmount *= 2;
            canDoubleNextKill = false;
        }
        else
        {
            canDoubleNextKill = true;
        }

        // 3. Add the score
        AddScore((ScoresSet)intAmount, popupPosition);

        // 4. Update the last kill time
        lastKillTime = Time.time;
    }

    public void AddScore(ScoresSet amount, Vector3 popupPosition = default)
    {
        // 1. Add to the total score
        _currentScore += (int)amount;

        // 2. Update the UI text
        UpdateScoreUI();

        // 3. Optionally show a popup text at the given position
        if (scoreFactory != null && popupPosition != default)
        {
            var scorePopUp = scoreFactory.Spawn((int)amount);
            scorePopUp.transform.position = popupPosition + Vector3.up;
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{_currentScore:D6}";
        }
    }
}