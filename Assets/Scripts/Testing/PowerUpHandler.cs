using UnityEngine;
using UnityEngine.EventSystems;

public class PowerupButtonDrop : MonoBehaviour, IDropHandler
{
    private bool isPowerupUsed = false; // Track if the powerup has been used

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.CompareTag("MainButton"))
        {
            MainButtonDrag mainButton = droppedObject.GetComponent<MainButtonDrag>();

            if (mainButton != null && !mainButton.HasExecutedPowerup() && !isPowerupUsed)
            {
                Debug.Log("Main Button dropped on " + gameObject.name);
                ExecutePowerup(mainButton); // Execute powerup logic
                isPowerupUsed = true; // Mark this powerup button as used
                mainButton.SetPowerupExecuted(); // Mark main button as having executed the powerup
            }
            else
            {
                Debug.Log("Powerup on " + gameObject.name + " has already been used or main button is locked!");
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
        isPowerupUsed = false; // Reset the powerup button usage
    }
}