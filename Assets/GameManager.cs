using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;  // Singleton instance

    public GameObject moveUpButton;
    public GameObject moveDownButton;
    public GameObject moveLeftButton;
    public GameObject moveRightButton;

    private PacMan3DMovement localPlayerScript;

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find the local player instance and get the script
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer != null)
        {
            GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
            if (localPlayer != null)
            {
                localPlayerScript = localPlayer.GetComponent<PacMan3DMovement>();
                SetupButtons();
            }
        }
    }

    void SetupButtons()
    {
        // Set button click listeners
        if (moveUpButton != null)
        {
            moveUpButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => localPlayerScript.MoveUp());
        }

        if (moveDownButton != null)
        {
            moveDownButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => localPlayerScript.MoveDown());
        }

        if (moveLeftButton != null)
        {
            moveLeftButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => localPlayerScript.MoveLeft());
        }

        if (moveRightButton != null)
        {
            moveRightButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => localPlayerScript.MoveRight());
        }
    }
}
  