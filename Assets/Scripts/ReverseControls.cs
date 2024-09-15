using Photon.Pun;

using UnityEngine;
public class BulletPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.UpdateInventory("Bullet");
                Destroy(gameObject);
            }
        }
    }
}
