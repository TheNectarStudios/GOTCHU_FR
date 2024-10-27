// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;

// public class PowerUpUI : MonoBehaviour
// {
//     public Transform inventoryPanel; // The panel where power-ups will be displayed
//     public GameObject powerUpButtonPrefab; // The button prefab to represent each power-up
//     private PlayerInventory playerInventory;

//     void Start()
//     {
//         playerInventory = FindObjectOfType<PlayerInventory>();
//     }

//     public void UpdateInventoryUI()
//     {
//         // Clear existing UI buttons
//         foreach (Transform child in inventoryPanel)
//         {
//             Destroy(child.gameObject);
//         }

//         // Create a button for each collected power-up
//         for (int i = 0; i < playerInventory.collectedPowerUps.Count; i++)
//         {
//             GameObject newButton = Instantiate(powerUpButtonPrefab, inventoryPanel);
//             PowerUp powerUp = playerInventory.collectedPowerUps[i];

//             // Update the button's image and text
//             newButton.GetComponentInChildren<Text>().text = powerUp.powerUpName;
//             newButton.GetComponent<Image>().sprite = powerUp.icon;

//             // Add a click event to use the power-up
//             int powerUpIndex = i; // Local variable to capture index
//             newButton.GetComponent<Button>().onClick.AddListener(() => playerInventory.UsePowerUp(powerUpIndex));
//         }
//     }
// }
