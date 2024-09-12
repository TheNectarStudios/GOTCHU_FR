using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseControls : MonoBehaviour
{
    private PacMan3DMovement playerMovement;
    public float reverseDuration = 5f;
    private GameObject[] ghosts;




    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Hero(Clone)")
        {
            ActivatePowerUp();
        }
    }

    public void ActivatePowerUp()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Player");


        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<PacMan3DMovement>().speed *= -1;
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(UnfreezeAfterDelay());
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        yield return new WaitForSeconds(reverseDuration);

        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<PacMan3DMovement>().speed *= -1;
        }
        Destroy(gameObject);
    }
}
