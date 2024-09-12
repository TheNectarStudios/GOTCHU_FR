using UnityEngine;
using System.Collections;
public class Freeze : MonoBehaviour
{
    private PacMan3DMovement playerMovement;
    public float freezeDuration = 3f;
    private GameObject[] ghosts;




    private void OnTriggerEnter(Collider collision)
    {
        //playerMovement = collision.gameObject.GetComponent<WASDMovement>();
        ActivatePowerUp();

    }

    public void ActivatePowerUp()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Player");


        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<PacMan3DMovement>().enabled = false;
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(UnfreezeAfterDelay());
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        yield return new WaitForSeconds(freezeDuration);

        ghosts = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<PacMan3DMovement>().enabled = true;
        }
        Destroy(gameObject);
    }
}
