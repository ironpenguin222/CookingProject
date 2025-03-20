using NodeCanvas.Framework;
using UnityEngine;
using ParadoxNotion.Design;
using TMPro;

public class AssignDish : ActionTask
{
    public BBParameter<TableOrders> tableOrders;
    public BBParameter<string> selectedDish;
    public TextMeshProUGUI currentDish;

    protected override void OnExecute()
    {
        if (tableOrders.value.HasOrders())
        {
            selectedDish.value = tableOrders.value.GetNextOrder();
            Debug.Log("Next dish to cook: " + selectedDish.value);
            currentDish.text = selectedDish.value;
            EndAction(true);
        }
        else
        {
            Debug.Log("No pending orders.");
            EndAction(false);
        }
    }
}
