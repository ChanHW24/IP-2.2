/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 30/01/2025
 * Description: Controls the instantiation and destruction of a VR camera in response to an input action.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CamController : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference toggleCameraAction; 

    [Header("Camera Settings")]
    public GameObject vrCameraPrefab; 
    public Transform cameraSpawnPoint;

    private GameObject instantiatedCamera = null;

    /// <summary>
    /// Subscribes to the toggle camera action when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        toggleCameraAction.action.performed += ToggleCamera;
    }
    
    /// <summary>
    /// Subscribes to the toggle camera action when the script is enabled.
    /// </summary>
    private void OnDisable()
    {
        toggleCameraAction.action.performed -= ToggleCamera;
    }

    /// <summary>
    /// Toggles the VR camera on and off when the assigned input action is triggered.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    void ToggleCamera(InputAction.CallbackContext context)
    {
        if (instantiatedCamera == null)
        {
            // Instantiate the camera at the spawn point
            instantiatedCamera = Instantiate(vrCameraPrefab, cameraSpawnPoint.position, cameraSpawnPoint.rotation);
            instantiatedCamera.transform.SetParent(cameraSpawnPoint);
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