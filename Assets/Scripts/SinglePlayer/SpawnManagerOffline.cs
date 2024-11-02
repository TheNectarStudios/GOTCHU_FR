using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManagerOffline : MonoBehaviour
{
    public GameObject protagonist;
    public Transform[] antagonistSpawnPoints;
    public GameObject antagonistPrefab;

    public GameObject joystickPrefab;
    public GameObject controlUI;
    public GameObject loadingScreen;
    public GameObject timerObject;

    public Button powerButton; // Reference to the power-up button
    public Image powerUpThumbnail;
    public Sprite freezeSprite;
    public Sprite bulletSprite;
    public Sprite speedBoostSprite;

    public GameObject bulletPrefab;

    private string currentPowerUp = null;
    private bool isCooldown = false;

    private void Start()
    {
        if (powerButton != null)
        {
            powerButton.onClick.AddListener(ActivateStoredPowerUp);
            powerButton.interactable = false; // Initially disable until power-up is ready
        }

        StartCoroutine(InitializeGameSequence());
    }

    private IEnumerator InitializeGameSequence()
    {
        yield return new WaitForSeconds(3.0f);
        HideLoadingScreen();

        yield return new WaitForSeconds(7.0f);
        EnableProtagonist();

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
            EnableJoystick();
        }
        else
        {
            Debug.LogWarning("Protagonist reference is missing.");
        }
    }

    private void SpawnAntagonists()
    {
        int spawnLimit = Mathf.Min(PlayerPrefs.GetInt("NumberOfBots", 1), antagonistSpawnPoints.Length);
        for (int i = 0; i < spawnLimit; i++)
        {
            Transform spawnPoint = antagonistSpawnPoints[i];
            Instantiate(antagonistPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        EnableJoystick();
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
        powerButton.interactable = true; // Enable the button when a power-up is available
        ShowPowerUpThumbnail(GetPowerUpSprite(powerUpName));
    }

    private void ActivateStoredPowerUp()
    {
        if (isCooldown || string.IsNullOrEmpty(currentPowerUp)) return;

        switch (currentPowerUp)
        {
            case "Freeze":
                ActivateFreezePowerUp();
                break;
            case "ReverseControls":
                ActivateBulletPowerUp();
                break;
            case "SpeedBoost":
                ActivateSpeedBoostPowerUp();
                break;
            default:
                Debug.LogWarning("Invalid power-up type: " + currentPowerUp);
                break;
        }

        currentPowerUp = null; // Clear current power-up after use
        HidePowerUpThumbnail();
        powerButton.interactable = false;
    }

    private void ActivateFreezePowerUp()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject enemy in enemies)
        {
            var botMovement = enemy.GetComponent<BotMovement>();
            if (botMovement != null)
            {
                // Call the FreezeBot method if it exists in BotMovement
                botMovement.FreezeBot();
                StartCoroutine(ReEnableBotMovement(botMovement, 5f));
            }
        }
    }

    private IEnumerator ReEnableBotMovement(BotMovement botMovement, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (botMovement != null)
        {
            // Call the UnfreezeBot method to restore the bot's functionality
            botMovement.UnfreezeBot();
        }
    }

    private void ActivateBulletPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            FireBullet(player.transform);
        }
    }

    private void FireBullet(Transform playerTransform)
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, playerTransform.position, playerTransform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = playerTransform.forward * 10f;
        }
    }

    private void ActivateSpeedBoostPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovementOffline movementScript = player.GetComponent<PlayerMovementOffline>();
            if (movementScript != null)
            {
                movementScript.speed += 2.0f; // Increase speed
                StartCoroutine(ResetSpeedAfterDelay(movementScript, 5f));
            }
        }
    }

    private IEnumerator ResetSpeedAfterDelay(PlayerMovementOffline movementScript, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (movementScript != null)
        {
            movementScript.speed -= 2.0f; // Reset speed to original
        }
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
            case "ReverseControls": return bulletSprite;
            case "SpeedBoost": return speedBoostSprite;
            default: return null;
        }
    }
}
