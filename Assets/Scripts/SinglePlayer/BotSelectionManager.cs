using UnityEngine;
using TMPro;  // Import TextMeshPro
using UnityEngine.SceneManagement;  // For scene loading

public class BotSelectionManager : MonoBehaviour
{
    public TMP_Dropdown botDropdown; // Reference to the TMP dropdown
    public string playerPrefsKey = "NumberOfBots"; // Key to store the number of bots in PlayerPrefs
    public string gameSceneName = "GameScene"; // Name of the game scene to load

    private void Start()
    {
        // Ensure the dropdown has options (for example, you can set 0-10 bots)
        if (botDropdown != null)
        {
            botDropdown.ClearOptions();
            for (int i = 1; i <= 3; i++)
            {
                botDropdown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
            }

            // Set default value (you can modify this as needed)
            botDropdown.value = 0;
        }
    }

    // Call this method when the dropdown value is changed
    public void OnDropdownValueChanged()
    {
        // Get the selected value (number of bots) and store it in PlayerPrefs
        int numberOfBots = botDropdown.value;
        PlayerPrefs.SetInt(playerPrefsKey, numberOfBots);
        PlayerPrefs.Save();  // Save the changes
    }

    // Call this method when the start button is clicked
    public void OnStartButtonClick()
    {
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }
}
