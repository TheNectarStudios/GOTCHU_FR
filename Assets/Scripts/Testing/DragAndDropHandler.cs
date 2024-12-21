using UnityEngine;
using UnityEngine.EventSystems;

public class MainButtonDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private bool isDraggable = false; // Locked by default

    [Header("Powerup Buttons")]
    public GameObject[] powerupButtons; // Array of powerup buttons to activate

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void UnlockButton() 
    {
        isDraggable = true; // Unlock the button after picking up the powerup
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return; // Prevent dragging if not unlocked
        canvasGroup.blocksRaycasts = false; // Allow drag to pass through objects
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return; // Prevent dragging if not unlocked
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Adjust position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return; // Prevent dragging if not unlocked
        canvasGroup.blocksRaycasts = true; // Re-enable raycasts after drag ends
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDraggable) return; // Only activate powerup buttons if the Main Button is unlocked

        // Activate all Powerup Buttons
        foreach (GameObject button in powerupButtons)
        {
            button.SetActive(true);
        }

        Debug.Log("Powerup Buttons Activated!");
    }
}
