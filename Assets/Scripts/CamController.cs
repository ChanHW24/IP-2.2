using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CamController : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference toggleCameraAction; // Assign in Inspector

    [Header("Camera Settings")]
    public GameObject vrCameraPrefab; // Assign the Camera Prefab from the Project
    public Transform cameraSpawnPoint; // Assign an empty GameObject where the camera should spawn

    private GameObject instantiatedCamera = null;

    private void OnEnable()
    {
        toggleCameraAction.action.performed += ToggleCamera;
    }

    private void OnDisable()
    {
        toggleCameraAction.action.performed -= ToggleCamera;
    }

    void ToggleCamera(InputAction.CallbackContext context)
    {
        if (instantiatedCamera == null)
        {
            // Instantiate the camera at the spawn point
            instantiatedCamera = Instantiate(vrCameraPrefab, cameraSpawnPoint.position, cameraSpawnPoint.rotation);
            instantiatedCamera.transform.SetParent(cameraSpawnPoint); // Parent to spawn point if needed
            Debug.Log("Camera Toggled: ON");
        }
        else
        {
            // Destroy the instantiated camera
            Destroy(instantiatedCamera);
            instantiatedCamera = null;
            Debug.Log("Camera Toggled: OFF");
        }
    }
}