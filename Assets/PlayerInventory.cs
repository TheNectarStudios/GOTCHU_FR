// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerInventory : MonoBehaviour
// {
//     private List<PowerUp> collectedPowerUps = new List<PowerUp>(); // List of collected power-ups

//     void Update()
//     {
//         // Check if the player presses the spacebar to activate the first power-up in the list
//         if (Input.GetKeyDown(KeyCode.Space) && collectedPowerUps.Count > 0)
//         {
//             ActivatePowerUp(0); // Activate the first power-up in the inventory
//         }
//     }

//     // Method to add power-ups to the inventory
//     public void AddPowerUp(PowerUp powerUp)
//     {
//         collectedPowerUps.Add(powerUp);
//         Debug.Log("Collected power-up: " + powerUp.powerUpName);
//     }

//     // Method to activate a power-up
//     private void ActivatePowerUp(int index)
//     {
//         PowerUp powerUpToActivate = collectedPowerUps[index];

//         // Check for the type of power-up and activate it accordingly
//         if (powerUpToActivate is SpeedBoost)
//         {
//             (powerUpToActivate as SpeedBoost).ActivatePowerUp();
//         }
//         else if (powerUpToActivate is ReverseControls)
//         {
//             (powerUpToActivate as ReverseControls).ActivatePowerUp();
//         }
//         else if (powerUpToActivate is Freeze)
//         {
//             (powerUpToActivate as Freeze).ActivatePowerUp();
//         }

//         // Remove the power-up from the inventory after using it
//         collectedPowerUps.RemoveAt(index);
//         Debug.Log("Power-up activated and removed from inventory: " + powerUpToActivate.powerUpName);
//     }
// }
