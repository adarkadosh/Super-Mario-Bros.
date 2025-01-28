using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviour
{
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
    private int initTime;
    [SerializeField] private bool shouldGameEnd;
    
    public int BestScore { get; private set; }
    
    private int _world;
    private int _level;
    private int _coins;
    private int _lives;
    
    private GameOverType _gameOverType;
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Additional initialization...
    }
    
    
    private void Start()
    {
        initTime = initTimeForRound;
        Application.targetFrameRate = 60;
        _world = 1;
        _level = 1;
        _coins = 0;
        _lives = initLives;
        
        GameEvents.OnWorldChanged?.Invoke(_world, _level);
        GameEvents.OnCoinsChanged?.Invoke(_coins);
        GameEvents.OnLivesChanged?.Invoke(_lives);
        GameEvents.OnTimeChanged?.Invoke(initTime);
        SoundFXManager.Instance.ChangeBackgroundMusic(backgroundMusic);
    }
    
    private void OnEnable()
    {
        GameEvents.OnResetLevel += ResetLevel;
        GameEvents.OnCoinCollected += AddCoin;
        GameEvents.OnGotExtraLife += AddLife;
        GameEvents.OnTimeUp += TimeUp;
        MarioEvents.OnMarioDeath += OnMarioDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnResetLevel -= ResetLevel;
        GameEvents.OnCoinCollected -= AddCoin;
        GameEvents.OnGotExtraLife -= AddLife;
        GameEvents.OnTimeUp -= TimeUp;
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
        
        _lives--;
        
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

    // public void HandleBackFromGameOver()
    // {
    //     switch (_gameOverType)
    //     {
    //         case GameOverType.GameOver:
    //             SceneTransitionManager.Instance.TransitionToScene(SceneName.StartScene);
    //             break;
    //         case GameOverType.TimeUp:
    //             SceneTransitionManager.Instance.TransitionToScene(SceneName.LivesIndicatorScene);
    //             break;
    //     }
    // }

    private void AddCoin(int coins)
    {
        _coins++;
        GameEvents.OnCoinsChanged?.Invoke(_coins);
        if (_coins >= maxCoinsToGetLife)
        {
            AddLife();
            _coins = 0;
        }
    }
    
    void Update()
    {
        TimeHandler();
    }

    private void TimeHandler()
    {
        timer -= Time.deltaTime;  // Decrement by real-time seconds
        
        if (initTime <= 100)
        {
            SoundFXManager.Instance.ChangeBackgroundMusic(timeRunningOutMusic);
        }

        if (timer <= 0)
        {
            initTime--;  // Reduce by 1 second
            timer = 0.5f;  // Reset timer to 1 second
        }

        if (initTime <= 0)
        {
            if(!shouldGameEnd) return;
            GameEvents.OnTimeUp?.Invoke();
        }
        
        GameEvents.OnTimeChanged?.Invoke(initTime);
    }

    private void AddLife()
    {
        _lives++;
    }
    
    public void SetBestScore(int score)
    {
        BestScore = score;
    }
}