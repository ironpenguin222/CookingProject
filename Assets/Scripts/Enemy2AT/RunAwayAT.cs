using NodeCanvas.Framework;
using ParadoxNotion.Design;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class RunAwayAT : ActionTask
{
    // Variable values

    public BBParameter<Transform> player;
    public float fleeDistance = 10f;
    public float maxFleeRange = 20f;
    public float arrivalThreshold = 1.5f;
    public int maxAttempts = 10;
    public TextMeshProUGUI speechBubble;

    private Vector3 fleeTarget;
    private bool pathSet = false;
    private NavMeshAgent navAgent;

    protected override void OnExecute()
    {
        // Sets up navagent
        if (navAgent == null)
        {
            navAgent = agent.GetComponent<NavMeshAgent>();
        }

        navAgent.speed = fleeDistance;

        // Alert speech bubble / if no player, then action ends

        speechBubble.text = "!";
        if (!player.value)
        {
            EndAction(false);
            return;
        }

        // Enemy attempts to run away from player

        for (int i = 0; i < maxAttempts; i++)
        {
            // Location away from player

            Vector3 awayFromPlayer = (navAgent.transform.position - player.value.position).normalized;

            // Movement offset

            Vector3 randomOffset = Random.insideUnitSphere * 5f;
            randomOffset.y = 0f;

            // Set up where we want them to go

            Vector3 candidate = navAgent.transform.position + (awayFromPlayer + randomOffset).normalized * fleeDistance;

            // Test and set destination

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, maxFleeRange, NavMesh.AllAreas))
            {
                navAgent.SetDestination(hit.position);
                pathSet = true;
                return;
            }
        }

        EndAction(false);
    }

    protected override void OnUpdate()
    {
        // Checks to make sure that the path is pathing

        if (!pathSet) return;

        if (!navAgent.pathPending && navAgent.remainingDistance <= arrivalThreshold)
        {
            EndAction(true);
        }
    }

    protected override void OnStop()
    {
        // Resets path when stopped

        if (navAgent.hasPath)
        {
            navAgent.ResetPath();
        }
    }
}

