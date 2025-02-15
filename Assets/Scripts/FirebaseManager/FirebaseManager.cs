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
using Unity.VisualScripting;
using System.Text;

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
    
    //ticket
    public GameObject ticketPrefab;
    public Transform ticketSpawnPoint;

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

        GetTop10LowestTimings(); // Display leaderboard
    }

    private void Update()
    {
        GetTop10LowestTimings(); // Update leaderboard
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
                SaveToDatabase(UserId, username, email, 0);

                // Update UI
                uIManager.usernameText.text = "User: " + username;
                uIManager.backButton.gameObject.SetActive(false);
            }
        });
    }

    // Saves the username and initial score to the Firebase database
    private void SaveToDatabase(string userId, string username, string email, float initialTime)
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

        // Save the initial timing
        databaseRef.Child("users").Child(userId).Child("timing").SetValueAsync(initialTime).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Initial Timing saved!");
            }
            else
            {
                Debug.LogError("Failed to save timing: " + task.Exception);
            }
        });

        // Create the inventory structure
        Dictionary<string, object> inventory = new Dictionary<string, object>
    {
        { "chineseLantern", new Dictionary<string, object> { { "collected", false } } },
        { "orchidFlower", new Dictionary<string, object> { { "collected", false } } },
        { "singaporeCoin", new Dictionary<string, object> { { "collected", false } } },
        { "singaporePainting", new Dictionary<string, object> { { "collected", false } } }
    };

        // Save the inventory
        databaseRef.Child("users").Child(userId).Child("inventory").SetValueAsync(inventory).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Inventory saved!");
            }
            else
            {
                Debug.LogError("Failed to save inventory: " + task.Exception);
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
                SpawnTicket();
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
                float highscore = float.Parse(snapshot.Child("timing").Value.ToString());

                Debug.Log("Username loaded: " + Username + " High Score: " + highscore);

                // Update UI with username and high score
                uIManager.usernameText.text = "User: " + Username;
                uIManager.hiscoreText.text = "Fastest Time: " + Mathf.FloorToInt(highscore);
            }
            else
            {
                Debug.LogError("Failed to load data.");
            }
        });
    }

    
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

    // Function to update the timing value in Firebase
    public void UpdateTiming(float newTiming)
    {
        if (string.IsNullOrEmpty(UserId))
        {
            Debug.LogError("UserId is null or empty. Please log in before updating timing.");
            return;
        }

        // Format the new timing to 3 decimal places
        float formattedTiming = Mathf.Round(newTiming * 1000) / 1000; // Round to 3 decimal places

        // Retrieve the current timing from Firebase
        databaseRef.Child("users").Child(UserId).Child("timing").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve timing: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                float currentTiming = 0f; // Default value if no timing exists

                if (task.Result.Exists)
                {
                    // Parse the current timing from Firebase
                    currentTiming = float.Parse(task.Result.Value.ToString());
                }

                Debug.Log($"Current timing: {currentTiming:F3}, New timing: {formattedTiming:F3}");

                // Check if the current timing is 0 (no valid timing set) or if the new timing is faster
                if (currentTiming == 0 || formattedTiming < currentTiming)
                {
                    // Update Firebase with the new timing (formatted to 3 decimal places)
                    databaseRef.Child("users").Child(UserId).Child("timing").SetValueAsync(formattedTiming).ContinueWithOnMainThread(updateTask =>
                    {
                        if (updateTask.IsCompleted)
                        {
                            Debug.Log($"Timing updated to {formattedTiming:F3}.");
                        }
                        else
                        {
                            Debug.LogError("Failed to update timing: " + updateTask.Exception);
                        }
                    });
                }
                else
                {
                    Debug.Log("New timing is not faster. No update performed.");
                }
            }
        });
    }

    public void UpdateInventory(string itemName)
    {
        if (databaseRef == null)
        {
            Debug.LogError("Database reference is not initialized.");
            return;
        }

        // Update the specific item in the user's inventory
        databaseRef.Child("users").Child(UserId).Child("inventory").Child(itemName).Child("collected").SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to update Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"Firebase updated: {itemName} collected.");
            }
        });
    }


    // Retrieves the top 10 players with the lowest timings (excluding 0)
    public void GetTop10LowestTimings()
    {
        databaseRef.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving timings: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            List<PlayerTiming> playerTimings = new List<PlayerTiming>();

            foreach (var userSnapshot in snapshot.Children)
            {
                string userID = userSnapshot.Key; // Get the userID
                string username = userSnapshot.Child("username").Value?.ToString(); // Get username
                string timingValue = userSnapshot.Child("timing").Value?.ToString(); // Get timing as string

                if (string.IsNullOrEmpty(username)) continue; // Skip if username is null or empty

                if (float.TryParse(timingValue, out float timing))
                {
                    // Skip if timing is 0
                    if (timing == 0) continue;

                    playerTimings.Add(new PlayerTiming(username, timing));
                }
                else
                {
                    Debug.LogWarning($"Invalid timing for user {username}");
                }
            }

            // Sort the list in ascending order based on timing (lowest timing first)
            var top10Players = playerTimings.OrderBy(player => player.timing).Take(10).ToList();

            // Display the top 10 players with the lowest timings
            DisplayTop10Timings(top10Players);
        });
    }

    // Displays the top 10 players with the lowest timings
    public void DisplayTop10Timings(List<PlayerTiming> top10Players)
    {
        if (top10Players == null || top10Players.Count == 0)
        {
            uIManager.leaderboardText.text = "No players found.";
            return;
        }

        StringBuilder displayText = new StringBuilder("Top 10 Players:\n");
        for (int i = 0; i < top10Players.Count; i++)
        {
            displayText.AppendLine($"{i + 1}. {top10Players[i].username}: {top10Players[i].timing:F2}");
        }

        uIManager.leaderboardText.text = displayText.ToString();
    }

    // Triggered when the leaderboard button is clicked
    public void LeaderboardButton()
    {
        GetTop10LowestTimings();
    }

    public void SpawnTicket()
    {
        ticketPrefab = Instantiate(ticketPrefab, ticketSpawnPoint.position, ticketSpawnPoint.rotation);
    }
}

