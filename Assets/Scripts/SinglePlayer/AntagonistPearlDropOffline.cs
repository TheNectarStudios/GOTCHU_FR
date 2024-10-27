using UnityEngine;
using System.Collections;

public class AntagonistPearlDropOffline : MonoBehaviour
{
    public GameObject pearlPrefab;
    public GameObject dropAnimationPrefab;
    public Transform pearlHolder;
    
    private bool hasDroppedPearl = false;
    private GameObject attachedPearl;
    private BotMovement botMovement;

    private void Start()
    {
        botMovement = GetComponent<BotMovement>();
        if (pearlHolder == null)
        {
            Debug.LogWarning("Pearl holder is not assigned. Please assign a Transform for pearl placement.");
            return;
        }

        AttachPearl();
        StartCoroutine(AutoDropPearlAfterTime(20f)); // Adjusted timing to 10 seconds to align with bot strategy
    }

    private void AttachPearl()
    {
        if (pearlPrefab != null && pearlHolder != null)
        {
            attachedPearl = Instantiate(pearlPrefab, pearlHolder.position, pearlHolder.rotation);
            attachedPearl.transform.SetParent(pearlHolder);
        }
        else
        {
            Debug.LogWarning("Pearl prefab or pearl holder is missing.");
        }
    }

    private void DropPearl()
    {
        if (attachedPearl != null && !hasDroppedPearl)
        {
            attachedPearl.transform.SetParent(null);
            
            // Allow gravity on pearl if Rigidbody is available
            Rigidbody pearlRigidbody = attachedPearl.GetComponent<Rigidbody>();
            if (pearlRigidbody != null)
            {
                pearlRigidbody.isKinematic = false;
            }

            hasDroppedPearl = true;

            // Play drop animation if available
            if (dropAnimationPrefab != null)
            {
                Instantiate(dropAnimationPrefab, attachedPearl.transform.position, Quaternion.identity);
            }

            // Inform the bot to guard the pearl's position
            if (botMovement != null)
            {
                botMovement.StartGuarding(attachedPearl.transform.position);
            }
            else
            {
                Debug.LogWarning("BotMovement script not found on this GameObject.");
            }
        }
    }

    private IEnumerator AutoDropPearlAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (!hasDroppedPearl)
        {
            DropPearl();
        }
    }
}
