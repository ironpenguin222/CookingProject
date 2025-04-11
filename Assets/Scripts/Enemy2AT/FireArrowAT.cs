using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using TMPro;

public class FireArrowAT : ActionTask
{
    // Variable Values

    public GameObject arrowPrefab;
    public Transform shootPoint;
    public BBParameter<Transform> target;

    public float shootCooldown = 2f;
    public float rotationSpeed = 5f;
    public float facingThreshold = 0.98f;
    public TextMeshProUGUI speechBubble;

    private float lastShotTime;
    private bool hasShot = false;
    private NavMeshAgent navAgent;

    protected override void OnExecute()
    {
        // Setup NavAgent

        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        hasShot = false;
    }

    protected override void OnUpdate()
    {
        // Make sure that everything is properly setup

        if (target.value == null || shootPoint == null || arrowPrefab == null)
        {
            EndAction(false);
            return;
        }

        // Make sure enemy facing target

        RotateTowardsTarget();

        // Checks to make sure cooldown is happening before shooting again

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
        // Signifier speech bubble and getting the direction

        speechBubble.text = "I'm Shooting At You! :)";
        Vector3 direction = (target.value.position - agent.transform.position).normalized;
        direction.y = 0f;

        // Slerp to smoothly rotate towards the player

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private bool IsFacingTarget()
    {
        // Checks using Vector3 dot to make sure that he is currently facing the target

        Vector3 toTarget = (target.value.position - agent.transform.position).normalized;
        toTarget.y = 0f;
        float dot = Vector3.Dot(agent.transform.forward, toTarget);
        return dot >= facingThreshold;
    }

    private void ShootArrow()
    {
        // Instantiates arrow and rigidbody

        GameObject arrow = GameObject.Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Fires the arrow at the player

            Vector3 direction = (target.value.position - shootPoint.position).normalized;
            rb.velocity = direction * 20f;
        }
    }
}
