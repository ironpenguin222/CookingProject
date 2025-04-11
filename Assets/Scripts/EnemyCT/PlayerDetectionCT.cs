using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
    public class PlayerDetectionCT : ConditionTask<Transform>
    {
        // Variable values

        public LayerMask playerLayerMask;
        public float detectionRadius = 10f;
        public BBParameter<Transform> detectionTarget;

        protected override bool OnCheck()
        {
            // Sets values to very far first

            Transform bestTarget = null;
            float bestDistance = Mathf.Infinity;

            // Scan for colliders within radius and mask
            Collider[] detectedTargets = Physics.OverlapSphere(agent.transform.position, detectionRadius, playerLayerMask);

            foreach (Collider targetCollider in detectedTargets)
            {
                // Reactive detection by searching all colliders

                float distance = Vector3.Distance(agent.transform.position, targetCollider.transform.position);

                if (distance < bestDistance)
                {
                    // sets best distance

                    bestDistance = distance;
                    bestTarget = targetCollider.transform;
                }
            }

            // sets  target based on this

            detectionTarget.value = bestTarget;

            return bestTarget != null;
        }
    }
}
