using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

public class RunAwayAT : ActionTask
{
    public BBParameter<Transform> player;
    public float fleeDistance = 10f;
    public float maxFleeRange = 20f;
    public float arrivalThreshold = 1.5f;
    public int maxAttempts = 10;
    public GameObject alert;

    private Vector3 fleeTarget;
    private bool pathSet = false;
    private NavMeshAgent navAgent;
    protected override void OnExecute()
    {
        if (navAgent == null)
        {
            navAgent = agent.GetComponent<NavMeshAgent>();
        }
        alert.SetActive(true);
        if (!player.value)
        {
            EndAction(false);
            return;
        }

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 awayFromPlayer = (navAgent.transform.position - player.value.position).normalized;
            Vector3 randomOffset = Random.insideUnitSphere * 5f;
            randomOffset.y = 0f;

            Vector3 candidate = navAgent.transform.position + (awayFromPlayer + randomOffset).normalized * fleeDistance;

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
        if (!pathSet) return;

        if (!navAgent.pathPending && navAgent.remainingDistance <= arrivalThreshold)
        {
            alert.SetActive(false);
            EndAction(true);
        }
    }

    protected override void OnStop()
    {
        if (navAgent.hasPath)
        {
            navAgent.ResetPath();
        }
    }
}

