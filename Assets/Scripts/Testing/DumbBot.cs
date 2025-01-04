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

        if (!clonesSpawned)
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
        if (chasingClonePrefab == null) return;

        for (int i = 0; i < numberOfChasingClones; i++)
        {
            Vector3 spawnPosition = transform.position; // Spawn at DumbBot's position

            GameObject clone = Instantiate(chasingClonePrefab, spawnPosition, Quaternion.identity);

            // Debug position to ensure it's correct
            Debug.Log($"Clone {i + 1} spawned at {spawnPosition}");

            Destroy(clone, cloneLifetime);
        }
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
