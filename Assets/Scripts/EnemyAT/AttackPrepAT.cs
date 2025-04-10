using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace NodeCanvas.Tasks.Actions {

	public class AttackPrepAT : ActionTask {
        public BBParameter<EnemyHealth> health;
        public BBParameter<Transform> target;
        public float lastAttackTime = -999f;
        public float timeWindow;
		private float attackCount;
        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit() {
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute() {
            float distanceToTarget = Vector3.Distance(agent.transform.position, target.value.position);
            PlayerController playerController = health.value.player.GetComponent<PlayerController>();
            if (Time.time - lastAttackTime < timeWindow && distanceToTarget < 7f)
            {
                health.value.suspicion += 1.3f;
            }

            if (distanceToTarget > 7f)
            {
                health.value.suspicion += 1.3f;
            }

            

            attackCount++;
            if (attackCount % 3 == 0)
            {
               health.value.suspicion += 2f;
            }


            lastAttackTime = Time.time;
            EndAction(true);
		}

		//Called once per frame while the action is active.
		protected override void OnUpdate() {
			
		}

		//Called when the task is disabled.
		protected override void OnStop() {
			
		}

		//Called when the task is paused.
		protected override void OnPause() {
			
		}
	}
}