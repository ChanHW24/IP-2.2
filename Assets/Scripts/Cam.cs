using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Threading.Tasks;


public class Cam : MonoBehaviour
{
    public string supabaseUrl = "https://kmxmtazgljdpwzhbtlfc.supabase.co"; // Your Supabase URL
    public string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; // Your Supabase Anon Key
    public string bucketName = "images"; // Your Supabase storage bucket name

    public Camera vrCamera; // Assign the VR Camera here
    public RawImage screenDisplay; // Assign the Quad’s RawImage component to display camera feed
    public Button captureButton; // UI Button to take a picture

    private RenderTexture renderTexture;
    private const string UploadFolder = "vr_screenshots"; // Folder in Supabase storage

    private void Start()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR Camera is not assigned!");
            return;
        }

        if (screenDisplay != null)
        {
            renderTexture = new RenderTexture(1024, 1024, 24);
            vrCamera.targetTexture = renderTexture;
            screenDisplay.texture = renderTexture;
        }

        captureButton.onClick.AddListener(() => StartCoroutine(CaptureAndUploadScreenshot()));
    }

    private IEnumerator CaptureAndUploadScreenshot()
    {
        yield return new WaitForEndOfFrame();

        // Set active RenderTexture and read pixels
        RenderTexture.active = vrCamera.targetTexture;
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        RenderTexture.active = null;

        // Convert to PNG
        byte[] imageData = screenshot.EncodeToPNG();
        Destroy(screenshot);

        // Save to file (for debugging)
        string filePath = Path.Combine(Application.persistentDataPath, $"VR_Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
        File.WriteAllBytes(filePath, imageData);
        Debug.Log($"Screenshot saved: {filePath}");

        // Upload to Supabase
        StartCoroutine(UploadFileToSupabase(filePath, imageData));
    }

    private IEnumerator UploadFileToSupabase(string filePath, byte[] fileData)
    {
        string fileName = Path.GetFileName(filePath);
        string uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/{UploadFolder}/{fileName}";

        Debug.Log($"Uploading to URL: {uploadUrl}");

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName, "image/png");

        using (UnityWebRequest request = UnityWebRequest.Post(uploadUrl, form))
        {
            request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");

            yield return request.SendWebRequest();

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
