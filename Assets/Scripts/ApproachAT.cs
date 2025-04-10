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
        public BBParameter<Transform> currentTarget;
        public BBParameter<Transform> detectionTarget;
        public float speed = 3.5f;
        public float closeEnoughThreshold = 2f;

        private NavMeshAgent navAgent;
        private bool isCorrectingCourse = false;
        private float delayBeforeMoving = 0f;
        private bool hasCorrected = false;

        public TextMeshProUGUI speechBubble;
       
        [TextArea] public List<string> noTargetLines = new List<string>();
        [TextArea] public List<string> detourLines = new List<string>();
        [TextArea] public List<string> startMoveLines = new List<string>();
        [TextArea] public List<string> reorientLines = new List<string>();
        [TextArea] public List<string> arrivalLines = new List<string>();

        protected override string OnInit()
        {
            navAgent = agent.GetComponent<NavMeshAgent>();
            return null;
        }

        protected override void OnExecute()
        {
            currentTarget.value = detectionTarget.value;
            if (currentTarget == null || currentTarget.value == null)
            {
                DisplayLine(noTargetLines);
                EndAction(false);
                return;
            }

            navAgent.speed = speed;
            navAgent.stoppingDistance = closeEnoughThreshold;
            navAgent.isStopped = false;

            isCorrectingCourse = Random.value < 0.2f;
            delayBeforeMoving = Random.Range(0.2f, 1.0f);
            hasCorrected = !isCorrectingCourse;

            if (isCorrectingCourse)
            {
                Vector3 wrongDir = (agent.transform.position - currentTarget.value.position).normalized;
                Vector3 fakeDest = agent.transform.position + wrongDir * 5f;
                navAgent.SetDestination(fakeDest);
                DisplayLine(detourLines);
            }
            else
            {
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

            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance && navAgent.velocity.sqrMagnitude < 0.01f)
            {
                navAgent.ResetPath();
                DisplayLine(arrivalLines);
                EndAction(true);
            }
        }

        protected override void OnStop()
        {
            if (navAgent != null && navAgent.hasPath)
            {
                navAgent.ResetPath();
            }
        }

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



