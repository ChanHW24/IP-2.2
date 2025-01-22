/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 23/01/2025
 * Description:
 * Contains function to communicate with Firebase.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;
using Firebase.Database;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.SocialPlatforms.Impl;
using System.Transactions;

public class FirebaseManager : MonoBehaviour
{
    // Singleton pattern for FirebaseManager instance
    public static FirebaseManager Instance { get; private set; }
    public static DatabaseReference DatabaseReference { get; private set; }

    FirebaseAuth auth;
    DatabaseReference databaseRef;
    public UIManager uIManager;
    public GameManager gameManager;

    // UI input fields for signup/login forms
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField loginEmailField;
    public TMP_InputField loginPasswordField;

    // UI buttons
    public GameObject signUpBtn;
    public GameObject loginBtn;

    // Store user ID and Username for future use
    public static string UserId { get; private set; }
    public static string Username { get; private set; }

    // Singleton setup and Firebase initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        // Initialize Firebase
        InitializeFirebase();
    }

    // Firebase initialization
    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        DatabaseReference = databaseRef;
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        DatabaseReference = databaseRef;

        //GetTop10HighScores(); // Display leaderboard
    }

    private void Update()
    {
        //GetTop10HighScores(); // Display leaderboard
    }

    // Called when SignUp button is clicked
    public void SignUp()
    {
        string username = usernameField.text.Trim();
        string email = emailField.text.Trim();
        string password = passwordField.text.Trim();

        // Create a new user with provided credentials
        SignUpUser(username, email, password);
    }

    // Called when Login button is clicked
    public void Login()
    {
        string email = loginEmailField.text.Trim();
        string password = loginPasswordField.text.Trim();

        // Attempt to log in with provided credentials
        LoginUser(email, password);
    }

    // Handles the signup process and stores username to database
    private void SignUpUser(string username, string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error creating account: " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                Firebase.Auth.AuthResult newPlayer = task.Result;
                UserId = newPlayer.User.UserId; // Store the User ID
                Username = username; // Store the Username

                Debug.LogFormat("Welcome! {0}, your UID is {1}", newPlayer.User.Email, newPlayer.User.UserId);

                // Save the username and initial score to the database
                SaveUsernameAndScoreToDatabase(UserId, username, email, 0);

                // Update UI
                uIManager.usernameText.text = "User: " + username;
                uIManager.backButton.gameObject.SetActive(false);
            }
        });
    }

    // Saves the username and initial score to the Firebase database
    private void SaveUsernameAndScoreToDatabase(string userId, string username, string email, float initialTime)
    {
        // Save the username
        databaseRef.Child("users").Child(userId).Child("username").SetValueAsync(username).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Username saved!");
            }
            else
            {
                Debug.LogError("Failed to save username: " + task.Exception);
            }
        });

        // Save the email
        databaseRef.Child("users").Child(userId).Child("email").SetValueAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Email saved!");
            }
            else
            {
                Debug.LogError("Failed to save email: " + task.Exception);
            }
        });

        // Save the initial score
        databaseRef.Child("users").Child(userId).Child("Timing").SetValueAsync(initialTime).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Initial Timing saved!");
            }
            else
            {
                Debug.LogError("Failed to save score: " + task.Exception);
            }
        });
    }

    // Handles the login process
    private void LoginUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Login Failed: " + task.Exception);
                uIManager.ShowMessage("Incorrect email or password. Please try again.");
                return;
            }
            else if (task.IsCompleted)
            {
                Firebase.Auth.AuthResult loggedInUser = task.Result;
                UserId = loggedInUser.User.UserId; // Store the user ID

                Debug.LogFormat("Welcome back {0}, your UID is {1}", loggedInUser.User.Email, loggedInUser.User.UserId);

                // Load data from the database
                LoadDataFromDatabase(UserId);

                uIManager.backButton.gameObject.SetActive(false);

                uIManager.ShowMessage("Login successful!");
            }
        });
    }

    // Loads user data from the Firebase database (username and high score)
    private void LoadDataFromDatabase(string userId)
    {
        databaseRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DataSnapshot snapshot = task.Result;
                Username = snapshot.Child("username").Value.ToString();
                float highscore = float.Parse(snapshot.Child("points").Value.ToString());

                Debug.Log("Username loaded: " + Username + " High Score: " + highscore);

                // Update UI with username and high score
                uIManager.usernameText.text = "User: " + Username;
                uIManager.hiscoreText.text = "High Score: " + Mathf.FloorToInt(highscore);
            }
            else
            {
                Debug.LogError("Failed to load data.");
            }
        });
    }

    /*
    // Sends a password reset email to the user
    public void SendPasswordResetEmail()
    {
        string email = uIManager.resetEmailField.text.Trim();

        if (!string.IsNullOrEmpty(email))
        {
            auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Password reset email sent!");
                    uIManager.ShowMessage("Password reset email sent! Check your inbox.");
                }
                else
                {
                    Debug.LogError("Failed to send reset email: " + task.Exception);
                    uIManager.ShowMessage("Error sending reset email. Try again.");
                }
            });
        }
        else
        {
            Debug.LogError("Email field is empty.");
            uIManager.ShowMessage("Please enter your email.");
        }
    }

    // Sends high score to Firebase
    public void UpdateHighScoreInFirebase(string userId, System.Action onComplete)
    {
        // Get the current high score from the database
        var userRef = DatabaseReference.Child("users").Child(userId);

        userRef.Child("points").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                float existingHighScore = float.Parse(task.Result.Value.ToString());

                if (gameManager.score > existingHighScore)
                {
                    // Update high score if the current score is higher
                    userRef.Child("points").SetValueAsync(gameManager.score).ContinueWithOnMainThread(updateTask =>
                    {
                        if (updateTask.IsCompleted)
                        {
                            Debug.Log("High score updated successfully!");
                            uIManager.hiscoreText.text = "High Score: " + Mathf.FloorToInt(gameManager.score);
                        }
                        else
                        {
                            Debug.Log("Failed to update high score: " + updateTask.Exception);
                        }
                        onComplete?.Invoke(); // Trigger callback
                    });
                }
                else
                {
                    onComplete?.Invoke(); // Trigger callback if no update needed
                }
            }
            else
            {
                onComplete?.Invoke(); // Trigger callback if task failed or no data exists
            }
        });
    }

    // Retrieves the top 10 high scores from the database
    public void GetTop10HighScores()
    {
        databaseRef.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<PlayerScore> playerScores = new List<PlayerScore>();

                foreach (var userSnapshot in snapshot.Children)
                {
                    string userID = userSnapshot.Key; // Get the userID
                    string username = userSnapshot.Child("username").Value.ToString(); // Get username
                    int score = Mathf.RoundToInt(Convert.ToSingle(userSnapshot.Child("points").Value)); // Get score


                    playerScores.Add(new PlayerScore(username, score));
                }

                // Sort the list in descending order based on high score
                var top10Players = playerScores.OrderByDescending(player => player.highscore).Take(10).ToList();

                // Display the top 10 players
                DisplayTop10Scores(top10Players);
            }
            else
            {
                Debug.LogError("Error retrieving high scores: " + task.Exception);
            }
        });
    }

    // Displays the top 10 high scores in the UI
    public void DisplayTop10Scores(List<PlayerScore> top10Players)
    {
        if (top10Players == null || top10Players.Count == 0)
        {
            uIManager.leaderboardText.text = "No players found.";
            return;
        }

        string displayText = "Top 10 Players:\n";
        for (int i = 0; i < top10Players.Count; i++)
        {
            displayText += (i + 1) + ". " + top10Players[i].username + ": " + top10Players[i].highscore.ToString("F2") + "\n";
        }

        uIManager.leaderboardText.text = displayText;
    }

    // Triggered when the leaderboard button is clicked
    public void LeaderboardButton()
    {
        GetTop10HighScores();
    }
    */
}

