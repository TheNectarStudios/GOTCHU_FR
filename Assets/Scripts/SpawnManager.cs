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

    // Add multiple spawner points for antagonists
    public Transform[] antagonistSpawnPoints; // Array of antagonist spawners
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

    private ShaderManager shaderManager;

    private int spawnedGhostsCount = 0;  // Keep track of ghosts spawned

    // Audio components
    public AudioSource audioSource; // Assign this in the Inspector
    public AudioClip freezeAudioClip;
    public AudioClip bulletAudioClip;
    public AudioClip speedBoostAudioClip;

    private void Start()
    {
        // Find ShaderManager in the scene and assign it
        shaderManager = FindObjectOfType<ShaderManager>();

        // Check if ShaderManager was found
        if (shaderManager == null)
        {
            Debug.LogError("ShaderManager not found in the scene!");
        }

        // If connected to Photon, in a room, and the current client is the Master
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            // Call the RPC to show the loading screen across all clients
            photonView.RPC("ShowLoadingScreenRPC", RpcTarget.All);

            // Start the coroutine to assign roles
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

        // Check the number of ghosts spawned so far
        if (spawnedGhostsCount < antagonistSpawnPoints.Length)
        {
            Transform spawnPoint = antagonistSpawnPoints[spawnedGhostsCount]; // Use a different spawn point
            GameObject antagonist = PhotonNetwork.Instantiate(antagonistPrefab.name, spawnPoint.position, spawnPoint.rotation);
            AssignButtonControls(antagonist);
            AssignCamera(antagonist);

            if (antagonist.GetComponent<PhotonView>().IsMine)
            {
                AssignAbilities(antagonist);
            }

            // Only increase the count for the first 2 spawners
            if (spawnedGhostsCount < 2)
            {
                spawnedGhostsCount++; // Increase spawned ghosts count
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
                PlayAudio(freezeAudioClip);
                ActivateFreezePowerUp();
            }
            else if (currentPowerUp == "Bullet")
            {
                PlayAudio(bulletAudioClip);
                ActivateBulletPowerUp();
            }
            else if (currentPowerUp == "SpeedBoost")
            {
                PlayAudio(speedBoostAudioClip);
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
    }

    private void ActivateBulletPowerUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && player.GetComponent<PhotonView>().IsMine)
        {
            FireBullet(player.transform);
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

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    [PunRPC]
    private void FreezeGhostsAcrossNetwork()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            if (ghost.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = freezeEffectMaterial;
                StartCoroutine(ResetGhostMaterialAfterDelay(ghost, 3f));
            }
        }
    }

    private IEnumerator ResetGhostMaterialAfterDelay(GameObject ghost, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ghost.TryGetComponent(out MeshRenderer meshRenderer))
        {
            // Reset to the original material (assuming you store it)
            // meshRenderer.material = originalMaterial; // Uncomment and set original material
        }
    }

    private void FireBullet(Transform playerTransform)
    {
        Vector3 bulletDirection = playerTransform.forward; // Assuming forward direction is the desired bullet direction
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, playerTransform.position, Quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(bulletDirection * 10f, ForceMode.Impulse); // Change 10f to your desired bullet speed
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
    private void ShowControlUI()
    {
        if (controlUI != null)
        {
            controlUI.SetActive(true);
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
    private void StartTimer()
    {
        // Your timer logic here
        Debug.Log("Timer started.");
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }
}
