using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public Timer timerScript; // Reference to the Timer script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered ExitTrigger. Stopping timer...");
            timerScript.StopTimer(); // Call the StopTimer() method in the Timer script
        }
    }
}
