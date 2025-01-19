using UnityEngine;
using System.Collections;

public class FlashTransparency : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private bool _isFlashing;
    private const float FlashInterval = 0.033f; // Approximately 2 frames at 60 FPS

    void Awake()
    {
        // Get the SpriteRenderer component attached to this GameObject
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the GameObject.");
        }
    }

    // Method to start the flashing effect
    public void StartFlashing()
    {
        if (!_isFlashing && _spriteRenderer != null)
        {
            _isFlashing = true;
            StartCoroutine(FlashCoroutine());
        }
    }

    // Method to stop the flashing effect
    public void StopFlashing()
    {
        if (_isFlashing && _spriteRenderer != null)
        {
            _isFlashing = false;
            StopCoroutine(FlashCoroutine());
            SetSpriteAlpha(1f); // Ensure sprite is fully opaque when stopping
        }
    }

    // Coroutine to handle the flashing effect
    private IEnumerator FlashCoroutine()
    {
        while (_isFlashing)
        {
            // Toggle the alpha value between 1 and 0.5
            float newAlpha = Mathf.Approximately(_spriteRenderer.color.a, 1f) ? 0f : 1f;
            // Debug.Log("newAlpha: " + newAlpha);
            SetSpriteAlpha(newAlpha);

            // Wait for the specified interval
            yield return new WaitForSeconds(FlashInterval);
        }
    }

    // Helper method to set the sprite's alpha value
    private void SetSpriteAlpha(float alpha)
    {
        Color color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
    }
}