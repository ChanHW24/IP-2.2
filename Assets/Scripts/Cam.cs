/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 23/01/2025
 * Description: This script captures a screenshot from a VR camera in Unity, displays it on a RawImage UI component,
 * saves it locally, and uploads it to Supabase storage.
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;

public class Cam : MonoBehaviour
{
    // Supabase configuration
    public string supabaseUrl = "https://uvklscosbezqzsowuxlb.supabase.co"; // Your Supabase URL
    public string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InV2a2xzY29zYmV6cXpzb3d1eGxiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzYyMTMwMjIsImV4cCI6MjA1MTc4OTAyMn0.MtRESkZnIRtfJNGrVAuL3_jwOC_Fb4z3lLXznJFm5RE"; // Your Supabase Anon Key
    public string bucketName = "images"; // Your Supabase storage bucket name

    // UI and camera references
    public Camera vrCamera; // VR camera used for capturing images
    public RawImage screenDisplay; // Assign the Quad’s RawImage component to display camera feed
    public Button captureButton; // UI Button to take a picture

    private RenderTexture renderTexture; // RenderTexture for capturing the camera feed
    private const string UploadFolder = "vr_screenshots"; // Folder name in Supabase storage
    private int photoCounter = 1; // Counter to track the number of photos taken

    private void Start()
    {
        // Ensure VR camera is assigned
        if (vrCamera == null)
        {
            Debug.LogError("VR Camera is not assigned!");
            return;
        }
        
        // Set up the display for the camera feed
        if (screenDisplay != null)
        {
            renderTexture = new RenderTexture(1024, 1024, 24);
            vrCamera.targetTexture = renderTexture;
            screenDisplay.texture = renderTexture;
        }
        // Add click event listener to capture button
        captureButton.onClick.AddListener(() => StartCoroutine(CaptureAndUploadScreenshot()));
    }

    // Coroutine to capture a screenshot and upload it
    private IEnumerator CaptureAndUploadScreenshot()
    {
        yield return new WaitForEndOfFrame();

        // Capture the VR camera’s view
        RenderTexture.active = vrCamera.targetTexture;
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        RenderTexture.active = null;

        // Convert the captured image to a JPG byte array
        byte[] imageData = screenshot.EncodeToJPG();
        Destroy(screenshot);

        // Save the screenshot locally
        string fileName = $"{photoCounter}.jpg"; // Name the file using the counter
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(filePath, imageData);
        Debug.Log($"Screenshot saved: {filePath}");

        // Increment the counter for the next photo
        photoCounter++;

        // Upload to Supabase
        StartCoroutine(UploadFileToSupabase(filePath, imageData));
    }

    // Coroutine to upload the image file to Supabase storage
    private IEnumerator UploadFileToSupabase(string filePath, byte[] fileData)
    {
        string fileName = Path.GetFileName(filePath);
        string uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/photos/{FirebaseManager.UserId}/{fileName}";

        Debug.Log($"Uploading to URL: {uploadUrl}");

        // Prepare the form with the image data
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName, "image/jpeg");

        // Send the POST request
        using (UnityWebRequest request = UnityWebRequest.Post(uploadUrl, form))
        {
            request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}"); // Wait for upload to complete

            yield return request.SendWebRequest();

            // Check for success or failure
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"File uploaded successfully: {fileName}");
            }
            else
            {
                Debug.LogError($"Upload failed: {request.error}");
            }
        }
    }
}