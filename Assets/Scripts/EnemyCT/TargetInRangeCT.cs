using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions {

	public class TargetInRangeCT : ConditionTask {

		// BBParameters

		public BBParameter<Transform> currentTarget;
		public BBParameter<EnemyHealth> manager;
		public float rangeDistance;

		protected override string OnInit(){

			// Gets value from the manager

			rangeDistance = manager.value.detectionRange;
			return null;
		}


		protected override bool OnCheck() {

			// Checks distance from target

			float distanceToTarget = Vector3.Distance(currentTarget.value.position, agent.transform.position);
			return distanceToTarget < rangeDistance;
		}
	}
}