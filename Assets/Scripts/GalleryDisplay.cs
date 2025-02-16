using System.Collections;
using UnityEngine;

public class GalleryDisplay : MonoBehaviour
{
    public int imageIndex; // Unique index for each quad (set in the Inspector)

    private bool hasTriggered = false; // To ensure the coroutine runs only once

    void Start()
    {
        // You can remove the StartCoroutine call from Start if you only want it to run on trigger
        // StartCoroutine(LoadImage());
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player (or any specific object)
        if (other.CompareTag("Player") && !hasTriggered)
        {
            // Start loading the image for this quad
            StartCoroutine(LoadImage());
            hasTriggered = true; // Ensure the coroutine doesn't run again
        }
    }

    public IEnumerator LoadImage()
    {
        string userId = FirebaseManager.UserId; // Get the user ID from firebase.

        // Construct the image URL based on the user ID and image index
        string imageUrl = GetImageUrl(userId, imageIndex);

        using (WWW www = new WWW(imageUrl))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                // Successfully loaded the image
                Texture2D texture = www.texture;
                Renderer renderer = GetComponent<Renderer>();

                // Create a new material using the URP Lit shader and assign the texture to it
                renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                renderer.material.mainTexture = texture;
            }
            else
            {
                // Handle errors (e.g., image not found)
                Debug.LogError("Error loading image: " + www.error);
            }
        }
    }

    public string GetImageUrl(string userId, int imageIndex)
    {
        // Construct the URL for the image based on the user ID and image index
        return $"https://uvklscosbezqzsowuxlb.supabase.co/storage/v1/object/public/images/photos/{userId}/{imageIndex}.jpg";
    }
}