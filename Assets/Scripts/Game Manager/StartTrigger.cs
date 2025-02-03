using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    public GameManager gameManager; // Reference to the Timer script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered StartTrigger. Starting timer...");
            gameManager.StartTimer(); // Call the StartTimer() method in the Timer script
        }
    }
}
