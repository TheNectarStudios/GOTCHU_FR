using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManagerOffline : MonoBehaviour
{
    public GameObject protagonist; // Reference to the protagonist already in the scene
    public Transform[] antagonistSpawnPoints;
    public GameObject antagonistPrefab;

    public GameObject joystickPrefab;
    public GameObject controlUI;
    public GameObject loadingScreen;
    public GameObject timerObject;

    public Image powerUpThumbnail;
    public Sprite freezeSprite;
    public Sprite bulletSprite;
    public Sprite speedBoostSprite;

    private int spawnedGhostsCount = 0;
    private int numberOfAntagonists = 1;  // Default to 1 if not set in PlayerPrefs
    private string currentPowerUp = null;

    private void Start()
    {
        // Fetch the number of bots from PlayerPrefs
        numberOfAntagonists = PlayerPrefs.GetInt("NumberOfBots", 1);

        StartCoroutine(SpawnEntitiesAfterDelay());
    }

    private IEnumerator SpawnEntitiesAfterDelay()
    {
        yield return new WaitForSeconds(3.0f); // Optional delay before spawning

        HideLoadingScreen();

        // Enable protagonist after 7 seconds
        yield return new WaitForSeconds(7.0f);
        EnableProtagonist();

        // Spawn antagonists
        SpawnAntagonists();

        yield return new WaitForSeconds(2f);

        ShowControlUI();
        StartTimer();
    }

    private void EnableProtagonist()
    {
        if (protagonist != null)
        {
            protagonist.SetActive(true);
            EnableJoystick(); // Enable joystick for protagonist
        }
        else
        {
            Debug.LogWarning("Protagonist reference is missing.");
        }
    }

    private void SpawnAntagonists()
    {
        int spawnLimit = Mathf.Min(numberOfAntagonists, antagonistSpawnPoints.Length);

        for (int i = 0; i < spawnLimit; i++)
        {
            Transform spawnPoint = antagonistSpawnPoints[i];
            Instantiate(antagonistPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedGhostsCount++;
        }

        EnableJoystick(); // Enable joystick for antagonists if required
    }

    private void EnableJoystick()
    {
        if (joystickPrefab != null)
        {
            joystickPrefab.SetActive(true);
        }
    }

    private void ShowControlUI()
    {
        if (controlUI != null)
        {
            controlUI.SetActive(true);
        }
    }

    private void HideLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    private void StartTimer()
    {
        if (timerObject != null)
        {
            timerObject.SetActive(true);
        }
    }

    // Power-up management
    public void UpdateInventory(string powerUpName)
    {
        currentPowerUp = powerUpName;
        ShowPowerUpThumbnail(GetPowerUpSprite(powerUpName));
    }

    private void ShowPowerUpThumbnail(Sprite powerUpSprite)
    {
        if (powerUpThumbnail != null)
        {
            powerUpThumbnail.sprite = powerUpSprite;
            powerUpThumbnail.gameObject.SetActive(true);
        }
    }

    private void HidePowerUpThumbnail()
    {
        if (powerUpThumbnail != null)
        {
            powerUpThumbnail.gameObject.SetActive(false);
        }
    }

    private Sprite GetPowerUpSprite(string powerUpName)
    {
        Debug.Log("Received power-up name: " + powerUpName);
        switch (powerUpName)
        {
            case "Freeze":
                return freezeSprite;
            case "Bullet":
                return bulletSprite;
            case "SpeedBoost":
                return speedBoostSprite;
            // Add additional cases for other power-ups if necessary
            default:
                Debug.LogWarning("Power-up name '" + powerUpName + "' does not match any available power-up sprites.");
                return null;
        }
    }


}
