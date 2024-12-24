using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MainButtonDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool isDraggable = false; // Initially locked
    private bool isPowerupExecuted = false; // Track if the powerup has been executed
    private Coroutine deactivateCoroutine;

    [Header("Powerup Buttons")]
    public GameObject[] powerupButtons; // Array of powerup buttons to activate/deactivate

    [Header("Buffer Settings")]
    public float bufferTime = 0.2f; // Buffer time in seconds before deactivating powerups

    [Header("Drag Settings")]
    public float directionThreshold = 0.8f; // Threshold for allowed drag directions

    [Header("Button Reset Settings")]
    public float resetDuration = 0.5f; // Time taken to reset to initial position
    private Vector2 initialPosition; // Store the button's initial position

    // Allowed drag directions: diagonally up-right, up-left, down-right, down-left, up, and down
    private Vector2[] allowedDirections = new Vector2[] 
    {
        new Vector2(1f, 1f).normalized,  // Diagonally up-right
        new Vector2(-1f, 1f).normalized, // Diagonally up-left
        new Vector2(1f, -1f).normalized, // Diagonally down-right
        new Vector2(-1f, -1f).normalized, // Diagonally down-left
        new Vector2(0f, 1f).normalized,  // Straight up
        new Vector2(0f, -1f).normalized  // Straight down
    };

    private int vibrationCounter = 0; // Counter to log vibrations

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Save the initial position of the button
        initialPosition = rectTransform.anchoredPosition;
    }

    // Unlock the Main Button and reset states for the new powerup
    public void UnlockButton()
    {
        isPowerupExecuted = false; // Reset execution state
        isDraggable = true; // Unlock the button to allow dragging
        ResetPowerupButtons(); // Reset the powerup buttons
        Debug.Log("Main Button Unlocked!");
    }

    // Start dragging the button
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return; // Prevent dragging if not unlocked
        canvasGroup.blocksRaycasts = false; // Allow drag to pass through objects
        StopDeactivateCoroutine(); // Cancel deactivation if drag starts
    }

    // During drag
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return; // Prevent dragging if not unlocked

        // Normalize the drag delta to get the drag direction
        Vector2 dragDirection = eventData.delta.normalized;

        // Check if the drag direction is close to any allowed direction
        foreach (Vector2 allowedDirection in allowedDirections)
        {
            if (Vector2.Dot(dragDirection, allowedDirection) > directionThreshold)
            {
                // Allow dragging in the allowed direction
                rectTransform.anchoredPosition += eventData.delta;
                return;
            }
        }

        // Ignore drag if not in allowed direction
        // Debug.Log("Drag direction restricted!");
    }

    // End dragging and start the deactivation buffer
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return; // Prevent dragging if not unlocked
        canvasGroup.blocksRaycasts = true; // Re-enable raycasts after drag ends

        // Reset the button to its initial position instantly
        ResetToInitialPosition();
        StartDeactivateCoroutine(); // Start buffer for deactivation
    }

    // When the button is pressed, activate powerup buttons
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return; // Only activate powerup buttons if the Main Button is unlocked
        ActivatePowerupButtons();
        StopDeactivateCoroutine(); // Cancel deactivation if pressing again

        // Trigger haptic feedback (short vibration)
        TriggerShortVibration();
    }

    // When the pointer is lifted, start buffer for deactivation and reset position
    public void OnPointerUp(PointerEventData eventData)
    {
        ResetToInitialPosition(); // Reset the button to initial position instantly
        StartDeactivateCoroutine(); // Start buffer for deactivation
    }

    // Activate powerup buttons when the main button is pressed
    private void ActivatePowerupButtons()
    {
        if (isPowerupExecuted) return; // Do not activate if the powerup is already executed

        foreach (GameObject button in powerupButtons)
        {
            button.SetActive(true); // Activate the powerup buttons
        }
        Debug.Log("Powerup Buttons Activated!");
    }

    // Deactivate powerup buttons
    private void DeactivatePowerupButtons()
    {
        foreach (GameObject button in powerupButtons)
        {
            button.SetActive(false); // Deactivate the powerup buttons
        }
        Debug.Log("Powerup Buttons Deactivated!");
    }

    private void StartDeactivateCoroutine()
    {
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterBuffer());
    }

    private void StopDeactivateCoroutine()
    {
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }
    }

    private IEnumerator DeactivateAfterBuffer()
    {
        yield return new WaitForSeconds(bufferTime); // Wait for buffer time before deactivating
        DeactivatePowerupButtons();
    }

    // Reset the button to its initial position instantly
    private void ResetToInitialPosition()
    {
        rectTransform.anchoredPosition = initialPosition; // Instantly set the button's position to the initial position
        Debug.Log("Button Reset to Initial Position!");
    }

    // Check if the powerup has been executed
    public bool HasExecutedPowerup()
    {
        return isPowerupExecuted;
    }

    // Mark the main button as having executed a powerup
    public void SetPowerupExecuted()
    {
        isPowerupExecuted = true;
        Debug.Log("Powerup Executed!");
        DeactivatePowerupButtons(); // Immediately deactivate powerup buttons after execution
    }

    // Reset the Main Button to allow use of the next powerup
    public void ResetMainButton()
    {
        isPowerupExecuted = false; // Reset execution flag
        isDraggable = false; // Lock button to prevent use until a new powerup is picked up
        Debug.Log("Main Button Reset!");
        ResetPowerupButtons(); // Ensure the powerup buttons are reset
    }

    // Reset the powerup buttons when a new powerup is picked up
    public void ResetPowerupButtons()
    {
        foreach (GameObject button in powerupButtons)
        {
            button.SetActive(false); // Deactivate all powerup buttons
        }
        Debug.Log("Powerup Buttons Reset!");
    }

    // Trigger a short vibration based on the platform and log the timing
    private void TriggerShortVibration()
    {
        vibrationCounter++; // Increment the vibration counter
        Debug.Log($"Vibration #{vibrationCounter} triggered at {Time.time} seconds.");

        // Check the platform (Android or iOS) and use the appropriate vibration method
        if (Application.platform == RuntimePlatform.Android)
        {
            // Android - Vibrate for 25 milliseconds (0.025 second), which is 25% of the usual intensity
            Handheld.Vibrate();
            StartCoroutine(VibrationDuration(0.05f)); // Vibrate for 50 milliseconds (half the original length)
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // iOS - Use a lighter haptic feedback style to simulate a weaker vibration
            Vibration.VibrateIOS(ImpactFeedbackStyle.Medium);
            StartCoroutine(VibrationDuration(0.05f)); // Simulate a shorter vibration on iOS
        }
    }

    // Coroutine to control the vibration duration
    private IEnumerator VibrationDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        // Log the time when vibration ends
        Debug.Log($"Vibration #{vibrationCounter} ended at {Time.time} seconds.");
    }
}
