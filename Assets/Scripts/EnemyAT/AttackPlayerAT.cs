using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackPlayerAT : ActionTask
{
    // Transform values

    public Transform sword;
    public Transform swordIdlePosition;
    public Transform swordWindUpPosition;
    public Transform swordMidSwingPosition;
    public Transform swordExtendedPosition;
    public BBParameter<Transform> target;
    public BBParameter<EnemyHealth> health;

    // Float values

    public float swordSwingSpeed = 10f;
    public float attackDuration = 0.6f;
    public float facingThreshold = 0.85f;
    public float rotationSpeed = 5f;
    public float distance;
    public float closeShave;

    // Booleans

    public bool isAttacking = false;
    public static bool damageWindow = false;


    protected override void OnExecute()
    {
        // Starts attack if not attacking

        if (!isAttacking)
        {
            distance = Vector3.Distance(agent.transform.position, target.value.position);
            StartCoroutine(RotateAndAttack());
        }
    }

    private IEnumerator RotateAndAttack()
    {
        // When enemy is not facing target, he rotates to face the player

        while (!IsFacingTarget())
        {
            Vector3 directionToTarget = (target.value.position - agent.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(SwingSword());
    }

    private bool IsFacingTarget()
    {
        // Returns based on if facing player or not

        Vector3 toTarget = (target.value.position - agent.transform.position).normalized;
        float dotPoint = Vector3.Dot(agent.transform.forward, toTarget);

        return dotPoint >= facingThreshold;
    }

    private IEnumerator SwingSword()
    {
        // Swinging the sword

        isAttacking = true;

        // Times

        float elapsedTime = 0f;
        float phaseTime = attackDuration / 4;
        float fastPhaseTime = phaseTime / 5;

        // Starting of the sword swing
        while (elapsedTime < phaseTime)
        {
            sword.position = Vector3.Lerp(sword.position, swordWindUpPosition.position, swordSwingSpeed * Time.deltaTime);
            sword.rotation = Quaternion.Lerp(sword.rotation, swordWindUpPosition.rotation, swordSwingSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Mid-point of sword swing
        elapsedTime = 0f;
        damageWindow = true;
        while (elapsedTime < fastPhaseTime)
        {
            sword.position = Vector3.Lerp(sword.position, swordMidSwingPosition.position, swordSwingSpeed * Time.deltaTime);
            sword.rotation = Quaternion.Lerp(sword.rotation, swordMidSwingPosition.rotation, swordSwingSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Super extended point
        elapsedTime = 0f;
        while (elapsedTime < phaseTime)
        {
            sword.position = Vector3.Lerp(sword.position, swordExtendedPosition.position, swordSwingSpeed * Time.deltaTime);
            sword.rotation = Quaternion.Lerp(sword.rotation, swordExtendedPosition.rotation, swordSwingSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Back to the idle pos
        damageWindow = false;
        elapsedTime = 0f;
        while (elapsedTime < phaseTime)
        {
            sword.position = Vector3.Lerp(sword.position, swordIdlePosition.position, swordSwingSpeed * Time.deltaTime);
            sword.rotation = Quaternion.Lerp(sword.rotation, swordIdlePosition.rotation, swordSwingSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (distance < closeShave)
        {
            // If the player is very close when attacked suspicion is lowered

            PlayerController playerController = health.value.player.GetComponent<PlayerController>();
            if(playerController != null)
            {
                if (!playerController.tookDamage)
                {
                    health.value.suspicion -= 2f;
                }
            }
            
        }

        isAttacking = false;
        EndAction(true);
    }
}
