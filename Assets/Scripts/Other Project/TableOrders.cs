using System.Collections.Generic;
using UnityEngine;

public class TableOrders : MonoBehaviour
{
    public List<string> ordersList = new List<string>();

    void Start()
    {
        ordersList.Add("The Box Burger");
        ordersList.Add("Boxed Cheese");
        ordersList.Add("Pasta (Made of Boxes)");
        ordersList.Add("Cardboard");
        ordersList.Add("A Sphere (With Corners)");
        ordersList.Add("Boxed Lunch");
        ordersList.Add("Jack in the Box");
        ordersList.Add("Loaf of Box");
        ordersList.Add("Box Cake");
        ordersList.Add("A Boxer");
        ordersList.Add("Juice Box");
        ordersList.Add("McDonald's Filet O' Fish");
        ordersList.Add("Garbage Cube");
    }

    public string GetNextOrder()
    {
        if (ordersList.Count == 0) return null;

        int randomIndex = Random.Range(0, ordersList.Count);
        string nextOrder = ordersList[randomIndex];
        ordersList.RemoveAt(randomIndex);

        return nextOrder;
    }

    public void AddOrder(string dish)
    {
        ordersList.Add(dish);
    }

    public bool HasOrders()
    {
        return ordersList.Count > 0;
    }
}

