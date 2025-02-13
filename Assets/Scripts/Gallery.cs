/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON; // Install JSON parser from Unity Asset Store if needed

public class Gallery : MonoBehaviour
/*{
    public string supabaseUrl = "https://kmxmtazgljdpwzhbtlfc.supabase.co"; // Your Supabase URL
    public string bucketName = "images"; // Your storage bucket
    public string uploadFolder = "vr_screenshots"; // Folder inside Supabase storage

    public RawImage[] imageSlots; // Assign 4 RawImage UI elements in the Inspector

    private void Start()
    {
        StartCoroutine(LoadImagesFromSupabase());
    }

    private IEnumerator LoadImagesFromSupabase()
    {
        for (int i = 0; i < imageSlots.Length; i++)
        {
            /*string imageUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{uploadFolder}/screenshot_{i}.png";#1#
            string imageUrl = $"https://kmxmtazgljdpwzhbtlfc.supabase.co/storage/v1/object/list/images?prefix=vr_screenshots";
            

            /*string imageUrl = "https://kmxmtazgljdpwzhbtlfc.supabase.co/storage/v1/object/public/images/vr_screenshots/test.png";#1#
            /*string imageUrl = "https://kmxmtazgljdpwzhbtlfc.supabase.co/storage/v1/object/public/images/vr_screenshots/VR_Screenshot_2025-02-12_03-20-36.png";#1#
            Debug.Log($"Fetching: {imageUrl}");

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                imageSlots[i].texture = texture;
                imageSlots[i].gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"Failed to load image {i}: {request.error}");
                imageSlots[i].gameObject.SetActive(false);
            }
        }
    }
}*/

//testing 2
/*{
    public string supabaseUrl = "https://kmxmtazgljdpwzhbtlfc.supabase.co"; // Your Supabase URL
    public string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; // Your Supabase Anon Key
    public string bucketName = "images"; // Your storage bucket
    public string uploadFolder = "vr_screenshots"; // Folder inside Supabase storage

    public RawImage[] imageSlots; // Assign 4 RawImage UI elements in the Inspector

    private void Start()
    {
        StartCoroutine(LoadImagesFromSupabase());
    }

    private IEnumerator LoadImagesFromSupabase()
    {
        // Step 1: Fetch the list of files in the bucket folder
        string listUrl = $"{supabaseUrl}/storage/v1/object/list/{bucketName}?prefix={uploadFolder}/";
        UnityWebRequest listRequest = UnityWebRequest.Get(listUrl);
        listRequest.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");
        listRequest.SetRequestHeader("Content-Type", "application/json");

        yield return listRequest.SendWebRequest();

        if (listRequest.result == UnityWebRequest.Result.Success)
        {
            // Step 2: Parse the JSON response to get the file names
            string jsonResponse = listRequest.downloadHandler.text;
            Debug.Log($"File list response: {jsonResponse}");

            FileList fileList = JsonUtility.FromJson<FileList>("{\"files\":" + jsonResponse + "}");

            // Step 3: Load the images dynamically
            for (int i = 0; i < imageSlots.Length && i < fileList.files.Length; i++)
            {
                string imageUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{uploadFolder}/{fileList.files[i]}";
                Debug.Log($"Fetching: {imageUrl}");

                UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
                yield return imageRequest.SendWebRequest();

                if (imageRequest.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
                    imageSlots[i].texture = texture;
                    imageSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError($"Failed to load image {i}: {imageRequest.error}");
                    imageSlots[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to fetch file list: {listRequest.error}");
        }
    }

    // Helper class to parse the JSON response
    [System.Serializable]
    private class FileList
    {
        public string[] files;
    }
}*/


//testing3
/*{
    public string supabaseUrl = "https://kmxmtazgljdpwzhbtlfc.supabase.co"; // Your Supabase URL
    public string supabaseAnonKey = "your-anon-key"; // Your Supabase Anon Key
    public string bucketName = "images"; // Your storage bucket
    public string uploadFolder = "vr_screenshots"; // Folder inside Supabase storage

    public RawImage[] imageSlots; // Assign 4 RawImage UI elements in the Inspector

    private void Start()
    {
        StartCoroutine(LoadImagesFromSupabase());
    }

    private IEnumerator LoadImagesFromSupabase()
    {
        // Construct URL to list files in your storage folder
        string listFilesUrl = $"{supabaseUrl}/storage/v1/object/list/{bucketName}/{uploadFolder}";

        using (UnityWebRequest listRequest = UnityWebRequest.Get(listFilesUrl))
        {
            // Include the authorization header
            listRequest.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");
            
            // Send request to list files
            yield return listRequest.SendWebRequest();

            if (listRequest.result == UnityWebRequest.Result.Success)
            {
                // Parse the response (JSON) into a list of file names
                var fileList = listRequest.downloadHandler.text;
                Debug.Log("Files listed successfully: " + fileList);

                // Optionally: Parse the file names (you can use a JSON parser to extract the names if needed)
                // For simplicity, let's assume you have a list of file names from the JSON response
                string[] fileNames = fileList.Split(',');

                int imageCount = Mathf.Min(fileNames.Length, imageSlots.Length); // Limit to the number of available slots
                for (int i = 0; i < imageCount; i++)
                {
                    // Construct the image URL based on the file name
                    string imageUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{uploadFolder}/{fileNames[i]}";
                    Debug.Log($"Fetching: {imageUrl}");  // Added Debug.Log for image URL

                    // Download image
                    UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
                    yield return imageRequest.SendWebRequest();

                    if (imageRequest.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
                        imageSlots[i].texture = texture;
                        imageSlots[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError($"Failed to load image {i}: {imageRequest.error}");
                        imageSlots[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to list files: " + listRequest.error);
            }
        }
    }
}*/

{
    public string supabaseUrl = "https://kmxmtazgljdpwzhbtlfc.supabase.co";
    public string bucketName = "images";
    public string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
    public string uploadFolder = "vr_screenshots"; // Folder in Supabase storage
    public RawImage[] imageSlots; // Assign 4 RawImage UI elements in the Inspector

    private void Start()
    {
        StartCoroutine(FetchImageListFromSupabase());
    }

    private IEnumerator FetchImageListFromSupabase()
    {
        string listUrl = $"{supabaseUrl}/storage/v1/object/list/{bucketName}?prefix={uploadFolder}";

        UnityWebRequest request = UnityWebRequest.Get(listUrl);
        request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse JSON response
            JSONNode jsonResponse = JSON.Parse(request.downloadHandler.text);

            List<string> imageUrls = new List<string>();

            foreach (JSONNode file in jsonResponse.Children) // Corrected iteration
            {
                string fileName = file["name"];
                if (fileName.EndsWith(".png")) // Filter for images
                {
                    string imageUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{fileName}";
                    imageUrls.Add(imageUrl);
                }
            }

            StartCoroutine(LoadImages(imageUrls));
        }
        else
        {
            Debug.LogError($"Failed to fetch image list: {request.error}");
        }
    }

    private IEnumerator LoadImages(List<string> imageUrls)
    {
        for (int i = 0; i < Mathf.Min(imageUrls.Count, imageSlots.Length); i++)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrls[i]);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                imageSlots[i].texture = texture;
                imageSlots[i].gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"Failed to load image {i}: {request.error}");
                imageSlots[i].gameObject.SetActive(false);
            }
        }
    }
}