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

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
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
        rectTransform.anchoredPosition += eventData.delta; // Adjust position
    }

    // End dragging and start the deactivation buffer
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return; // Prevent dragging if not unlocked
        canvasGroup.blocksRaycasts = true; // Re-enable raycasts after drag ends
        StartDeactivateCoroutine(); // Start buffer for deactivation
    }

    // When the button is pressed, activate powerup buttons
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return; // Only activate powerup buttons if the Main Button is unlocked
        ActivatePowerupButtons();
        StopDeactivateCoroutine(); // Cancel deactivation if pressing again
    }

    // When the pointer is lifted, start buffer for deactivation
    public void OnPointerUp(PointerEventData eventData)
    {
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
}
