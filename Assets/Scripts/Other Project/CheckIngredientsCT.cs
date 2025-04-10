using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions {

	public class CheckIngredientsCT : ConditionTask {
		public BBParameter<float> ingredients;
        public BBParameter<int> dishes;
        protected override bool OnCheck() {
			return ingredients.value <= 0 || dishes.value != 0;
		}
	}
}