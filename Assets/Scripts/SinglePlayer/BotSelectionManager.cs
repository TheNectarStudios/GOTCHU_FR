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
        // Ensure the dropdown has options for 1, 2, or 3 bots
        if (botDropdown != null)
        {
            botDropdown.ClearOptions();
            botDropdown.options.Add(new TMP_Dropdown.OptionData("1"));
            botDropdown.options.Add(new TMP_Dropdown.OptionData("2"));
            botDropdown.options.Add(new TMP_Dropdown.OptionData("3"));

            // Set default value to 1 bot
            botDropdown.value = 0;
            PlayerPrefs.SetInt(playerPrefsKey, 1); // Default number of bots is 1
            PlayerPrefs.Save();
        }
    }

    // Call this method when the dropdown value is changed
    public void OnDropdownValueChanged()
    {
        // Get the selected value (number of bots) and store it in PlayerPrefs
        int numberOfBots = botDropdown.value + 1; // Dropdown value 0 = 1 bot, 1 = 2 bots, 2 = 3 bots
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
