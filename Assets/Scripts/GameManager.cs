using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public int World { get; private set; }
    public int Level { get; private set; }
    public int Coins { get; private set; }
    public int Lives { get; private set; }
    
    private void Start()
    {
        World = 1;
        Level = 1;
        Coins = 0;
        Lives = 3;
    }

    private void OnEnable()
    {
        GameEvents.OnResetLevel += ResetLevel;
    }
    
    private void OnDisable()
    {
        GameEvents.OnResetLevel -= ResetLevel;
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
}
