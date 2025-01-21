using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CanvasAutoResizer : MonoBehaviour
{
    private Canvas _canvas;
    private RectTransform _canvasRectTransform;

    [SerializeField]
    private int referenceWidth = 1920;  // Set your desired reference width
    [SerializeField]
    private int referenceHeight = 1080; // Set your desired reference height

    void Start()
    {
        ResizeCanvas();
    }

    void Update()
    {
        // Only resize when screen size changes (useful for responsive UI)
        if (!Mathf.Approximately(Screen.width, _canvasRectTransform.rect.width) || !Mathf.Approximately(Screen.height, _canvasRectTransform.rect.height))
        {
            ResizeCanvas();
        }
    }

    void ResizeCanvas()
    {
        _canvas = GetComponent<Canvas>();
        _canvasRectTransform = GetComponent<RectTransform>();

        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera || _canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            float scaleFactorX = (float)Screen.width / referenceWidth;
            float scaleFactorY = (float)Screen.height / referenceHeight;

            // Use the smaller scale factor to maintain aspect ratio
            float scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY);

            // Set the new width and height to maintain aspect ratio
            _canvasRectTransform.sizeDelta = new Vector2(referenceWidth * scaleFactor, referenceHeight * scaleFactor);
            _canvas.scaleFactor = scaleFactor;

            Debug.Log($"Canvas resized to: {_canvasRectTransform.sizeDelta}");
        }
        else
        {
            Debug.LogWarning("Canvas is not set to Screen Space - Camera or Overlay.");
        }
    }
}