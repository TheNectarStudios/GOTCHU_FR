using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class SpawnManager : MonoBehaviourPunCallbacks
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

    // Reference for the joystick UI
    public GameObject joystickPrefab;

    public GameObject antagonistInvisibilityPanel;
    public GameObject antagonistDashPanel;
    public GameObject antagonistTrapPanel;


    private ShaderManager shaderManager;

    private int spawnedGhostsCount = 0;  // Keep track of ghosts spawned

    private void Start()
    {
        shaderManager = FindObjectOfType<ShaderManager>();
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

        photonView.RPC("ShowControlUI", RpcTarget.All);
        yield return new WaitForSeconds(2f);
        photonView.RPC("StartTimer", RpcTarget.All);
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
        AssignCamera(protagonist);
        if (protagonist.GetComponent<PhotonView>().IsMine)
        {
            ShowPanel(protagonistPanel);
            EnableJoystick(); // Enable the joystick for the protagonist
        }
    }

    [PunRPC]
    private void SetAntagonist()
    {
        if (spawnedGhostsCount < antagonistSpawnPoints.Length)
        {
            Transform spawnPoint = antagonistSpawnPoints[spawnedGhostsCount];
            GameObject antagonist = PhotonNetwork.Instantiate(antagonistPrefab.name, spawnPoint.position, spawnPoint.rotation);
            AssignButtonControls(antagonist);
            AssignCamera(antagonist);

            if (antagonist.GetComponent<PhotonView>().IsMine)
            {
                AssignAbilities(antagonist);
                EnableJoystick(); // Enable the joystick for the antagonist
            }
            if (spawnedGhostsCount < 2)
            {
                spawnedGhostsCount++; 
            }
        }
    }
   private void AssignButtonControls(GameObject player)
    {
        if (player.GetComponent<PhotonView>().IsMine)
        {
            PacMan3DMovement movementScript = player.GetComponent<PacMan3DMovement>();
            if (movementScript != null)
            {

                powerButton.interactable = true;
                powerButton.onClick.RemoveAllListeners();
                powerButton.onClick.AddListener(ActivateStoredPowerUp);
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
        photonView.RPC("ShowPowerUpThumbnailRPC", RpcTarget.All, powerUpName);
    }
    

      private void ActivateFreezePowerUp()
    {
        photonView.RPC("FreezeGhostsAcrossNetwork", RpcTarget.AllBuffered);
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
                if (shaderManager != null)
                {
                    shaderManager.SetTilingMultiplier(shaderManager.freezeEffectMaterial, shaderManager.visibleValue);
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
            enemyMovement.enabled = true;
        }

        Animator ghostAnimator = enemyMovement.GetComponent<Animator>();
        if (ghostAnimator != null)
        {
            ghostAnimator.enabled = true; 
        }
        if (shaderManager != null)
        {
            shaderManager.SetTilingMultiplier(shaderManager.freezeEffectMaterial, shaderManager.invisibleValue);
        }
    }


   private void ActivateBulletPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && player.GetComponent<PhotonView>().IsMine)
        {
            FireBullet(player.transform);
        }
    }


    void FireBullet(Transform playerTransform)
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, playerTransform.position, playerTransform.rotation, 0);
            PhotonView bulletPhotonView = bullet.GetComponent<PhotonView>();
            if (bulletPhotonView != null && bulletPhotonView.Owner != PhotonNetwork.LocalPlayer)
            {
                bulletPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            PacMan3DMovement playerMovement = playerTransform.GetComponent<PacMan3DMovement>();
            if (playerMovement != null)
            {
                Vector3 movementDirection = playerMovement.GetLastMovementDirection();
                if (movementDirection != Vector3.zero)
                {
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.velocity = movementDirection * 10f;  
                }
                else
                {
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.velocity = playerTransform.forward * 10f;  // Default bullet speed
                }
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
    } private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

     private void EnableJoystick()
    {
        if (joystickPrefab != null && PhotonNetwork.IsMasterClient)
        {
            GameObject joystickInstance = PhotonNetwork.Instantiate(joystickPrefab.name, Vector3.zero, Quaternion.identity);
            joystickInstance.transform.SetParent(GameObject.Find("Canvas").transform, false); // Attach to canvas
            joystickInstance.SetActive(true);
            Debug.Log("Joystick enabled for player.");
        }
        else
        {
            Debug.LogWarning("Joystick prefab not assigned or not the master client!");
        }
    }

}
