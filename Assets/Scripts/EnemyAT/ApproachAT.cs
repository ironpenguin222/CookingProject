using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions
{
    public class ApproachAT : ActionTask
    {
        // Variable values

        public BBParameter<Transform> currentTarget;
        public BBParameter<Transform> detectionTarget;
        public float speed = 3.5f;
        public float closeEnoughThreshold = 2f;

        private NavMeshAgent navAgent;
        private bool isCorrectingCourse = false;
        private float delayBeforeMoving = 0f;
        private bool hasCorrected = false;

        public TextMeshProUGUI speechBubble;

        // Text options for when something happens with enemy
       
        [TextArea] public List<string> noTargetLines = new List<string>();
        [TextArea] public List<string> detourLines = new List<string>();
        [TextArea] public List<string> startMoveLines = new List<string>();
        [TextArea] public List<string> reorientLines = new List<string>();
        [TextArea] public List<string> arrivalLines = new List<string>();

        protected override string OnInit()
        {
            // Sets agent

            navAgent = agent.GetComponent<NavMeshAgent>();
            return null;
        }

        protected override void OnExecute()
        {
            // Ends action if target is null

            currentTarget.value = detectionTarget.value;
            if (currentTarget == null || currentTarget.value == null)
            {
                DisplayLine(noTargetLines);
                EndAction(false);
                return;
            }

            // Sets values for the movement

            navAgent.speed = speed;
            navAgent.stoppingDistance = closeEnoughThreshold;
            navAgent.isStopped = false;

            // Makes the enemy follishly wander around accidentally

            isCorrectingCourse = Random.value < 0.2f;
            delayBeforeMoving = Random.Range(0.4f, 1.4f);
            hasCorrected = !isCorrectingCourse;

            if (isCorrectingCourse)
            {
                // Gives him a fake destination to make him appear foolish

                Vector3 wrongDir = (agent.transform.position - currentTarget.value.position).normalized;
                Vector3 fakeDest = agent.transform.position + wrongDir * 10f;
                navAgent.SetDestination(fakeDest);
                DisplayLine(detourLines);
            }
            else
            {
                // Moves to normal destination

                navAgent.SetDestination(currentTarget.value.position);
                DisplayLine(startMoveLines);
            }
        }

        protected override void OnUpdate()
        {
            if (navAgent == null || currentTarget.value == null)
            {
                DisplayLine(noTargetLines);
                return;
            }

            // Reoriented to the destination towards the player by setting destination to the player

            if (!hasCorrected)
            {
                delayBeforeMoving -= Time.deltaTime;
                if (delayBeforeMoving <= 0f)
                {
                    navAgent.SetDestination(currentTarget.value.position);
                    DisplayLine(reorientLines);
                    hasCorrected = true;
                }
                return;
            }

            // Checks if close enough to player to start attacking the player

            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance && navAgent.velocity.sqrMagnitude < 0.01f)
            {
                
                float distanceToTarget = Vector3.Distance(agent.transform.position, currentTarget.value.position);

                if (distanceToTarget <= closeEnoughThreshold + 0.5f)
                {
                    DisplayLine(arrivalLines);
                }
                navAgent.ResetPath();
                EndAction(true);
            }
        }

        // Resets path when stopped

        protected override void OnStop()
        {
            if (navAgent != null && navAgent.hasPath)
            {
                navAgent.ResetPath();
            }
        }

        // Displays the different lines using the speech bubble above his head which reads out for different situations.

        private void DisplayLine(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
                return;

            string chosenLine = lines[Random.Range(0, lines.Count)];
            Debug.Log(chosenLine);

            if (speechBubble != null)
            {
                speechBubble.text = chosenLine;
            }
        }
    }
}



