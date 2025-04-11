using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

public class FireArrowAT : ActionTask
{
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public BBParameter<Transform> target;

    public float shootCooldown = 2f;
    public float rotationSpeed = 5f;
    public float facingThreshold = 0.98f;

    private float lastShotTime;
    private bool hasShot = false;
    private NavMeshAgent navAgent;

    protected override void OnExecute()
    {
        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        hasShot = false;
    }

    protected override void OnUpdate()
    {
        if (target.value == null || shootPoint == null || arrowPrefab == null)
        {
            EndAction(false);
            return;
        }

        RotateTowardsTarget();

        if (!hasShot && IsFacingTarget() && Time.time - lastShotTime >= shootCooldown)
        {
            ShootArrow();
            lastShotTime = Time.time;
            hasShot = true;
            EndAction(true);
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = (target.value.position - agent.transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private bool IsFacingTarget()
    {
        Vector3 toTarget = (target.value.position - agent.transform.position).normalized;
        toTarget.y = 0f;
        float dot = Vector3.Dot(agent.transform.forward, toTarget);
        return dot >= facingThreshold;
    }

    private void ShootArrow()
    {
        GameObject arrow = GameObject.Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (target.value.position - shootPoint.position).normalized;
            rb.velocity = direction * 20f;
        }
    }
}
