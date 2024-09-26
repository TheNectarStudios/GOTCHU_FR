using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform protagonistSpawnPoint;
    public Material freezeEffectMaterial;

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

        photonView.RPC("HideAllRolePanelsRPC", RpcTarget.All);

        AssignRoles();

        yield return new WaitForSeconds(3f);
        photonView.RPC("HideAllRolePanelsRPC", RpcTarget.All);
        photonView.RPC("StartTimer", RpcTarget.All);

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
        photonView.RPC("FreezeGhostsAcrossNetwork", RpcTarget.AllBuffered);
        photonView.RPC("ApplyFreezeEffectForAntagonists", RpcTarget.All);
    }





    [PunRPC]
    private void FreezeGhostsAcrossNetwork()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject enemy in enemies)
        {
            PacMan3DMovement enemyMovement = enemy.GetComponent<PacMan3DMovement>();
            if (enemyMovement != null)
            {
                Debug.Log("Disabling movement for ghost: " + enemy.name);
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

                StartCoroutine(ReEnableMovement(enemyMovement, 5f, enemy.name));
            }
        }
    }


    private IEnumerator ReEnableMovement(PacMan3DMovement enemyMovement, float delay, string enemyName)
    {
        yield return new WaitForSeconds(delay);

        if (enemyMovement != null)
        {
            Debug.Log("Re-enabling movement for ghost: " + enemyName);
            enemyMovement.enabled = true;
        }

        Animator ghostAnimator = enemyMovement.GetComponent<Animator>();
        if (ghostAnimator != null)
        {
            ghostAnimator.enabled = true; 
        }
    }

   private void ActivateBulletPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && player.GetComponent<PhotonView>().IsMine)
        {
            // Pass the player's transform to FireBullet
            FireBullet(player.transform);
        }
    }


    void FireBullet(Transform playerTransform)
    {
        if (bulletPrefab != null)
        {
            // Instantiate the bullet at the player's position
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, playerTransform.position, playerTransform.rotation, 0);

            // Get the PhotonView of the bullet
            PhotonView bulletPhotonView = bullet.GetComponent<PhotonView>();

            // Transfer ownership of the bullet to the player who fired it
            if (bulletPhotonView != null && bulletPhotonView.Owner != PhotonNetwork.LocalPlayer)
            {
                bulletPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            // Get the player's movement script to determine the direction
            PacMan3DMovement playerMovement = playerTransform.GetComponent<PacMan3DMovement>();

            // Ensure the movement script exists
            if (playerMovement != null)
            {
                // Use the method to get the last movement direction
                Vector3 movementDirection = playerMovement.GetLastMovementDirection();

                // Apply direction to the bullet's velocity
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
                    rb.velocity = playerTransform.forward * 10f;  // Default bullet speed
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

        if (player != null && player.GetComponent<PhotonView>().IsMine)
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

    [PunRPC]
    private void ShowLoadingScreenRPC()
    {
        loadingScreen.SetActive(true);
    }

    [PunRPC]
    private void HideLoadingScreenRPC()
    {
        loadingScreen.SetActive(false);
    }

    [PunRPC]
    private void ShowControlUI()
    {
        controlUI.SetActive(true);
    }

    [PunRPC]
    private void HideAllRolePanelsRPC()
    {
        protagonistPanel.SetActive(false);
        antagonistInvisibilityPanel.SetActive(false);
        antagonistDashPanel.SetActive(false);
        antagonistTrapPanel.SetActive(false);
    }

    [PunRPC]
    private void StartTimer()
    {
        timerObject.SetActive(true);
    }




[PunRPC]

private void ApplyFreezeEffectForAntagonists()
{
    if (!PhotonNetwork.LocalPlayer.IsMasterClient) // Assuming master is protagonist
    {
        ShaderManager shaderManager = FindObjectOfType<ShaderManager>();
        if (shaderManager != null)
        {
            shaderManager.SetFreezeEffectForAntagonists(true);  // Make Ghosts visible with freeze effect
        }
    }
}

[PunRPC]
private void RemoveFreezeEffectForAntagonists()
{
    if (!PhotonNetwork.LocalPlayer.IsMasterClient) // Assuming master is protagonist
    {
        ShaderManager shaderManager = FindObjectOfType<ShaderManager>();
        if (shaderManager != null)
        {
            shaderManager.SetFreezeEffectForAntagonists(false);  // Make Ghosts invisible
        }
    }
}


    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

}
