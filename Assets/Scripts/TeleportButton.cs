/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 10/02/2025
 * Description: handles the map teleport feature 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportButton : MonoBehaviour
{
    /// <summary>
    /// list of teleport points in the map
    /// </summary>
    public int teleportIndex; 
    private Map mapScript; 

    /// <summary>
    /// Initializes the teleport button by setting up the button's listener and finding the Map script.
    /// </summary>
    private void Start()
    {
        // Find the Map script in the scene (MapManager object)
        mapScript = FindObjectOfType<Map>();
        if (mapScript == null)
        {
            Debug.LogError("Map script not found in the scene!");
        }
        
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);
    }

    /// <summary>
    /// Called when the teleport button is pressed.
    /// Triggers the teleportation to the designated teleport point if the Map script is found.
    /// </summary>
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
}
