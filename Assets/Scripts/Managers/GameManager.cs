using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private int maxCoinsToGetLife = 100;
    public int World { get; private set; }
    public int Level { get; private set; }
    public int Coins { get; private set; }
    public int Lives { get; private set; }

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
        MarioEvents.OnMarioDeath += OnMarioDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnResetLevel -= ResetLevel;
        GameEvents.OnCoinCollected -= AddCoin;
        GameEvents.OnGotExtraLife -= AddLife;
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
        if (Coins >= maxCoinsToGetLife)
        {
            AddLife();
            Coins = 0;
        }
    }

    public void AddLife()
    {
        Lives++;
    }
}