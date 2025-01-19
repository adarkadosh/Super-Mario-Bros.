using UnityEngine;

public class SetGameResolution : MonoBehaviour
{
    void Start()
    {
        int targetWidth = 256;
        int targetHeight = 240;
        bool fullscreen = false;

        Screen.SetResolution(targetWidth, targetHeight, fullscreen);
    }
}
