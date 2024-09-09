using UnityEngine;
using System.Collections;
public class Freeze : MonoBehaviour
{
    private WASDMovement playerMovement;
    public float freezeDuration = 3f;
    private GameObject[] ghosts;

    


    private void OnCollisionEnter(Collision collision)
    {
        //playerMovement = collision.gameObject.GetComponent<WASDMovement>();
        ActivatePowerUp();
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

    }

    public void ActivatePowerUp()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");


        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<WASDMovement>().enabled = false;
        }

        StartCoroutine(UnfreezeAfterDelay());
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        yield return new WaitForSeconds(freezeDuration);

        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<WASDMovement>().enabled = true;
        }
        Destroy(gameObject);
    }
}
