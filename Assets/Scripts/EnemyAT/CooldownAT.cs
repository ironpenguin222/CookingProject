using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

public class CooldownAT : ActionTask
{
    public float cooldownDuration = 2f;

    protected override void OnExecute()
    {
        // Starts waiting coroutine

        StartCoroutine(WaitAndEnd());
    }

    private IEnumerator WaitAndEnd()
    {
        // Waits for amount of time

        yield return new WaitForSeconds(cooldownDuration);
        EndAction(true);
    }
}