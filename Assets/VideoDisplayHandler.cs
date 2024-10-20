using UnityEngine;
using UnityEngine.UI;

public class FullScreenImage : MonoBehaviour
{
    public RawImage rawImage;           // The RawImage component to display the video
    public RectTransform screenTransform;  // Reference to the RawImage's RectTransform

    private void Start()
    {
        // Ensure the RawImage is assigned
        if (rawImage == null || screenTransform == null)
        {
            Debug.LogError("Please assign the RawImage and RectTransform components.");
            return;
        }

        // Make the RawImage fill the entire screen
        FitToFullScreen();
    }

    // Adjust the size of the RawImage to fill the screen
    private void FitToFullScreen()
    {
        // Set the size of the RectTransform to match the screen size
        screenTransform.anchorMin = Vector2.zero;   // Bottom-left corner of the screen
        screenTransform.anchorMax = Vector2.one;    // Top-right corner of the screen
        screenTransform.offsetMin = Vector2.zero;   // No offset for left and bottom
        screenTransform.offsetMax = Vector2.zero;   // No offset for right and top
    }
}
