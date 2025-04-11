using UnityEngine;
using NodeCanvas.Framework;

public class DodgeCT : ConditionTask
{
    // More variable values

    public BBParameter<GameObject> player;
    public BBParameter<EnemyHealth> manager;
    public float detectionRange;


    protected override string OnInit()
    {
        // Setup the detection range

        detectionRange = manager.value.detectionRange;
        return null;
    }

    protected override bool OnCheck()
    {
        if (player.isNull) return false;

        // Checks if the player is close and attacking to know when to dodge

        float distance = Vector3.Distance(agent.transform.position, player.value.transform.position);
        if (distance > detectionRange) return false;

        PlayerController playerController = player.value.GetComponent<PlayerController>();
        if (playerController == null) return false;

        return playerController.isAttacking;
    }
}
