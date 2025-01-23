using System;
using UnityEngine;
using Unity.Cinemachine;

[ExecuteAlways] // Optional: Allows the script to run in the editor
public class FitCameraToWorldWidth : MonoBehaviour
{
    private const float RatioChangeThreshold = 0.01f;

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] private CinemachineCamera virtualCamera;

    [Header("Desired World Units Width")]
    [SerializeField] private float targetWidth = 10f;

    private float currentAspectRatio;

    private void Awake()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("CinemachineVirtualCamera is not assigned and not found on the GameObject.");
                enabled = false;
                return;
            }
        }

        // Check if the camera is orthographic
        if (!virtualCamera.Lens.Orthographic)
        {
            Debug.LogWarning("Cinemachine Virtual Camera is not set to orthographic. Please enable orthographic mode.");
        }
    }

    private void Start()
    {
        currentAspectRatio = (float)Screen.width / Screen.height;
        FitToWidth();
    }

    private void Update()
    {
        float newAspectRatio = (float)Screen.width / Screen.height;
        if (Math.Abs(newAspectRatio - currentAspectRatio) > RatioChangeThreshold)
        {
            currentAspectRatio = newAspectRatio;
            FitToWidth();
        }
    }

    private void FitToWidth()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera reference is missing.");
            return;
        }

        // Access and modify the lens settings
        var lens = virtualCamera.Lens;

        // Calculate the current width based on orthographic size and aspect ratio
        float currentHeight = lens.OrthographicSize * 2f;
        float currentWidth = currentHeight * currentAspectRatio;

        // Determine the ratio to fit the target width
        float ratioChange = targetWidth / currentWidth;

        // Adjust the orthographic size accordingly
        lens.OrthographicSize *= ratioChange;

        // Apply the updated lens settings back to the virtual camera
        virtualCamera.Lens = lens;
    }
}
