using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace NodeCanvas.Tasks.Actions
{
    public class ApproachAT : ActionTask
    {
        public BBParameter<Transform> currentTarget;
        public float speed = 3.5f;
        public float closeEnoughThreshold = 2f;
        private NavMeshAgent navAgent;

        protected override string OnInit()
        {
            navAgent = agent.GetComponent<NavMeshAgent>();
            return null;
        }

        protected override void OnExecute()
        {

            navAgent.speed = speed;
            navAgent.SetDestination(currentTarget.value.position);
            EndAction(true);
        }

        protected override void OnUpdate()
        {
            if (navAgent.pathPending) return;

            if (Vector3.Distance(navAgent.transform.position, currentTarget.value.position) <= closeEnoughThreshold)
            {
                navAgent.ResetPath();
                EndAction(true);
            }
        }
    }
}
