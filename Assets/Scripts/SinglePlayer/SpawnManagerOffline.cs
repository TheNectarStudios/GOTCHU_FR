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

    public Button powerButton;
    public Image powerUpThumbnail;
    public Sprite freezeSprite;
    public Sprite bulletSprite;
    public Sprite speedBoostSprite;

    public GameObject bulletPrefab;

    // Recoil configuration
    public float recoilIntensity = 0.2f; // Increased intensity for more pronounced recoil
    public float recoilDuration = 0.15f; // Slightly longer duration
    public float recoilWobbleFactor = 0.1f; // More wobble for added effect

    private string currentPowerUp = null;
    private bool isCooldown = false;
    private PlayerMovementOffline playerMovementScript;
    private int bulletCount = 0; // Counter for the number of bullets fired
    private const int maxBullets = 3; // Maximum number of bullets allowed

    private void Start()
    {
        if (powerButton != null)
        {
            powerButton.onClick.AddListener(ActivateStoredPowerUp);
            powerButton.interactable = false; // Initially disable until a power-up is available
        }

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
        powerButton.interactable = true;
        ShowPowerUpThumbnail(GetPowerUpSprite(powerUpName));
    }

    private void ActivateStoredPowerUp()
    {
        if (isCooldown || string.IsNullOrEmpty(currentPowerUp)) return;

        if (currentPowerUp == "ReverseControls")
        {
            // Handle bullet power-up, firing one bullet per button press
            if (bulletCount < maxBullets)
            {
                FireBullet(protagonist.transform, playerMovementScript.GetLastMovementDirection());
                bulletCount++;
                if (bulletCount >= maxBullets)
                {
                    // Clear the bullet power-up after all bullets are fired
                    currentPowerUp = null;
                    HidePowerUpThumbnail();
                    powerButton.interactable = false;
                }
            }
        }
        else
        {
            // Handle other power-ups, which are used once
            switch (currentPowerUp)
            {
                case "Freeze":
                    ActivateFreezePowerUp();
                    break;
                case "SpeedBoost":
                    ActivateSpeedBoostPowerUp();
                    break;
                default:
                    Debug.LogWarning("Invalid power-up type: " + currentPowerUp);
                    break;
            }

            // Clear the power-up after it is used
            currentPowerUp = null;
            HidePowerUpThumbnail();
            powerButton.interactable = false;
        }
    }

    private void FireBullet(Transform playerTransform, Vector3 direction)
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, playerTransform.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 bulletDirection = DetermineFixedDirection(direction);
                rb.velocity = bulletDirection * 10f;
                bullet.transform.rotation = Quaternion.LookRotation(bulletDirection);
                Debug.Log("Bullet fired in direction: " + bulletDirection);

                // Apply recoil effect in the opposite direction of the bullet
                StartCoroutine(ApplyRecoil(bulletDirection));
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

    private Vector3 DetermineFixedDirection(Vector3 lastDirection)
    {
        if (lastDirection.x > 0) return Vector3.right;
        if (lastDirection.x < 0) return Vector3.left;
        if (lastDirection.z > 0) return Vector3.forward;
        if (lastDirection.z < 0) return Vector3.back;
        return Vector3.zero;
    }

    private IEnumerator ApplyRecoil(Vector3 bulletDirection)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = protagonist.transform.localPosition;
        Quaternion originalRotation = protagonist.transform.localRotation;

        // Calculate recoil direction as the opposite of bullet direction
        Vector3 recoilDirection = -bulletDirection.normalized * recoilIntensity;

        while (elapsedTime < recoilDuration)
        {
            // Apply recoil and add wobble effect
            float wobbleX = Random.Range(-recoilWobbleFactor, recoilWobbleFactor);
            float wobbleY = Random.Range(-recoilWobbleFactor, recoilWobbleFactor);
            float wobbleZ = Random.Range(-recoilWobbleFactor, recoilWobbleFactor);

            protagonist.transform.localPosition = originalPosition + recoilDirection;
            protagonist.transform.localRotation = originalRotation * Quaternion.Euler(wobbleX, wobbleY, wobbleZ);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset to original position and rotation
        protagonist.transform.localPosition = originalPosition;
        protagonist.transform.localRotation = originalRotation;
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

    private void ActivateSpeedBoostPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovementOffline movementScript = player.GetComponent<PlayerMovementOffline>();
            if (movementScript != null)
            {
                movementScript.speed += 2.0f;
                StartCoroutine(ResetSpeedAfterDelay(movementScript, 5f));
            }
        }
    }

    private IEnumerator ResetSpeedAfterDelay(PlayerMovementOffline movementScript, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (movementScript != null)
        {
            movementScript.speed -= 2.0f;
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
