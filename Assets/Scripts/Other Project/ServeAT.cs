using NodeCanvas.Framework;
using ParadoxNotion.Design;
using TMPro;


namespace NodeCanvas.Tasks.Actions
{

    public class ServeAT : ActionTask
    {
        public BBParameter<int> dishes;
        public BBParameter<TableOrders> tableOrders;
        public BBParameter<string> selectedDish;
        public TextMeshProUGUI currentDish;
        protected override void OnExecute()
        {
            if (dishes.value > 0)
            {
                dishes.value--;

                if (tableOrders.value.HasOrders())
                {
                    selectedDish.value = tableOrders.value.GetNextOrder();
                }
                else
                {
                    selectedDish.value = "";
                }
            }
            currentDish.text = selectedDish.value;
            EndAction(true);
        }
    }
}