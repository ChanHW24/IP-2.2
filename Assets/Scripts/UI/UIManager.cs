/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 23/01/2025
 * Description:
 * Contains Game Object inputs for UI.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    public TextMeshProUGUI gameOverText;
    public GameObject retryButton;
    public GameObject gameMenuUI;
    public GameObject loginMenuUI;
    public GameObject signUpMenuUI;
    public TextMeshProUGUI usernameText;
    public GameObject forgotPasswordButton;
    public TMP_InputField resetEmailField;
    public GameObject resetPasswordBtn;
    public TextMeshProUGUI messageText; // Text element to display messages
    public GameObject backButton;
    public Button menuButton;
    public TextMeshProUGUI leaderboardText;

    private void Start()
    {

    }

    public void SwitchUser()
    {
        loginMenuUI.SetActive(true);
        backButton.SetActive(true);
        gameMenuUI.SetActive(false);
    }

    // To display any messages for the player/user
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        // Start the shake effect when message is shown
        StartCoroutine(ShakeMessage());

        Invoke("HideMessage", 5.0f); // Hide message after 5 seconds
    }

    private void HideMessage()
    {
        messageText.gameObject.SetActive(false);
    }

    // Animation when text appear in the text box
    private IEnumerator ShakeMessage()
    {
        Vector3 originalPosition = messageText.transform.localPosition;
        float duration = 0.5f; // Total duration of shake
        float magnitude = 5f;  // Shake intensity

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            messageText.transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // Reset the position after shaking
        messageText.transform.localPosition = originalPosition;
    }

    public void DisplayHighScore(int score)
    {
        leaderboardText.text = "High Score: " + score;
    }
}
