using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.AI;
using NodeCanvas.Tasks.Actions;

public class DodgeAT : ActionTask
{
    // Variables

    public float dodgeDistance = 2f;
    public float dodgeSpeed = 5f;
    public float defaultDodge;
    public float rotationAngle = 30f;
    public float dodgeDuration = 0.5f;
    public BBParameter<EnemyHealth> health;
    private Vector3 dodgeTarget;
    private float dodgeEndTime;
    private bool isDodging = false;
    private NavMeshAgent navAgent;

    protected override string OnInit()
    {
        // Sets up default values

        navAgent = agent.GetComponent<NavMeshAgent>();
        defaultDodge = dodgeDistance;
        return null;
    }

    protected override void OnExecute()
    {
        // vector 3 values for different dodges

        dodgeDistance = defaultDodge;
        Vector3 backwardDirection = -agent.transform.forward * dodgeDistance;
        Vector3 sideDodge = (Random.Range(0, 2) == 0) ? agent.transform.right : -agent.transform.right;

        // Checks if there is a wall behind him to move differently based on surroundings

        if (Physics.Raycast(agent.transform.position, -agent.transform.forward, out RaycastHit hit, dodgeDistance))
        {
            dodgeDistance *= 1.5f;
            dodgeTarget = agent.transform.position + sideDodge * dodgeDistance;
        }
        else
        {
            dodgeTarget = agent.transform.position + backwardDirection;
        }

        // Uses navmesh to dodge better

        if (NavMesh.SamplePosition(dodgeTarget, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
        {
            dodgeTarget = navHit.position;
        }

        // dodges out from navmesh destionation for dodge time

        navAgent.speed = dodgeSpeed;
        navAgent.SetDestination(dodgeTarget);
        dodgeEndTime = Time.time + dodgeDuration;
        isDodging = true;
    }

    protected override void OnUpdate()
    {
        if (!isDodging) return;

        // Gets direction to dodge

        Vector3 directionToDodge = (dodgeTarget - agent.transform.position).normalized;
        if (directionToDodge != Vector3.zero)
        {
            // Sets target rotation

            Quaternion targetRotation = Quaternion.LookRotation(directionToDodge);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Reduces suspicion if the enemy dodges without getting hit

        if (Time.time >= dodgeEndTime || navAgent.remainingDistance <= 0.1f)
        {
            PlayerController playerController = health.value.player.GetComponent<PlayerController>();
            if (!health.value.tookDamage && playerController.isAttacking)
            {
                health.value.suspicion -= 1;
            }
            isDodging = false;
            EndAction(true);
        }
    }

    protected override void OnStop()
    {
        isDodging = false;
    }
}
