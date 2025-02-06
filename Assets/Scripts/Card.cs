using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public SlidingDoor slidingDoor; // Reference to the SlidingDoor script
    
    private void OnTriggerEnter(Collider other)
    {
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
