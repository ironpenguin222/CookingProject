using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions {

	public class checkOrdersCT : ConditionTask {
        public BBParameter<TableOrders> tableOrders;
        public BBParameter<string> selectedDish;
        protected override bool OnCheck() {
            return string.IsNullOrEmpty(selectedDish.value) && tableOrders.value.HasOrders();
        }
	}
}