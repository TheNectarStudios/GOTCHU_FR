using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public MainButtonDrag mainButtonScript; // Reference to the Main Button's script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("powerUp"))
        {
            mainButtonScript.UnlockButton(); // Unlock the Main Button
            Destroy(other.gameObject); // Destroy the powerup object
        }
    }
}
