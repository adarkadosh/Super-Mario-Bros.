using UnityEngine;
using System.Collections;

public class StarPowerEffect : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color[] _colors;
    private int _currentColorIndex;
    private const float ColorChangeInterval = 0.5f; // Time in seconds between color changes
    private bool _isStarPowerActive;

    void Awake()
    {
        // Get the SpriteRenderer component attached to this GameObject
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the GameObject.");
        }

        // Define the colors to cycle through
        _colors = new Color[]
        {
            Color.red,
            Color.black,
            Color.green,
            Color.white // Assuming white is the original color
        };
    }

    // Method to start the star power effect
    public void StartStarPower()
    {
        if (!_isStarPowerActive)
        {
            _isStarPowerActive = true;
            StartCoroutine(CycleColors());
        }
    }

    // Method to stop the star power effect
    public void StopStarPower()
    {
        if (_isStarPowerActive)
        {
            _isStarPowerActive = false;
            StopCoroutine(CycleColors());
            ResetColor();
        }
    }

    // Coroutine to cycle through colors
    private IEnumerator CycleColors()
    {
        while (_isStarPowerActive)
        {
            // Set the sprite's color to the current color in the array
            _spriteRenderer.color = _colors[_currentColorIndex];

            // Move to the next color, looping back to the start if necessary
            _currentColorIndex = (_currentColorIndex + 1) % _colors.Length;

            // Wait for the specified interval before changing to the next color
            yield return new WaitForSeconds(ColorChangeInterval);
        }
    }

    // Reset the sprite's color to the original
    private void ResetColor()
    {
        _spriteRenderer.color = Color.white; // Assuming white is the original color
    }
}