using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BotMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    [Header("Bot Settings")]
    public float movementSpeed = 3.5f;
    public float acceleration = 8f;
    public float guardRadius = 5f;
    public float timelineBufferTime = 3f;
    private bool trackingPlayer = false;
    private bool guardingPearl = false;
    private Vector3 guardPosition;
    private bool isPatrolling = false;
    public float separationRadius = 3f;
    public float pathDiversionTime = 4f;
    private float proximityTimer = 0f;
    private bool isFrozen = false;

    [Header("Invisibility Settings")]
    private InvisibilityOffline invisibilityScript;

    [Header("UFO Rotation Settings")]
    public float pitchAmount = 15f; // Maximum pitch angle
    public float yawAmount = 15f;   // Maximum yaw angle
    public float rotationSmoothing = 5f; // How smoothly the UFO tilts

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = 120f;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Ensure there is a GameObject tagged 'Player' in the scene.");
        }

        invisibilityScript = GetComponent<InvisibilityOffline>();
        if (invisibilityScript == null)
        {
            Debug.LogError("InvisibilityOffline script not found on the bot.");
        }

        StartCoroutine(TimelineBuffer(timelineBufferTime));
    }

    private void Update()
    {
        if (isFrozen)
        {
            return;
        }

        if (guardingPearl)
        {
            if (!isPatrolling)
            {
                StartCoroutine(PatrolAroundPearl());
            }
            else if (trackingPlayer && player != null)
            {
                if (Random.value < 0.3f)
                {
                    agent.SetDestination(player.position);
                }
            }

            CheckAndDiversifyPath();

            if (invisibilityScript != null)
            {
                invisibilityScript.ActivateInvisibility();
            }
        }
        else if (trackingPlayer && player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        // Apply UFO pitch and yaw
        ApplyUFORotation();
    }

    private void ApplyUFORotation()
    {
        Vector3 velocity = agent.velocity;
        if (velocity.magnitude > 0.1f)
        {
            // Calculate pitch and yaw based on velocity
            float pitch = Mathf.Clamp(-velocity.z * pitchAmount, -pitchAmount, pitchAmount);
            float yaw = Mathf.Clamp(velocity.x * yawAmount, -yawAmount, yawAmount);

            // Create target rotation with pitch and yaw
            Quaternion targetRotation = Quaternion.Euler(pitch, transform.eulerAngles.y, yaw);

            // Smoothly interpolate to the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothing);
        }
    }

    private IEnumerator TimelineBuffer(float bufferDuration)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(bufferDuration);
        agent.isStopped = false;
        trackingPlayer = true;
    }

    public void StartGuarding(Vector3 pearlPosition)
    {
        guardingPearl = true;
        guardPosition = pearlPosition;
        isPatrolling = false;
    }

    private IEnumerator PatrolAroundPearl()
    {
        isPatrolling = true;
        while (guardingPearl)
        {
            if (isFrozen)
            {
                yield return null;
                continue;
            }

            Vector3 patrolPoint = guardPosition + Random.insideUnitSphere * guardRadius;
            patrolPoint.y = guardPosition.y;
            agent.SetDestination(patrolPoint);

            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
        isPatrolling = false;
    }

    private void CheckAndDiversifyPath()
    {
        Collider[] nearbyAntagonists = Physics.OverlapSphere(transform.position, separationRadius);

        foreach (Collider col in nearbyAntagonists)
        {
            BotMovement otherBot = col.GetComponent<BotMovement>();
            if (otherBot != null && otherBot != this && otherBot.guardingPearl)
            {
                proximityTimer += Time.deltaTime;
                if (proximityTimer > pathDiversionTime)
                {
                    Vector3 newPatrolPoint = guardPosition + Random.insideUnitSphere * guardRadius * 1.5f;
                    newPatrolPoint.y = guardPosition.y;
                    agent.SetDestination(newPatrolPoint);
                    proximityTimer = 0f;
                }
                return;
            }
        }
        proximityTimer = 0f;
    }

    public void FreezeBot()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        isFrozen = true;
        Debug.Log("Bot frozen: " + gameObject.name);
    }

    public void UnfreezeBot()
    {
        if (agent != null)
        {
            agent.isStopped = false;
        }
        isFrozen = false;
        Debug.Log("Bot unfrozen: " + gameObject.name);
    }
}
