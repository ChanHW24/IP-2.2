using System.IO;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int imageWidth = 1024;
    public bool saveAsJPEG = true;

    void Start()
    {
        byte[] bytes = I360Render.Capture(imageWidth, saveAsJPEG);
        if (bytes != null)
        {
            string path = Path.Combine(Application.persistentDataPath, "360render" + (saveAsJPEG ? ".jpeg" : ".png"));
            File.WriteAllBytes(path, bytes);
            Debug.Log("360 render saved to " + path);
        }
    }
}