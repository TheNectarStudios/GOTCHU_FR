using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManagerOffline : MonoBehaviour
{
    public Transform protagonistSpawnPoint;
    public GameObject protagonistPrefab;

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
    private string currentPowerUp = null;

    private void Start()
    {
        StartCoroutine(SpawnEntitiesAfterDelay());
    }

    private IEnumerator SpawnEntitiesAfterDelay()
    {
        yield return new WaitForSeconds(3.0f); // Optional delay before spawning

        HideLoadingScreen();

        // Spawn protagonist
        SpawnProtagonist();

        // Spawn antagonists
        SpawnAntagonists();

        yield return new WaitForSeconds(2f);

        ShowControlUI();
        StartTimer();
    }

    private void SpawnProtagonist()
    {
        if (protagonistPrefab != null && protagonistSpawnPoint != null)
        {
            GameObject protagonist = Instantiate(protagonistPrefab, protagonistSpawnPoint.position, protagonistSpawnPoint.rotation);
            EnableJoystick(); // Enable joystick for protagonist
        }
    }

    private void SpawnAntagonists()
    {
        for (int i = 0; i < antagonistSpawnPoints.Length; i++)
        {
            if (spawnedGhostsCount < antagonistSpawnPoints.Length)
            {
                Transform spawnPoint = antagonistSpawnPoints[spawnedGhostsCount];
                Instantiate(antagonistPrefab, spawnPoint.position, spawnPoint.rotation);
                spawnedGhostsCount++;
            }
        }

        EnableJoystick(); // Enable joystick for antagonists as well, if required
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
        switch (powerUpName)
        {
            case "Freeze": return freezeSprite;
            case "Bullet": return bulletSprite;
            case "SpeedBoost": return speedBoostSprite;
            default: return null;
        }
    }
}
