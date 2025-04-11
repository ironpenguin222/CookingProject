using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    public class EnemyClosebyCT : ConditionTask
    {
        // Variable values

        public BBParameter<Transform> currentTarget;
        public float rangeDistance;

        protected override string OnInit()
        {
            return null;
        }

        protected override bool OnCheck()
        {
            // Checks the distance from the target

            float distanceToTarget = Vector3.Distance(currentTarget.value.position, agent.transform.position);
            return distanceToTarget < rangeDistance;
        }
    }
}