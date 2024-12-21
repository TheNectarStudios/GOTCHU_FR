using UnityEngine;
using UnityEngine.EventSystems;

public class PowerupButtonDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.CompareTag("MainButton"))
        {
            Debug.Log("Main Button dropped on " + gameObject.name);
            ExecutePowerup();
        }
    }

    private void ExecutePowerup()
    {
        // Logic for executing the specific powerup
        Debug.Log("Executing powerup for " + gameObject.name);
    }
}
