using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public GameManager gameManager; // Reference to the Game manager script.
    public FirebaseManager firebaseManager; // Reference to the Firebase manager script.

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered ExitTrigger. Stopping timer...");
            gameManager.StopTimer(); // Call the StopTimer() method in the Timer script
            gameManager.CheckAndUpdateTiming(gameManager.timer); // Send to Firebase to compare if this new timing beat the old timing.

        }
    }
}
