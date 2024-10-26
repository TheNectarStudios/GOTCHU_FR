using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BotMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    [Header("Bot Settings")]
    [Tooltip("Movement speed of the bot.")]
    public float movementSpeed = 3.5f; // Default speed, adjustable in the Inspector
    private bool trackingPlayer = false; // Whether the bot should start tracking the player

    private void Start()
    {
        // Get the NavMeshAgent component attached to this bot
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed; // Set the initial speed of the agent

        // Find the player with the "Player" tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Ensure there is a GameObject tagged 'Player' in the scene.");
        }

        // Start coroutine to begin tracking after 13 seconds
        StartCoroutine(StartTrackingAfterDelay(13f));
    }

    private void Update()
    {
        // If tracking is enabled and player exists, set the agent's destination to the player's position
        if (trackingPlayer && player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    // Coroutine to start tracking the player after a specified delay
    private IEnumerator StartTrackingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        trackingPlayer = true;
    }

    private void OnValidate()
    {
        // Update the NavMeshAgent's speed when changing in the Inspector
        if (agent != null)
        {
            agent.speed = movementSpeed;
        }
    }
}
