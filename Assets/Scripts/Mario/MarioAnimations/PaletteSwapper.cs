using System.Collections;
using UnityEngine;

public class PaletteSwapper : MonoBehaviour
{
    private static readonly int HatColour = Shader.PropertyToID("_HatColour");
    private static readonly int BodyColour = Shader.PropertyToID("_BodyColour");
    private static readonly int ClothColour = Shader.PropertyToID("_ClothColour");
    private const float StarFlashInterval = 0.333f;

    [SerializeField] private Material general; // Material for the shader
    [SerializeField] private Color[] blackMarioColor;
    [SerializeField] private Color[] greenMarioColor;
    [SerializeField] private Color[] redMarioColor;
    [SerializeField] private Color[] regularMarioColor;
    [SerializeField] private Color[] fireMarioColor;

    private Color[][] _starMarioColors; // Array of star effect colors
    private SpriteRenderer _spriteRenderer;
    private bool _isStar;
    private Coroutine _starCoroutine;

    private void Awake()
    {
        // Get the SpriteRenderer and initialize star palette colors
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing!");
            return;
        }

        _starMarioColors = new[] {fireMarioColor, blackMarioColor, greenMarioColor, redMarioColor};
    }

    private void OnEnable()
    {
        // Subscribe to Mario state change events
        MarioEvents.OnMarioStateChange += SwapPalette;
    }

    private void OnDisable()
    {
        // Unsubscribe from Mario state change events
        MarioEvents.OnMarioStateChange -= SwapPalette;
    }

    // Swap palette based on Mario's state
    private void SwapPalette(MarioState state)
    {
        if (general == null)
        {
            Debug.LogWarning("Material is not assigned!");
            return;
        }

        // Apply colors based on Mario's state
        switch (state)
        {
            case MarioState.Small:
            case MarioState.Big:
                ApplyColors(regularMarioColor);
                _starMarioColors[0] = regularMarioColor; // Use regular colors for the star effect
                break;
            case MarioState.Fire:
                ApplyColors(fireMarioColor);
                _starMarioColors[0] = fireMarioColor; // Use fire colors for the star effect
                break;
        }
    }

    // Apply colors to the shader
    private void ApplyColors(Color[] colors)
    {
        if (colors == null || colors.Length < 3)
        {
            Debug.LogWarning("Invalid color array passed to ApplyColors!");
            return;
        }

        general.SetColor(HatColour, colors[0]); // Apply hat color
        general.SetColor(BodyColour, colors[1]); // Apply body color
        general.SetColor(ClothColour, colors[2]); // Apply cloth color
    }

    // Start the star effect (flashing palette)
    public void StartFlashing()
    {
        if (!_isStar && _spriteRenderer != null)
        {
            _isStar = true;
            _starCoroutine = StartCoroutine(SwapStarPalette());
            // StartCoroutine(SwapStarPalette());
            ApplyColors(_starMarioColors[0]); // Apply the current palette
        }
    }
    
    public void StopFlashing()
    {
        if (_isStar && _spriteRenderer != null)
        {
            _isStar = false;

            if (_starCoroutine != null)
                StopCoroutine(_starCoroutine);

            // Start fading out the star effect over 2 seconds
            // StartCoroutine(SwapStarWithDelay(2f));
            ApplyColors(_starMarioColors[0]); // Apply the current palette
        }
    }
    
    public IEnumerator SwapStarWithDelay(float duration)
    {
        float elapsed = 0f;
        int index = 0;

        while (elapsed < duration)
        {
            ApplyColors(_starMarioColors[index]);
            index = (index + 1) % _starMarioColors.Length;
            elapsed += StarFlashInterval;
            yield return new WaitForSeconds(StarFlashInterval);
        }

        // After delay, switch back to regular color
        ApplyColors(_starMarioColors[0]); // Apply the current palette
    }

    // Coroutine for the star flashing effect
    private IEnumerator SwapStarPalette(float duration = 0.05f)
    {
        int index = 0; // Current index for star colors

        while (_isStar)
        {
            ApplyColors(_starMarioColors[index]); // Apply the current palette
            index = (index + 1) % _starMarioColors.Length; // Cycle through palettes
            yield return new WaitForSeconds(duration); // Wait for the interval
        }
    }
}
