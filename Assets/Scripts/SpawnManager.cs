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
    public GameObject bulletPrefab;

    public Button buttonUp;
    public Button buttonDown;
    public Button buttonLeft;
    public Button buttonRight;

    public float bufferTime = 3.0f;
    private bool isCooldown = false;

    public Image powerUpThumbnail;
    public Sprite freezeSprite;
    public Sprite bulletSprite;
    public Sprite speedBoostSprite;

    public GameObject protagonistPanel;
    public GameObject antagonistInvisibilityPanel;
    public GameObject antagonistDashPanel;
    public GameObject antagonistTrapPanel;

    public GameObject controlUI;
    public GameObject loadingScreen;

    public GameObject timerObject; 
   
    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ShowLoadingScreenRPC", RpcTarget.All);
            StartCoroutine(WaitAndAssignRoles());
        }
    }

    private IEnumerator WaitAndAssignRoles()
    {
        yield return new WaitForSeconds(bufferTime);
        photonView.RPC("HideLoadingScreenRPC", RpcTarget.All);

        // Hide all role panels before showing new ones
        photonView.RPC("HideAllRolePanelsRPC", RpcTarget.All);

        // Assign roles
        AssignRoles();

        // Wait a short period to ensure all panels are hidden
        yield return new WaitForSeconds(3f);
        photonView.RPC("HideAllRolePanelsRPC", RpcTarget.All);
        
        // Start the timer (renamed to match the method name)
        photonView.RPC("StartTimer", RpcTarget.All);

        // Enable the control UI after hiding role panels
        photonView.RPC("ShowControlUI", RpcTarget.All);
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
        Debug.Log("Protagonist Role Assigned");

        GameObject protagonist = PhotonNetwork.Instantiate(protagonistPrefab.name, protagonistSpawnPoint.position, protagonistSpawnPoint.rotation);
        AssignButtonControls(protagonist);
        AssignCamera(protagonist);

        if (protagonist.GetComponent<PhotonView>().IsMine)
        {
            ShowPanel(protagonistPanel);
        }
    }

    [PunRPC]
    private void SetAntagonist()
    {
        Debug.Log("Antagonist Role Assigned");

        GameObject antagonist = PhotonNetwork.Instantiate(antagonistPrefab.name, antagonistSpawnPoint.position, antagonistSpawnPoint.rotation);
        AssignButtonControls(antagonist);
        AssignCamera(antagonist);

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
            currentPowerUp = null;
            photonView.RPC("HidePowerUpThumbnailRPC", RpcTarget.All);
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

    [PunRPC]
    private void ShowPowerUpThumbnailRPC(string powerUpType)
    {
        Sprite powerUpSprite = null;
        switch (powerUpType)
        {
            case "Freeze":
                powerUpSprite = freezeSprite;
                break;
            case "Bullet":
                powerUpSprite = bulletSprite;
                break;
            case "SpeedBoost":
                powerUpSprite = speedBoostSprite;
                break;
        }

        ShowPowerUpThumbnail(powerUpSprite);
    }

    [PunRPC]
    private void HidePowerUpThumbnailRPC()
    {
        HidePowerUpThumbnail();
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

        photonView.RPC("ShowPowerUpThumbnailRPC", RpcTarget.All, powerUpName);
    }

    private void ActivateFreezePowerUp()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject enemy in enemies)
        {
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

    void FireBullet(Transform playerTransform)
    {
        if (bulletPrefab != null)
        {
            // Instantiate the bullet at the player's position
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, playerTransform.position, playerTransform.rotation, 0);

            // Get the player's movement script to determine the direction
            PacMan3DMovement playerMovement = playerTransform.GetComponent<PacMan3DMovement>();

            // Check if the movement script exists
            if (playerMovement != null)
            {
                // Use the method to get the last movement direction
                Vector3 movementDirection = playerMovement.GetLastMovementDirection();

                // If the movement direction is valid, apply it to the bullet's velocity
                if (movementDirection != Vector3.zero)
                {
                    Debug.Log("Bullet fired in direction: " + movementDirection);
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.velocity = movementDirection * 10f;  // Set bullet speed
                }
                else
                {
                    Debug.LogError("Player is not moving, bullet fired in default forward direction");
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.velocity = playerTransform.forward * 10f;  // Set bullet speed
                }
            }
            else
            {
                Debug.LogError("PacMan3DMovement script not found on player");
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
                playerMovement.speed *= 2;  // Example of speed boost
                StartCoroutine(ResetSpeedBoost(playerMovement, 5f));
            }
        }
    }

    private IEnumerator ResetSpeedBoost(PacMan3DMovement playerMovement, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerMovement != null)
        {
            playerMovement.speed /= 2;  // Reset speed
        }
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    [PunRPC]
    private void ShowLoadingScreenRPC()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
    }

    [PunRPC]
    private void HideLoadingScreenRPC()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    [PunRPC]
    private void HideAllRolePanelsRPC()
    {
        if (protagonistPanel != null)
        {
            protagonistPanel.SetActive(false);
        }
        if (antagonistInvisibilityPanel != null)
        {
            antagonistInvisibilityPanel.SetActive(false);
        }
        if (antagonistDashPanel != null)
        {
            antagonistDashPanel.SetActive(false);
        }
        if (antagonistTrapPanel != null)
        {
            antagonistTrapPanel.SetActive(false);
        }

    }

    [PunRPC]
    private void ShowControlUI()
    {
        if (controlUI != null)
        {
            controlUI.SetActive(true);
        }
    }
    [PunRPC]
    private void StartTimer()
    {
        if (timerObject != null)
        {
            timerObject.SetActive(true);
        }
    }

}
