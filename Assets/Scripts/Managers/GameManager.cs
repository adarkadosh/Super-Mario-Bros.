using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Sound Settings")] [SerializeField]
    private AudioClip backgroundMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip levelCompleteMusic;
    [SerializeField] private AudioClip timeRunningOutMusic;
    [SerializeField] private AudioClip marioDeathMusic;
    
    
    [Header("UI Settings")] [SerializeField]
    private TextMeshProUGUI coinText;

    [SerializeField] private TextMeshProUGUI worldText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Game Settings")] [SerializeField]
    private int maxCoinsToGetLife = 100;

    [SerializeField] private int maxLives = 3;
    [SerializeField] private int initTime = 400;
    [SerializeField] private bool _shouldGameEnd;
    public int World { get; private set; }
    public int Level { get; private set; }
    public int Coins { get; private set; }
    public int Lives { get; private set; }
    
    private float timer = 0.5f;  // Tracks time in seconds

    private void Start()
    {
        Application.targetFrameRate = 60;
        World = 1;
        Level = 1;
        Coins = 0;
        Lives = 3;
        
    }
    

    private void OnEnable()
    {
        GameEvents.OnResetLevel += ResetLevel;
        GameEvents.OnCoinCollected += AddCoin;
        GameEvents.OnGotExtraLife += AddLife;
        GameEvents.OnTimeUp += TimeUp;
        MarioEvents.OnMarioDeath += OnMarioDeath;
    }

    private void TimeUp()
    {
        // Freeze all characters for 3 seconds
        GameEvents.FreezeAllCharacters?.Invoke(3f);

        // Reset the level after 3 seconds
        ResetLevel(3f);
    }

    private void OnDisable()
    {
        GameEvents.OnResetLevel -= ResetLevel;
        GameEvents.OnCoinCollected -= AddCoin;
        GameEvents.OnGotExtraLife -= AddLife;
        GameEvents.OnTimeUp -= TimeUp;
        MarioEvents.OnMarioDeath -= OnMarioDeath;
    }

    private void OnMarioDeath()
    {
        // Freeze all characters for 3 seconds
        GameEvents.FreezeAllCharacters?.Invoke(3f);

        // Reset the level after 3 seconds
        ResetLevel(3f);
    }

    public void ResetLevel(float delay)
    {
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        Lives--;
        if (Lives <= 0)
        {
            GameOver();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void AddCoin(int coins)
    {
        Coins++;
        coinText.text = $"{Coins:D2}";
        if (Coins >= maxCoinsToGetLife)
        {
            AddLife();
            Coins = 0;
        }
    }
    
    void Update()
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
            if(!_shouldGameEnd) return;
            GameEvents.OnTimeUp?.Invoke();
            SceneManager.LoadScene("GameOver");
        }
        
        // coinText.text = $"{Coins:2D}";
        worldText.text = $"{World:D1}-{Level:D1}";
        timeText.text = $"{initTime:D3}";
    }

    private void AddLife()
    {
        Lives++;
    }
}