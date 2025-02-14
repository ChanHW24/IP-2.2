using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportButton : MonoBehaviour
{
    public int teleportIndex; // Index of the teleport point
    private Map mapScript; // Reference to the Map script

    private void Start()
    {
        // Find the Map script in the scene (MapManager object)
        mapScript = FindObjectOfType<Map>();
        if (mapScript == null)
        {
            Debug.LogError("Map script not found in the scene!");
        }

        // Ensure the button is set up to call the teleport method
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);
    }

    void OnButtonPressed()
    {
        if (mapScript != null)
        {
            mapScript.TeleportTo(teleportIndex); // Trigger teleportation
        }
        else
        {
            Debug.LogError("Map script not assigned!");
        }
    }
    
    /*void OnButtonPressed()
    {
        Debug.Log($"Teleport button pressed with index: {teleportIndex}");
        if (mapScript != null)
        {
            mapScript.TeleportTo(teleportIndex); // Trigger teleportation
        }
        else
        {
            Debug.LogError("Map script not assigned!");
        }
    }*/
}
