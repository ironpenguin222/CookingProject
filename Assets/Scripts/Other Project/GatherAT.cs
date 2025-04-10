using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions {

	public class GatherAT : ActionTask {
        public BBParameter<float> ingredients;

        protected override void OnExecute()
        {
            ingredients.value = 5;
            EndAction(true);
        }
    }
}