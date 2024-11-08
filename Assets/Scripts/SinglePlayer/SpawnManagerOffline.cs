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
    private PlayerMovementOffline playerMovementScript;

    private void Start()
    {
        if (powerButton != null)
        {
            powerButton.onClick.AddListener(ActivateStoredPowerUp);
            powerButton.interactable = false; // Initially disable until power-up is ready
        }

        // Get the PlayerMovementOffline script from the protagonist
        if (protagonist != null)
        {
            playerMovementScript = protagonist.GetComponent<PlayerMovementOffline>();
            if (playerMovementScript == null)
            {
                Debug.LogWarning("PlayerMovementOffline script is missing on the protagonist.");
            }
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
            botMovement.UnfreezeBot();
        }
    }

    public void ActivateBulletPowerUp()
    {
        if (playerMovementScript == null) return;

        // Use the last movement direction from the player movement script
        Vector3 lastDirection = playerMovementScript.GetLastMovementDirection();

        // Determine the fixed direction (0°, 90°, 180°, 270°)
        Vector3 bulletDirection = Vector3.zero;

        if (lastDirection.x > 0) // Right
        {
            bulletDirection = Vector3.right;
        }
        else if (lastDirection.x < 0) // Left
        {
            bulletDirection = Vector3.left;
        }
        else if (lastDirection.z > 0) // Up
        {
            bulletDirection = Vector3.forward;
        }
        else if (lastDirection.z < 0) // Down
        {
            bulletDirection = Vector3.back;
        }

        // Fire the bullet in the fixed direction
        if (bulletDirection != Vector3.zero)
        {
            FireBullet(protagonist.transform, bulletDirection);
        }
        else
        {
            Debug.LogWarning("Bullet direction is zero, but firing in last known direction.");
        }
    }

    private void FireBullet(Transform playerTransform, Vector3 direction)
    {
        if (bulletPrefab != null)
        {
            // Instantiate the bullet at the player's position
            GameObject bullet = Instantiate(bulletPrefab, playerTransform.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Apply velocity in the fixed direction
                rb.velocity = direction * 10f; // Adjust speed as needed

                // Rotate the bullet to face the direction it's moving
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                Debug.Log("Bullet fired in direction: " + direction);
            }
            else
            {
                Debug.LogWarning("Rigidbody component is missing on the bullet prefab!");
            }
        }
        else
        {
            Debug.LogWarning("Bullet prefab is missing!");
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
