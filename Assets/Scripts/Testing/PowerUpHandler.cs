using UnityEngine;
using UnityEngine.EventSystems;

public class PowerupButtonDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.CompareTag("MainButton"))
        {
            MainButtonDrag mainButton = droppedObject.GetComponent<MainButtonDrag>();

            if (mainButton != null)
            {
                Debug.Log("Main Button dropped on " + gameObject.name);
                ExecutePowerup(mainButton); // Execute powerup logic
                mainButton.SetPowerupExecuted(); // Mark main button as having executed the powerup
            }
        }
    }

    private void ExecutePowerup(MainButtonDrag mainButton)
    {
        // Logic for executing the specific powerup
        Debug.Log("Executing powerup for " + gameObject.name);
        // Add your specific powerup logic here
    }

    // Reset the powerup usage so it can be used again later
    public void ResetPowerupUsage()
    {
        // This method is now optional based on your needs
    }
}
