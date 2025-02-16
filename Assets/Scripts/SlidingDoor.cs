/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 22/01/2025
 * Description: handles the opening of door 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform doorLeft; 
    public Transform doorRight;
    public Vector3 openOffset; 
    public float openSpeed = 2f;
    
    private Vector3 leftDoorClosedPos; 
    private Vector3 rightDoorClosedPos; 
    private Vector3 leftDoorOpenPos; 
    private Vector3 rightDoorOpenPos;

    private bool isOpening = false;
   
    /// <summary>
    /// Initializes door positions at the start of the scene.
    /// </summary>
    void Start()
    {
        // Store initial positions
        leftDoorClosedPos = doorLeft.position;
        rightDoorClosedPos = doorRight.position;

        // Calculate open positions
        leftDoorOpenPos = leftDoorClosedPos - openOffset; // Slide left door to the left
        rightDoorOpenPos = rightDoorClosedPos + openOffset; // Slide right door to the right
    }

    /// <summary>
    /// Updates door positions smoothly during each frame if the door is opening.
    /// </summary>
    void Update()
    {
        if (isOpening)
        {
           Debug.Log("Door's opening...");
           doorLeft.position = Vector3.MoveTowards(doorLeft.position, leftDoorOpenPos, openSpeed * Time.deltaTime);
           doorRight.position = Vector3.MoveTowards(doorRight.position, rightDoorOpenPos, openSpeed * Time.deltaTime);
        }
    }
    
    public void OpenDoor()
    {
        isOpening = true; 
    } 
}
