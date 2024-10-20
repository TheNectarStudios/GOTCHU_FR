using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManagerOffline : MonoBehaviour
{
    public Transform protagonistSpawnPoint;
    public Material freezeEffectMaterial;

    public Transform[] antagonistSpawnPoints;
    public Button powerButton;
    private string currentPowerUp = null;

    public GameObject protagonistPrefab;
    public GameObject antagonistPrefab;
    public GameObject bulletPrefab;

    public float bufferTime = 3.0f;
    private bool isCooldown = false;

    public Image powerUpThumbnail;
    public Sprite freezeSprite;
    public Sprite bulletSprite;
    public Sprite speedBoostSprite;

    public GameObject protagonistPanel;
    public GameObject antagonistPanel;  // Single antagonist panel

    public GameObject controlUI;
    public GameObject loadingScreen;

    public GameObject timerObject;

    public GameObject joystickPrefab;

    public GameObject antagonistInvisibilityPanel;
    public GameObject antagonistDashPanel;
    public GameObject antagonistTrapPanel;

    private ShaderManager shaderManager;
    private int spawnedGhostsCount = 0;  // Keep track of ghosts spawned

    private void Start()
    {
        shaderManager = FindObjectOfType<ShaderManager>();
        StartCoroutine(WaitAndAssignRoles());
    }

    private IEnumerator WaitAndAssignRoles()
    {
        yield return new WaitForSeconds(bufferTime);

        HideLoadingScreen();
        HideAllRolePanels();

        AssignRoles();

        yield return new WaitForSeconds(3f);
        HideAllRolePanels();
        ShowControlUI();
        yield return new WaitForSeconds(2f);
        StartTimer();
    }

    private void AssignRoles()
    {
        List<int> players = new List<int> { 0, 1, 2, 3 };  // Dummy list representing 4 players
        int protagonistIndex = Random.Range(0, players.Count);

        for (int i = 0; i < players.Count; i++)
        {
            if (i == protagonistIndex)
            {
                SetProtagonist();
            }
            else
            {
                SetAntagonist();
            }
        }
    }

    private void SetProtagonist()
    {
        GameObject protagonist = Instantiate(protagonistPrefab, protagonistSpawnPoint.position, protagonistSpawnPoint.rotation);
        AssignButtonControls(protagonist);
        AssignCamera(protagonist);
        ShowPanel(protagonistPanel);
        EnableJoystick(); // Enable the joystick for the protagonist
    }

    private void SetAntagonist()
    {
        if (spawnedGhostsCount < antagonistSpawnPoints.Length)
        {
            Transform spawnPoint = antagonistSpawnPoints[spawnedGhostsCount];
            GameObject antagonist = Instantiate(antagonistPrefab, spawnPoint.position, spawnPoint.rotation);
            AssignButtonControls(antagonist);
            AssignCamera(antagonist);

            AssignAbilities(antagonist);
            EnableJoystick(); // Enable the joystick for the antagonist
            spawnedGhostsCount++;
        }
    }

    private void AssignButtonControls(GameObject player)
    {
        PacMan3DMovement movementScript = player.GetComponent<PacMan3DMovement>();
        if (movementScript != null)
        {
            powerButton.interactable = true;
            powerButton.onClick.RemoveAllListeners();
            powerButton.onClick.AddListener(ActivateStoredPowerUp);
        }
    }

    private void ActivateStoredPowerUp()
    {
        if (currentPowerUp != null)
        {
            if (currentPowerUp == "Freeze")
            {
                ActivateFreezePowerUp();
            }
            else if (currentPowerUp == "Bullet")
            {
                ActivateBulletPowerUp();
            }
            else if (currentPowerUp == "SpeedBoost")
            {
                ActivateSpeedBoostPowerUp();
            }
            currentPowerUp = null;
            HidePowerUpThumbnail();
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

    private void AssignCamera(GameObject player)
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            TopDownCameraFollow cameraFollowScript = mainCamera.GetComponent<TopDownCameraFollow>();

            if (cameraFollowScript == null)
            {
                cameraFollowScript = mainCamera.gameObject.AddComponent<TopDownCameraFollow>();
            }

            cameraFollowScript.target = player.transform;
        }
    }

    private void AssignAbilities(GameObject player)
    {
        powerButton.interactable = true;
        powerButton.onClick.RemoveAllListeners();

        Invisibility invisibility = player.GetComponent<Invisibility>();
        Dash dash = player.GetComponent<Dash>();
        Trap trap = player.GetComponent<Trap>();

        if (invisibility != null)
        {
            powerButton.onClick.AddListener(() => ActivatePower(invisibility.ActivateInvisibility, invisibility.cooldownTime));
            ShowPanel(antagonistInvisibilityPanel);
        }
        if (dash != null)
        {
            powerButton.onClick.AddListener(() => ActivatePower(dash.ActivateDash, dash.cooldownTime));
            ShowPanel(antagonistDashPanel);
        }
        if (trap != null)
        {
            powerButton.onClick.AddListener(() => ActivatePower(trap.PlaceTrap, trap.cooldownTime));
            ShowPanel(antagonistTrapPanel);
        }
    }

    private void ActivatePower(System.Action powerAction, float cooldown)
    {
        if (!isCooldown)
        {
            powerAction.Invoke();
            StartCoroutine(CooldownRoutine(cooldown));
        }
    }

    private IEnumerator CooldownRoutine(float cooldown)
    {
        isCooldown = true;
        powerButton.interactable = false;
        yield return new WaitForSeconds(cooldown);
        powerButton.interactable = true;
        isCooldown = false;
    }

    public void UpdateInventory(string powerUpName)
    {
        currentPowerUp = powerUpName;
        ShowPowerUpThumbnail(GetPowerUpSprite(powerUpName));
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

    private void ActivateFreezePowerUp()
    {
        FreezeGhosts();
    }

    private void FreezeGhosts()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject enemy in enemies)
        {
            PacMan3DMovement enemyMovement = enemy.GetComponent<PacMan3DMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.enabled = false;
                Rigidbody ghostRigidbody = enemy.GetComponent<Rigidbody>();
                if (ghostRigidbody != null)
                {
                    ghostRigidbody.velocity = Vector3.zero;
                    ghostRigidbody.angularVelocity = Vector3.zero;
                }

                Animator ghostAnimator = enemy.GetComponent<Animator>();
                if (ghostAnimator != null)
                {
                    ghostAnimator.enabled = false;
                }

                if (freezeEffectMaterial != null)
                {
                    // Apply freeze effect material or any other visual effect
                }

                StartCoroutine(ReEnableMovement(enemyMovement, 5f));
            }
        }
    }

    private IEnumerator ReEnableMovement(PacMan3DMovement enemyMovement, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemyMovement != null)
        {
            enemyMovement.enabled = true;
        }

        Animator ghostAnimator = enemyMovement.GetComponent<Animator>();
        if (ghostAnimator != null)
        {
            ghostAnimator.enabled = true;
        }

        if (freezeEffectMaterial != null)
        {
            // Remove freeze effect material or reset visual effect
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
            rb.velocity = playerTransform.forward * 10f;  // Set bullet speed
        }
    }

    private void ActivateSpeedBoostPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PacMan3DMovement movementScript = player.GetComponent<PacMan3DMovement>();

            if (movementScript != null)
            {
                movementScript.speed += 2.0f;
                StartCoroutine(ResetSpeedAfterDelay(movementScript, 5f));
            }
        }
    }

    private IEnumerator ResetSpeedAfterDelay(PacMan3DMovement movementScript, float delay)
    {
        yield return new WaitForSeconds(delay);
        movementScript.speed -= 2.0f;
    }

    private void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }

    private void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }

    private void ShowControlUI()
    {
        controlUI.SetActive(true);
    }

    private void HideAllRolePanels()
    {
        protagonistPanel.SetActive(false);
        antagonistInvisibilityPanel.SetActive(false);
        antagonistDashPanel.SetActive(false);
        antagonistTrapPanel.SetActive(false);
    }

    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    private void EnableJoystick()
    {
        if (joystickPrefab != null)
        {
            joystickPrefab.SetActive(true);
        }
    }

    private void StartTimer()
    {
        if (timerObject != null)
        {
            timerObject.SetActive(true);
        }
    }
}
