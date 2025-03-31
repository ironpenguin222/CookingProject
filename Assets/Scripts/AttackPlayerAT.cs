using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackPlayerAT : ActionTask
{
    public Transform sword;
    public Transform swordIdlePosition;
    public Transform swordWindUpPosition;
    public Transform swordMidSwingPosition;
    public Transform swordExtendedPosition;
    public Transform target;

    public float swordSwingSpeed = 10f;
    public float attackDuration = 0.6f;
    public float facingThreshold = 0.85f;
    public float rotationSpeed = 5f;

    private bool isAttacking = false;


    protected override void OnExecute()
    {
        if (!isAttacking)
        {
            StartCoroutine(RotateAndAttack());
        }
    }

    private IEnumerator RotateAndAttack()
    {
        while (!IsFacingTarget())
        {
            Vector3 directionToTarget = (target.position - agent.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(SwingSword());
    }

    private bool IsFacingTarget()
    {

        Vector3 toTarget = (target.position - agent.transform.position).normalized;
        float dotPoint = Vector3.Dot(agent.transform.forward, toTarget);

        return dotPoint >= facingThreshold;
    }

    private IEnumerator SwingSword()
    {
        isAttacking = true;
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
        elapsedTime = 0f;
        while (elapsedTime < phaseTime)
        {
            sword.position = Vector3.Lerp(sword.position, swordIdlePosition.position, swordSwingSpeed * Time.deltaTime);
            sword.rotation = Quaternion.Lerp(sword.rotation, swordIdlePosition.rotation, swordSwingSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        EndAction(true);
    }
}
