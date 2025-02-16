/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 27/01/2025
 * Description: Handles interaction with a ticket to open a sliding door (gantry).
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public SlidingDoor slidingDoor; // Reference to the SlidingDoor script
    
    /// <summary>
    /// Detects when a ticket enters the trigger zone and opens the door.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is tagged as a "Ticket"
        if (other.CompareTag("Ticket"))
        {
            // Destroy the ticket
            Destroy(other.gameObject);

            // Open the gantry doors
            slidingDoor.OpenDoor();

            Debug.Log("Ticket scanned. Gantry opening...");
        }
    }
}
