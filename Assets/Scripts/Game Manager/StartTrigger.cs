using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    public Timer timerScript; // Reference to the Timer script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered StartTrigger. Starting timer...");
            timerScript.StartTimer(); // Call the StartTimer() method in the Timer script
        }
    }
}
