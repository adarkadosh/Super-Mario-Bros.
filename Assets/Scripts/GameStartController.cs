using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadMainGameAfterDelay());
    }

    IEnumerator LoadMainGameAfterDelay()
    {
        yield return new WaitForSeconds(3f);  // Wait for 3 seconds
        SceneManager.LoadScene("Super Mario Bros.");  // Load the main game
    }
}