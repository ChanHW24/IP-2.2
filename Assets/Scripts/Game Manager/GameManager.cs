/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 17/02/2025
 * Description: Handles the script for the game.
 */
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uIManager;
    public List<string> collectedItems = new List<string>();
    
    public SlidingDoor exitGantryDoor; // Reference to the exit gantry door script
    private List<string> requiredItems = new List<string> { "singaporeCoin", "chineseLantern", "orchidFlower", "singaporePainting" };

    public TextMeshProUGUI timerText; // Reference to TextMeshProUGUI for the timer
    public float timer = 0f;         // Timer value
    private bool isTiming = false;    // Flag to check if the timer is running

    void Start()
    {
        // Initialize the timer text to show "Time: --" or an empty string
        timerText.text = "Time: --";
        
        if (exitGantryDoor == null)
        {
            Debug.LogError("Exit gantry door reference is missing!");
        }
    }

    private void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void StartTimer()
    {
        if (!isTiming)
        {
            isTiming = true;
            timer = 0f; // Reset timer when starting
            Debug.Log("Timer started!");
        }
        else
        {
            Debug.LogWarning("Timer is already running.");
        }
    }

    public void StopTimer()
    {
        if (isTiming)
        {
            isTiming = false;
            Debug.Log($"Timer stopped! Final time: {timer:0.00} s");
            DisplayFinalTime();
            CheckAndUpdateTiming(timer); // Send the final time to Firebase
        }
        else
        {
            Debug.LogWarning("Timer is not running!");
        }
    }

    private void UpdateTimerUI()
    {
        // Update the timer text with the current time
        timerText.text = $"Time: {timer:0.00} s";
    }

    private void DisplayFinalTime()
    {
        // Display the final time in the UI
        timerText.text = $"Final Time: {timer:0.00} s";
    }

    // Send Timer value to Firebase to check if highscore needs to be updated.
    public void CheckAndUpdateTiming(float newTiming)
    {
        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.UpdateTiming(newTiming);
        }
        else
        {
            Debug.LogError("FirebaseManager instance is null. Ensure it is initialized.");
        }
    }

    public void AddItem(string itemName)
    {
        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            Debug.Log($"Item collected: {itemName}");
            
            CheckAllItemsCollected();
        }
    }


    
    private void CheckAllItemsCollected()
    {
        if (requiredItems.TrueForAll(item => collectedItems.Contains(item)))
        {
            Debug.Log("All required items collected! Opening exit gantry...");
            exitGantryDoor.OpenDoor(); // Open the exit gantry doors
        }
    }
    
}