using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DumbBot : MonoBehaviour
{
    public string playerTag = "Player";
    public GameObject chasingClonePrefab; // Prefab for chasing clones
    public int numberOfChasingClones = 2; // Number of clones to spawn
    public float cloneLifetime = 10f; // Lifetime of the chasing clones

    public float chaseDuration = 5f;
    public float restDuration = 3f;
    public float wanderRadius = 5f; // Radius for random wandering
    public float chaseRadius = 10f; // Radius for detecting the player
    public float tiltSensitivity = 2f; // Sensitivity for animations

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Transform player;
    private bool isChasing = false;
    private bool isResting = false;
    private bool clonesSpawned = false; // To ensure clones are only spawned once per chase

    private float chaseTimer = 0f;
    private float restTimer = 0f;

    private float cloneSpawnCooldownTimer = 0f; // Timer to track cooldown
    private float cloneSpawnCooldownDuration = 40f; // Duration of the cooldown in seconds

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        FindPlayer();
    }

    private void Update()
    {
        if (isResting)
        {
            HandleResting();
        }
        else if (isChasing)
        {
            HandleChasing();
        }
        else
        {
            HandleWandering();
        }

        CheckForPlayerInRange();

        // Reduce cooldown timer over time
        if (cloneSpawnCooldownTimer > 0f)
        {
            cloneSpawnCooldownTimer -= Time.deltaTime;
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No player found with the specified tag.");
        }
    }

    private void HandleChasing()
    {
        if (player == null) return;

        chaseTimer += Time.deltaTime;
        navMeshAgent.SetDestination(player.position);
        UpdateTiltAnimation(); // Update tilting during chasing

        if (!clonesSpawned && cloneSpawnCooldownTimer <= 0f)
        {
            SpawnChasingClones();
            clonesSpawned = true;
        }

        if (chaseTimer >= chaseDuration)
        {
            isChasing = false;
            isResting = true;
            chaseTimer = 0f;
            navMeshAgent.ResetPath();
            clonesSpawned = false; // Reset for the next chase
        }

        UpdateAnimations();
    }

    private void HandleResting()
    {
        restTimer += Time.deltaTime;

        if (restTimer >= restDuration)
        {
            isResting = false;
            restTimer = 0f;
        }

        UpdateAnimations(false);
    }

    private void HandleWandering()
    {
        if (!navMeshAgent.hasPath)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }

        UpdateAnimations();
    }

    private void UpdateTiltAnimation()
    {
        Vector3 velocity = navMeshAgent.velocity;

        if (velocity.magnitude < 0.1f)
        {
            animator.SetFloat("TiltSide", 0f);
            animator.SetFloat("TiltDirection", 0f);
            return;
        }

        float angle = Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        if (angle >= 0f && angle < 90f)
        {
            animator.SetFloat("TiltDirection", 1f * tiltSensitivity);
            animator.SetFloat("TiltSide", 0f);
        }
        else if (angle >= 90f && angle < 180f)
        {
            animator.SetFloat("TiltSide", -1f * tiltSensitivity);
            animator.SetFloat("TiltDirection", 0f);
        }
        else if (angle >= 180f && angle < 270f)
        {
            animator.SetFloat("TiltDirection", -1f * tiltSensitivity);
            animator.SetFloat("TiltSide", 0f);
        }
        else if (angle >= 270f && angle < 360f)
        {
            animator.SetFloat("TiltSide", 1f * tiltSensitivity);
            animator.SetFloat("TiltDirection", 0f);
        }
    }

    private void UpdateAnimations(bool isMoving = true)
    {
        if (animator == null) return;

        Vector3 velocity = navMeshAgent.velocity;
        float speed = isMoving ? velocity.magnitude : 0f;

        animator.SetFloat("Speed", speed);
    }

    private void CheckForPlayerInRange()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRadius && !isResting)
        {
            isChasing = true;
            navMeshAgent.SetDestination(player.position);
        }
    }

    private void SpawnChasingClones()
    {
        if (chasingClonePrefab == null || cloneSpawnCooldownTimer > 0f || player == null) return;

        for (int i = 0; i < numberOfChasingClones; i++)
        {
            bool validPositionFound = false;
            Vector3 spawnPosition = Vector3.zero;

            for (int attempt = 0; attempt < 10; attempt++) // Try up to 10 times to find a valid position
            {
                Vector3 randomOffset = Random.insideUnitSphere * chaseRadius;
                randomOffset.y = 0; // Keep clones on the ground level

                Vector3 potentialPosition = player.position + randomOffset;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(potentialPosition, out hit, chaseRadius, NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;
                    validPositionFound = true;
                    break;
                }
            }

            if (validPositionFound)
            {
                GameObject clone = Instantiate(chasingClonePrefab, spawnPosition, Quaternion.identity);
                Destroy(clone, cloneLifetime);

                // Debug position to ensure it's correct
                Debug.Log($"Clone {i + 1} spawned at {spawnPosition}");
            }
            else
            {
                Debug.LogWarning($"Failed to find a valid spawn position for clone {i + 1}.");
            }
        }

        // Start cooldown timer after spawning clones
        cloneSpawnCooldownTimer = cloneSpawnCooldownDuration;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isChasing = true;
            isResting = false;
            chaseTimer = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
