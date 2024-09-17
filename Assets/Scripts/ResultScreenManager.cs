using UnityEngine;
using TMPro;  // Required for TextMeshPro

public class ResultScreenManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;  // Assign this in the inspector

    private void Start()
    {
        // Retrieve the game result message from PlayerPrefs
        string resultMessage = PlayerPrefs.GetString("GameResult", "No result available");

        // Display the result message in the TextMeshProUGUI field
        resultText.text = resultMessage;
    }
}
