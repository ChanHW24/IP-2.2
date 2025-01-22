using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Reference to TextMeshProUGUI for the timer
    private float timer = 0f;         // Timer value
    private bool isTiming = false;    // Flag to check if the timer is running

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
}
