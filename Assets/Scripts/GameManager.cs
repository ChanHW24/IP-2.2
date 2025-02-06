using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // A list to store data of collected items
    public List<string> collectedItems = new List<string>();
    
    public void AddItem(string itemName)
    {
        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            Debug.Log($"Item collected: {itemName}");
        }
    }

    public void DisplayCollectedItems()
    {
        Debug.Log("Collected Items:");
        foreach (var item in collectedItems)
        {
            Debug.Log(item);
        }
    }
}
