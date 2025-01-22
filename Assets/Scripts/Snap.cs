using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snap : MonoBehaviour
{
    public Transform snapPoint; // Reference to the snap point position
    public string correctTag; // Tag of the correct map piece
    public float snapDistance = 0.2f; // Maximum distance for snapping
    public AudioClip snapSound; // Optional: Sound effect for snapping

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the correct tag
        if (other.CompareTag(correctTag))
        {
            // Calculate the distance between the piece and the snap point
            float distance = Vector3.Distance(other.transform.position, snapPoint.position);

            if (distance <= snapDistance)
            {
                // Snap the piece into place
                other.transform.position = snapPoint.position;
                other.transform.rotation = snapPoint.rotation;

                // Optionally, disable further interaction with the snapped piece
                var rb = other.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                // Play a snap sound if provided
                if (snapSound != null)
                {
                    AudioSource.PlayClipAtPoint(snapSound, snapPoint.position);
                }

                // Provide feedback for successful snapping
                Debug.Log($"{other.name} snapped into place!");
            }
        }
    }
}