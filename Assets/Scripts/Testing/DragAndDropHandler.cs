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

    private int vibrationCounter = 0; // Counter to log vibrations

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = rectTransform.anchoredPosition;
    }

    public void UnlockButton()
    {
        isPowerupExecuted = false;
        isDraggable = true;
        ResetPowerupButtons();
        Debug.Log("Main Button Unlocked!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        canvasGroup.blocksRaycasts = false;
        StopDeactivateCoroutine();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        Vector2 dragDirection = eventData.delta.normalized;

        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        canvasGroup.blocksRaycasts = true;
        ResetToInitialPosition();
        StartDeactivateCoroutine();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return;
        ActivatePowerupButtons();
        StopDeactivateCoroutine();
        TriggerShortVibration();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetToInitialPosition();
        StartDeactivateCoroutine();
    }

    private void ActivatePowerupButtons()
    {
        if (isPowerupExecuted) return;

        foreach (GameObject button in powerupButtons)
        {
            button.SetActive(true);
        }
        Debug.Log("Powerup Buttons Activated!");
    }

    private void DeactivatePowerupButtons()
    {
        foreach (GameObject button in powerupButtons)
        {
            button.SetActive(false);
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
        yield return new WaitForSeconds(bufferTime);
        DeactivatePowerupButtons();
    }

    private void ResetToInitialPosition()
    {
        rectTransform.anchoredPosition = initialPosition;
        Debug.Log("Button Reset to Initial Position!");
    }

    public bool HasExecutedPowerup()
    {
        return isPowerupExecuted;
    }

    public void SetPowerupExecuted()
    {
        isPowerupExecuted = true;
        Debug.Log("Powerup Executed!");
        DeactivatePowerupButtons();
    }

    public void ResetMainButton()
    {
        isPowerupExecuted = false;
        isDraggable = false;
        Debug.Log("Main Button Reset!");
        ResetPowerupButtons();
    }

    public void ResetPowerupButtons()
    {
        foreach (GameObject button in powerupButtons)
        {
            button.SetActive(false);
        }
        Debug.Log("Powerup Buttons Reset!");
    }

    private void TriggerShortVibration()
    {
        vibrationCounter++;
        Debug.Log($"Vibration #{vibrationCounter} triggered at {Time.time} seconds.");

        if (Application.platform == RuntimePlatform.Android)
        {
            // Handheld.Vibrate();
            StartCoroutine(VibrationDuration(0.01f)); // Vibrate for 10 milliseconds
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
            StartCoroutine(VibrationDuration(0.01f)); // Simulate a shorter vibration on iOS
        }
    }

    private IEnumerator VibrationDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log($"Vibration #{vibrationCounter} ended at {Time.time} seconds.");
    }
}
