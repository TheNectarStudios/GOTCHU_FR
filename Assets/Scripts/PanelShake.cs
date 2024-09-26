using UnityEngine;
using System.Collections;

public class PanelShake : MonoBehaviour
{
    // Reference to the panel's RectTransform
    public RectTransform panelRect;

    // Shake settings
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 10f;

    private Vector2 initialPosition;
    private bool isShaking = false;

    private void Start()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();

        initialPosition = panelRect.anchoredPosition; // Store the initial anchored position as Vector2
    }

    // Method to call when you want to start shaking
    public void TriggerShake()
    {
        if (!isShaking)
            StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // Generate random shake within the specified magnitude
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            // Apply the shake to the panel
            panelRect.anchoredPosition = new Vector2(offsetX, offsetY) + initialPosition;

            yield return null; // Wait for the next frame
        }

        // Return to the initial position after the shake
        panelRect.anchoredPosition = initialPosition;
        isShaking = false;
    }
}
