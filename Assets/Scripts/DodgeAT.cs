using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.AI;
using NodeCanvas.Tasks.Actions;

public class DodgeAT : ActionTask
{
    public float dodgeDistance = 2f;
    public float dodgeSpeed = 5f;
    public float defaultDodge;
    public float rotationAngle = 30f;
    public float dodgeDuration = 0.5f;

    private Vector3 dodgeTarget;
    private float dodgeEndTime;
    private bool isDodging = false;
    private NavMeshAgent navAgent;

    protected override string OnInit()
    {
        navAgent = agent.GetComponent<NavMeshAgent>();
        defaultDodge = dodgeDistance;
        return null;
    }

    protected override void OnExecute()
    {
        dodgeDistance = defaultDodge;
        Vector3 backwardDirection = -agent.transform.forward * dodgeDistance;
        Vector3 sideDodge = (Random.Range(0, 2) == 0) ? agent.transform.right : -agent.transform.right;

        if (Physics.Raycast(agent.transform.position, -agent.transform.forward, out RaycastHit hit, dodgeDistance))
        {
            dodgeDistance *= 1.5f;
            dodgeTarget = agent.transform.position + sideDodge * dodgeDistance;
        }
        else
        {
            dodgeTarget = agent.transform.position + backwardDirection;
        }

        if (NavMesh.SamplePosition(dodgeTarget, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
        {
            dodgeTarget = navHit.position;
        }

        navAgent.speed = dodgeSpeed;
        navAgent.SetDestination(dodgeTarget);
        dodgeEndTime = Time.time + dodgeDuration;
        isDodging = true;
    }

    protected override void OnUpdate()
    {
        if (!isDodging) return;

        Vector3 directionToDodge = (dodgeTarget - agent.transform.position).normalized;
        if (directionToDodge != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToDodge);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (Time.time >= dodgeEndTime || navAgent.remainingDistance <= 0.1f)
        {
            isDodging = false;
            EndAction(true);
        }
    }

    protected override void OnStop()
    {
        isDodging = false;
    }
}
