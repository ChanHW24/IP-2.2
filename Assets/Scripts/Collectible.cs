using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Collectible : MonoBehaviour
{
    public string itemName; // Name of the item
    /*private CollectibleManager collectibleManager;
    
    [Header("Input Action")]
    public InputActionReference collectAction; // Assign the "PrimaryButton" action in Inspector

    private void Start()
    {
        collectibleManager = FindObjectOfType<CollectibleManager>(); // Find manager in scene
        
        if (collectAction != null)
        {
            collectAction.action.Enable();
            collectAction.action.performed += CollectItem;
        }
    }

    private void CollectItem(InputAction.CallbackContext context)
    {
        if (collectibleManager != null)
        {
            collectibleManager.AddItem(itemName);
            Debug.Log($"Collected: {itemName}");
            Destroy(gameObject); // Remove object after collection
        }
    }

    private void OnDestroy()
    {
        if (collectAction != null)
        {
            collectAction.action.performed -= CollectItem; // Unsubscribe when destroyed
        }
    }*/
}
