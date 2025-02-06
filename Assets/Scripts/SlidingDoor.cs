using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform doorLeft; // Assign the left door
       public Transform doorRight; // Assign the right door
       public Vector3 openOffset; // Offset to determine how far doors should slide open
       public float openSpeed = 2f; // Speed of door opening
   
       private Vector3 leftDoorClosedPos;
       private Vector3 rightDoorClosedPos;
       private Vector3 leftDoorOpenPos;
       private Vector3 rightDoorOpenPos;
   
       private bool isOpening = false;
   
       void Start()
       {
           // Store initial positions
           leftDoorClosedPos = doorLeft.position;
           rightDoorClosedPos = doorRight.position;
   
           // Calculate open positions
           leftDoorOpenPos = leftDoorClosedPos - openOffset; // Slide left door to the left
           rightDoorOpenPos = rightDoorClosedPos + openOffset; // Slide right door to the right
       }
   
       void Update()
       {
           if (isOpening)
           {
               // Smoothly move doors to open positions
               doorLeft.position = Vector3.MoveTowards(doorLeft.position, leftDoorOpenPos, openSpeed * Time.deltaTime);
               doorRight.position = Vector3.MoveTowards(doorRight.position, rightDoorOpenPos, openSpeed * Time.deltaTime);
           }
       }
   
       public void OpenDoor()
       {
           isOpening = true; // Start the door opening process
       }
}
