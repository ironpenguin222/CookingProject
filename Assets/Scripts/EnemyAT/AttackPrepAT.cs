using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace NodeCanvas.Tasks.Actions {

	public class AttackPrepAT : ActionTask {

        // Variables

        public BBParameter<EnemyHealth> health;
        public BBParameter<Transform> target;
        public float lastAttackTime = -999f;
        public float timeWindow;
		private float attackCount;

        protected override string OnInit() {
			return null;
		}

		protected override void OnExecute() {
            float distanceToTarget = Vector3.Distance(agent.transform.position, target.value.position);
            PlayerController playerController = health.value.player.GetComponent<PlayerController>();

            // If attacking too fast suspicion rises

            if (Time.time - lastAttackTime < timeWindow && distanceToTarget < 7f)
            {
                health.value.suspicion += 1.3f;
            }

            // If attacks when too far suspicion rises (he looks dumb)

            if (distanceToTarget > 7f)
            {
                health.value.suspicion += 1.3f;
            }

            // Every 3 attacks suspicion rises

            attackCount++;
            if (attackCount % 3 == 0)
            {
               health.value.suspicion += 2f;
            }


            lastAttackTime = Time.time;
            EndAction(true);
		}
	}
}