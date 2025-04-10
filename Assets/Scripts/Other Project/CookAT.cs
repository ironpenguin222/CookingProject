using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections;

namespace NodeCanvas.Tasks.Actions
{

    public class CookAT : ActionTask
    {
        public BBParameter<string> selectedDish;
        public BBParameter<int> ingredientCount;
        public BBParameter<int> dishes;

        protected override void OnExecute()
        {
            if (ingredientCount.value > 0)
            {
                StartCoroutine(Cook());
            }
            else
            {
                Debug.Log("Not enough ingredients to cook " + selectedDish.value);
                EndAction(false);
            }
        }

        IEnumerator Cook()
        {
            Debug.Log("Cooking " + selectedDish.value + "...");
            yield return new WaitForSeconds(3);
            ingredientCount.value--;
            dishes.value++;

            EndAction(true);
        }
    }
}
