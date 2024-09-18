using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform protagonistSpawnPoint;
    public Transform antagonistSpawnPoint;
    public Button powerButton;
    private string currentPowerUp = null;

    public GameObject protagonistPrefab;
    public GameObject antagonistPrefab;
    public GameObject bulletPrefab; // Bullet prefab for shooting

    public Button buttonUp;
    public Button buttonDown;
    public Button buttonLeft;
    public Button buttonRight;

    public float bufferTime = 3.0f;
    private bool isCooldown = false;  // To handle cooldown across powers

    // UI Image for power-up thumbnail
    public Image powerUpThumbnail;

    // Sprites for different power-ups
    public Sprite freezeSprite;
    public Sprite bulletSprite;
    public Sprite speedBoostSprite;

    // Panels for each role and ability
    public GameObject protagonistPanel;
    public GameObject antagonistInvisibilityPanel;
    public GameObject antagonistDashPanel;
    public GameObject antagonistTrapPanel;

    // Control UI
    public GameObject controlUI;

    // Loading screen
    public GameObject loadingScreen;

    public GameObject TimerObject ; 

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            ShowLoadingScreen();
            StartCoroutine(WaitAndAssignRoles());
        }
    }

    private IEnumerator WaitAndAssignRoles()
    {
        yield return new WaitUntil(() => !protagonistPanel.activeSelf && 
                                         !antagonistInvisibilityPanel.activeSelf &&
                                         !antagonistDashPanel.activeSelf &&
                                         !antagonistTrapPanel.activeSelf);  // Wait until all role panels are hidden

        yield return new WaitForSeconds(bufferTime);  // Wait for the buffer time
        HideLoadingScreen();
        AssignRoles();
    }

    private void AssignRoles()
    {
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        int protagonistIndex = Random.Range(0, players.Count);

        for (int i = 0; i < players.Count; i++)
        {
            if (i == protagonistIndex)
            {
                photonView.RPC("SetProtagonist", players[i]);
            }
            else
            {
                photonView.RPC("SetAntagonist", players[i]);
            }
        }
    }

    [PunRPC]
    private void SetProtagonist()
    {
        GameObject protagonist = PhotonNetwork.Instantiate(protagonistPrefab.name, protagonistSpawnPoint.position, protagonistSpawnPoint.rotation);
        AssignButtonControls(protagonist);
        AssignCamera(protagonist);  // Attach camera to protagonist

        if (protagonist.GetComponent<PhotonView>().IsMine)
        {
            // Show the protagonist panel
            ShowPanel(protagonistPanel);
        }
    }

    [PunRPC]
    private void SetAntagonist()
    {
        GameObject antagonist = PhotonNetwork.Instantiate(antagonistPrefab.name, antagonistSpawnPoint.position, antagonistSpawnPoint.rotation);
        AssignButtonControls(antagonist);
        AssignCamera(antagonist);  // Attach camera to antagonist

        if (antagonist.GetComponent<PhotonView>().IsMine)
        {
            AssignAbilities(antagonist);
        }
    }

    private void AssignButtonControls(GameObject player)
    {
        if (player.GetComponent<PhotonView>().IsMine)
        {
            PacMan3DMovement movementScript = player.GetComponent<PacMan3DMovement>();
            if (movementScript != null)
            {
                buttonUp.onClick.RemoveAllListeners();
                buttonDown.onClick.RemoveAllListeners();
                buttonLeft.onClick.RemoveAllListeners();
                buttonRight.onClick.RemoveAllListeners();

                buttonUp.onClick.AddListener(() => movementScript.MoveUp());
                buttonDown.onClick.AddListener(() => movementScript.MoveDown());
                buttonLeft.onClick.AddListener(() => movementScript.MoveLeft());
                buttonRight.onClick.AddListener(() => movementScript.MoveRight());

                // Ensure the power button is always interactable
                powerButton.interactable = true;
                powerButton.onClick.RemoveAllListeners();
                powerButton.onClick.AddListener(ActivateStoredPowerUp);
            }
            else
            {
                Debug.LogError("PacMan3DMovement script not found on the player.");
            }
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
            currentPowerUp = null;  // Clear inventory after use
            HidePowerUpThumbnail(); // Hide the thumbnail after activation
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
        if (player.GetComponent<PhotonView>().IsMine)
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
    }

    private void AssignAbilities(GameObject player)
    {
        if (player.GetComponent<PhotonView>().IsMine)
        {
            // Make the power button available and assign abilities to the single button
            powerButton.interactable = true;
            powerButton.onClick.RemoveAllListeners();

            Invisibility invisibility = player.GetComponent<Invisibility>();
            Dash dash = player.GetComponent<Dash>();
            Trap trap = player.GetComponent<Trap>();

            if (invisibility != null)
            {
                powerButton.onClick.AddListener(() => ActivatePower(invisibility.ActivateInvisibility, invisibility.cooldownTime));
                ShowPanel(antagonistInvisibilityPanel);  // Show invisibility panel
            }
            if (dash != null)
            {
                powerButton.onClick.AddListener(() => ActivatePower(dash.ActivateDash, dash.cooldownTime));
                ShowPanel(antagonistDashPanel);  // Show dash panel
            }
            if (trap != null)
            {
                powerButton.onClick.AddListener(() => ActivatePower(trap.PlaceTrap, trap.cooldownTime));
                ShowPanel(antagonistTrapPanel);  // Show trap panel
            }

            // Enable control UI after role UI is hidden
            StartCoroutine(EnableControlUIAfterDelay());
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
        Debug.Log("Power-Up added to inventory: " + powerUpName);

        // Show the power-up thumbnail when picked up
        switch (powerUpName)
        {
            case "Freeze":
                ShowPowerUpThumbnail(freezeSprite);
                break;
            case "Bullet":
                ShowPowerUpThumbnail(bulletSprite);
                break;
            case "SpeedBoost":
                ShowPowerUpThumbnail(speedBoostSprite);
                break;
        }
    }

    private void ActivateFreezePowerUp()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject enemy in enemies)
        {
            // Get the PacMan3DMovement script and disable it
            PacMan3DMovement enemyMovement = enemy.GetComponent<PacMan3DMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.enabled = false;
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
            // Instantiate the bullet over the network
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, playerTransform.position, playerTransform.rotation, 0);

            PhotonView bulletPhotonView = bullet.GetComponent<PhotonView>();
            if (bulletPhotonView != null && bulletPhotonView.IsMine)
            {
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    // Get the last movement direction from the player
                    PacMan3DMovement movementScript = playerTransform.GetComponent<PacMan3DMovement>();
                    if (movementScript != null)
                    {
                        Vector3 movementDirection = movementScript.GetLastMovementDirection();
                        bulletRb.AddForce(movementDirection * 10f, ForceMode.Impulse); // Adjust force as needed
                    }
                    else
                    {
                        bulletRb.AddForce(playerTransform.forward * 10f, ForceMode.Impulse); // Default to forward if no movement direction
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to assign ownership or bullet PhotonView is null.");
            }
        }
    }

    private void ActivateSpeedBoostPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PacMan3DMovement playerMovement = player.GetComponent<PacMan3DMovement>();
            if (playerMovement != null)
            {
                float originalSpeed = playerMovement.speed;
                playerMovement.speed *= 2;  // Double the speed
                StartCoroutine(ResetSpeedAfterTime(playerMovement, originalSpeed, 5f));  // Speed boost lasts for 5 seconds
            }
        }
    }

    private IEnumerator ResetSpeedAfterTime(PacMan3DMovement playerMovement, float originalSpeed, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (playerMovement != null)
        {
            playerMovement.speed = originalSpeed;  // Reset to original speed
        }
    }

    private void ShowPanel(GameObject panel)
    {
        // Hide all panels first
        protagonistPanel.SetActive(false);
        antagonistInvisibilityPanel.SetActive(false);
        antagonistDashPanel.SetActive(false);
        antagonistTrapPanel.SetActive(false);

        // Show the specific panel
        if (panel != null)
        {
            panel.SetActive(true);
            StartCoroutine(HidePanelAfterDelay(panel, 4f));  // Hide the panel after 4 seconds
        }
    }

    private IEnumerator HidePanelAfterDelay(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);  // Hide the panel after the delay
        controlUI.SetActive(true) ; 
        TimerObject.SetActive(true) ; 
    }

    private void ShowLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
    }

    private void HideLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    private IEnumerator EnableControlUIAfterDelay()
    {
        yield return new WaitUntil(() => !protagonistPanel.activeSelf && 
                                         !antagonistInvisibilityPanel.activeSelf &&
                                         !antagonistDashPanel.activeSelf &&
                                         !antagonistTrapPanel.activeSelf);  // Wait until all role panels are hidden

        controlUI.SetActive(true);  // Enable the control UI
    }
}
