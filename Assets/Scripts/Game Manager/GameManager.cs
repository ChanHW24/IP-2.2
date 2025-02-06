using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uIManager;

    public TextMeshProUGUI timerText; // Reference to TextMeshProUGUI for the timer
    public float timer = 0f;         // Timer value
    private bool isTiming = false;    // Flag to check if the timer is running


    void Start()
    {
        
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
        }
        else
        {
            Debug.LogWarning("Timer is not running!");
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"Time: {timer:0.00} s";
    }

    private void DisplayFinalTime()
    {
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

}

