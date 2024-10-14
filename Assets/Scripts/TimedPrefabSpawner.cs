using System.Collections;
using UnityEngine;

public class TimedPrefabSpawner : MonoBehaviour
{
    // Public variables to control from the editor
    public GameObject prefabToSpawn;  // The prefab to instantiate
    public Transform spawnPoint;      // The spawn location
    public float spawnDelay = 5f;     // Time in seconds before the prefab is instantiated

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to delay the spawning
        StartCoroutine(SpawnAfterDelay());
    }

    // Coroutine to wait for a specified time before spawning the prefab
    IEnumerator SpawnAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(spawnDelay);

        // Instantiate the prefab at the spawn point
        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
    }
}
