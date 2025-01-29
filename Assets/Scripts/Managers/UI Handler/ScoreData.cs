using UnityEngine;
using System;
using Managers;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game Data/ScoreData")]
public class ScoreData : ScriptableObject
{
    public int initialLives = 3;
    public int levelTime = 400;
    public int coinsToExtraLife = 100;
    public int coinsToExtraLifeValue = 1;
    public int initialLevel = 1;
    public int initialWorld = 1;
    
    
    public int _bestScore;
    public int _currentScore;
    public int _coinsCollected;
    public int _livesRemaining;
    public int _timeRemaining;
    public int _level;
    public int _world;
    
    public int BestScore => _bestScore;
    public int CurrentScore => _currentScore;
    public int CoinsCollected => _coinsCollected;
    public int LivesRemaining => _livesRemaining;
    public int TimeRemaining => _timeRemaining;
    public int Level => _level;
    public int World => _world;

    
    void OnEnable()
    {
        GameEvents.OnScoreChanged += UpdateScore;
        GameEvents.OnCoinCollected += UpdateCoins;
        GameEvents.OnTimeChanged += UpdateTime;
        GameEvents.OnWorldChanged += UpdateLevel;
        GameEvents.OnLivesChanged += UpdateLives;
    }
    
    void OnDisable()
    {
        GameEvents.OnScoreChanged -= UpdateScore;
        GameEvents.OnCoinCollected -= UpdateCoins;
        GameEvents.OnTimeChanged -= UpdateTime;
        GameEvents.OnWorldChanged -= UpdateLevel;
        GameEvents.OnLivesChanged -= UpdateLives;
    }

    public void ResetScoreData()
    {
        _currentScore = 0;
        _coinsCollected = 0;
        _livesRemaining = initialLives;
        _timeRemaining = 400;
        _level = 1;
        _world = 1;
    }


    private void UpdateScore(int score)
    {
        _currentScore = score;
        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;
        }
    }
    
    public void UpdateCoins(int coins)
    {
        _coinsCollected += coins;
        if (_coinsCollected >= 100)
        {
            _coinsCollected -= 100;
            _livesRemaining++;
        }
    }

    private void UpdateTime(int time)
    {
        _timeRemaining += time;
    }

    private void UpdateLevel(int level, int world)
    {
        _level = level;
        _world = world;
    }
    
    public void UpdateLives(int lives)
    {
        _livesRemaining = lives;
    }
    
    
}