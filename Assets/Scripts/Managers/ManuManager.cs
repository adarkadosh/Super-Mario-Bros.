using UnityEngine;

public class ManuManager : MonoSingleton<ManuManager>
{
    
    public void StartGame()
    {
        Debug.Log("Game started");
    }

    public void QuitGame()
    {
        Debug.Log("Game quit");
    }
    
    public void PauseGame()
    {
        Debug.Log("Game paused");
    }
    
    public void ResumeGame()
    {
        Debug.Log("Game resumed");
    }
    
    public void RestartGame()
    {
        Debug.Log("Game restarted");
    }
    
    
    
}