using System.Collections;
using UnityEngine;

public class GalleryDisplay : MonoBehaviour
{
    public string userId; // User ID from FirebaseManager
    public int imageIndex; // Unique index for each quad (set in the Inspector)

    void Start()
    {
        // Start loading the image for this quad
        StartCoroutine(LoadImage());
    }

    private IEnumerator LoadImage()
    {
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

    private string GetImageUrl(string userId, int imageIndex)
    {
        // Construct the URL for the image based on the user ID and image index
        return $"https://uvklscosbezqzsowuxlb.supabase.co/storage/v1/object/public/images/photos/fake uid/{imageIndex}.jpg";
    }
}