using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uIManager;

    public int score = 0; // Player's score


    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points; // Add points to the score
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (uIManager.scoreText != null)
        {
            uIManager.scoreText.text = "Score: " + score;
        }
    }

    // Function to reset the score
    public void ResetScore()
    {
        score = 0; // Reset score to 0
        UpdateScoreUI(); // Update the UI to reflect the reset
    }

}

