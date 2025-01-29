using System.Collections;
using UnityEngine;


public class GameManager : MonoSuperSingleton<GameManager>
{
    [Header("Score Data")]
    [SerializeField] private ScoreData scoreData;
    
    [Header("Sound Settings")] 
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip levelCompleteMusic;
    [SerializeField] private AudioClip timeRunningOutMusic;
    [SerializeField] private AudioClip marioDeathMusic;

    [Header("Game Settings")] [SerializeField]
    private int maxCoinsToGetLife = 100;

    [SerializeField] private int initLives = 3;
    [SerializeField] private float delayBeforeTransition = 3f;
    [SerializeField] private float timer = 0.5f;  // Tracks time in seconds
    [SerializeField] private int initTimeForRound = 400; 
    private int _initTime;
    [SerializeField] private bool shouldGameEnd;
    
    private int _world;
    private int _level;
    private int _coins;
    private int _lives;
    
    private GameOverType _gameOverType;
    private bool _gameActive;
    
    private void Start()
    {
        if (scoreData == null)
        {
            Debug.LogError("ScoreData is not assigned in GameManager.");
            return;
        }

        scoreData.ResetScoreData();
        InitializeGameState();
    }

    private void InitializeGameState()
    {
        _world = scoreData.initialWorld;
        _level = scoreData.initialLevel;
        _coins = scoreData.CoinsCollected;
        _lives = scoreData.LivesRemaining;
        _initTime = scoreData.levelTime;

        // Trigger events to update UI
        GameEvents.OnWorldChanged?.Invoke(_world, _level);
        GameEvents.OnCoinsChanged?.Invoke(_coins);
        GameEvents.OnLivesChanged?.Invoke(_lives);
        GameEvents.OnTimeChanged?.Invoke(_initTime);
    }
    
    private void OnEnable()
    {
        GameEvents.OnResetLevel += ResetLevel;
        GameEvents.OnCoinCollected += AddCoin;
        GameEvents.OnGotExtraLife += AddLife;
        GameEvents.OnGameStarted += OnGameStart;
        GameEvents.OnTimeUp += TimeUp;
        GameEvents.OnGameRestart += OnGameRestart;
        GameEvents.OnGameWon += OnGameWon;
        MarioEvents.OnMarioDeath += OnMarioDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnResetLevel -= ResetLevel;
        GameEvents.OnCoinCollected -= AddCoin;
        GameEvents.OnGotExtraLife -= AddLife;
        GameEvents.OnTimeUp -= TimeUp;
        GameEvents.OnGameStarted -= OnGameStart;
        GameEvents.OnGameRestart -= OnGameRestart;
        GameEvents.OnGameWon -= OnGameWon;
        MarioEvents.OnMarioDeath -= OnMarioDeath;
    }

    private void TimeUp()
    {
        Debug.Log("Time's Up!");
        MarioEvents.OnMarioDeath?.Invoke();
        _gameOverType = GameOverType.TimeUp;
        GameEvents.OnGameLost?.Invoke(_gameOverType);
        SceneTransitionManager.Instance.TransitionToScene(SceneName.EndGameScene);
    }

    private void OnMarioDeath()
    {
        Debug.Log("Mario Died. Starting Death Sequence...");
        SoundFXManager.Instance.ChangeBackgroundMusic(marioDeathMusic);
        GameEvents.FreezeAllCharacters?.Invoke(5f);
        ResetLevel(2.7f);
    }

    public void ResetLevel(float delay)
    {
        StartCoroutine(ResetLevelCoroutine(delay));
    }

    private IEnumerator ResetLevelCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _gameActive = false;
        
        _lives--;
        scoreData.UpdateLives(_lives);
        GameEvents.OnLivesChanged?.Invoke(_lives);
        
        if (_lives <= 0)
        {
            _gameOverType = GameOverType.GameOver;
            GameEvents.OnGameLost?.Invoke(_gameOverType);
            SoundFXManager.Instance.ChangeBackgroundMusic(gameOverMusic);
            SceneTransitionManager.Instance.TransitionToScene(SceneName.EndGameScene);
        }
        else
        {
            SceneTransitionManager.Instance.TransitionToScene(SceneName.LivesIndicatorScene);
        }
        GameEvents.OnLivesChanged?.Invoke(_lives);
    }
    

    private void AddCoin(int coins)
    {
        scoreData.UpdateCoins(coins);
        GameEvents.OnCoinsChanged?.Invoke(scoreData.CoinsCollected);

        if (scoreData.CoinsCollected >= scoreData.coinsToExtraLife)
        {
            AddLife();
            scoreData.UpdateCoins(-scoreData.coinsToExtraLife); // Reset coins
        }
    }
    
    void Update()
    {
        if (_gameActive) TimeHandler();
    }

    private void TimeHandler()
    {
        timer -= Time.deltaTime;  // Decrement by real-time seconds
        
        if (_initTime <= 100)
        {
            SoundFXManager.Instance.ChangeBackgroundMusic(timeRunningOutMusic);
        }

        if (timer <= 0)
        {
            _initTime--;  // Reduce by 1 second
            timer = 0.5f;  // Reset timer to 1 second
        }

        if (_initTime <= 0)
        {
            if(!shouldGameEnd) return;
            GameEvents.OnTimeUp?.Invoke();
        }
        
        GameEvents.OnTimeChanged?.Invoke(_initTime);
    }

    private void AddLife()
    {
        scoreData.UpdateLives(scoreData.LivesRemaining + 1);
        GameEvents.OnLivesChanged?.Invoke(scoreData.LivesRemaining);
        // GameEvents.OnGotExtraLife?.Invoke();
    }
    
    private void OnGameRestart()
    {
        scoreData.ResetScoreData();
        InitializeGameState();
    }
    
    private void OnGameStart()
    {
        _gameActive = true;
        _initTime = initTimeForRound;
        SoundFXManager.Instance.ChangeBackgroundMusic(backgroundMusic);
    }

    private void OnGameWon()
    {
        _gameActive = false;
        SoundFXManager.Instance.ChangeBackgroundMusic(levelCompleteMusic);
        SceneTransitionManager.Instance.TransitionToScene(SceneName.WonGameScene);
    }
}