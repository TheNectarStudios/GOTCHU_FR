using UnityEngine;
using Photon.Pun;
using System.Collections;

public class AntagonistPearlDrop : MonoBehaviourPun
{
    public GameObject pearlPrefab;  // Reference to the pearl prefab
    public Transform pearlHolder;   // Where the pearl will be attached on the antagonist
    public float autoDropTime = 20f;  // Time after which the pearl will drop automatically
    private bool hasDroppedPearl = false;  // To track if the pearl has been dropped
    private GameObject attachedPearl;  // The pearl currently attached to the antagonist

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Attach the pearl to the antagonist when the game starts
            AttachPearl();

            // Start a coroutine to auto-drop the pearl after 20 seconds
            StartCoroutine(AutoDropPearlAfterTime(autoDropTime));
        }
    }

    private void Update()
    {
        // If the player presses the Enter key and the pearl hasn't been dropped yet
        if (photonView.IsMine && !hasDroppedPearl && Input.GetKeyDown(KeyCode.Return))
        {
            DropPearl();
        }
    }

    // Attach the pearl to the antagonist
    private void AttachPearl()
    {
        // Instantiate the pearl and parent it to the antagonist at the specified pearlHolder position
        attachedPearl = PhotonNetwork.Instantiate(pearlPrefab.name, pearlHolder.position, pearlHolder.rotation);
        attachedPearl.transform.SetParent(pearlHolder);  // Set the pearl as a child of the pearlHolder
    }

    // Drop the pearl
    private void DropPearl()
    {
        if (attachedPearl != null)
        {
            // Detach the pearl from the antagonist
            attachedPearl.transform.SetParent(null);

            // Allow physics to affect the dropped pearl (if necessary)
            Rigidbody pearlRigidbody = attachedPearl.GetComponent<Rigidbody>();
            if (pearlRigidbody != null)
            {
                pearlRigidbody.isKinematic = false;  // Enable physics
            }

            hasDroppedPearl = true;  // Mark the pearl as dropped
        }
    }

    // Automatically drop the pearl after a specified amount of time
    private IEnumerator AutoDropPearlAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (!hasDroppedPearl)
        {
            DropPearl();  // Automatically drop the pearl if it hasn't been dropped manually
        }
    }
}
