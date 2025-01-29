using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUBCoinAnimation : MonoBehaviour
{
    public Image coinImage;          // Reference to the UI Image component
    public Sprite[] coinSprites;     // Array to hold the coin animation sprites
    public float animationSpeed = 0.2f; // Speed of sprite switching (seconds per frame)

    private int _currentFrame;

    private void Start()
    {
        if (coinSprites.Length > 0)
        {
            StartCoroutine(AnimateCoin());
        }
    }

    IEnumerator AnimateCoin()
    {
        while (true)
        {
            // Change the sprite to the next one in the array
            coinImage.sprite = coinSprites[_currentFrame];

            // Move to the next frame, loop back to 0 if at the end
            _currentFrame = (_currentFrame + 1) % coinSprites.Length;

            // Wait before changing to the next frame
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}