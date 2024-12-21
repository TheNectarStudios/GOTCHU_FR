using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public MainButtonDrag mainButtonScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("powerUp"))
        {
            mainButtonScript.ResetMainButton(); // Reset main button state
            mainButtonScript.UnlockButton(); // Unlock the main button for new powerup
            Destroy(other.gameObject); // Destroy the powerup object
        }
    }
}
