using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MapController : MonoBehaviour
{
    public InputActionProperty collectMapAction;  // Collect map action (Primary Button)
    public InputActionProperty displayMapAction;  // Display map action (Secondary Button)

    public GameObject collectedMapPrefab;  // Assign the map prefab in Inspector
    public Transform leftHandTransform;  // Assign the left-hand XR controller transform

    private GameObject collectedMapInstance;
    private bool hasCollectedMap = false;

    void Start()
    {
        // Subscribe to button press events
        collectMapAction.action.performed += CollectMap;
        displayMapAction.action.performed += DisplayMap;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        collectMapAction.action.performed -= CollectMap;
        displayMapAction.action.performed -= DisplayMap;
    }

    private void CollectMap(InputAction.CallbackContext context)
    {
        if (!hasCollectedMap)
        {
            Debug.Log("Map collected!");
            hasCollectedMap = true;
        }
    }

    private void DisplayMap(InputAction.CallbackContext context)
    {
        if (hasCollectedMap)
        {
            if (collectedMapInstance == null)
            {
                collectedMapInstance = Instantiate(collectedMapPrefab, leftHandTransform);
                collectedMapInstance.transform.localPosition = Vector3.zero;  // Adjust position relative to hand
                collectedMapInstance.transform.localRotation = Quaternion.identity;
                Debug.Log("Map displayed on left hand.");
            }
            else
            {
                Destroy(collectedMapInstance);
                collectedMapInstance = null;
                Debug.Log("Map removed from left hand.");
            }
        }
    }
}