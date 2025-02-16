/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 07/02/2025
 * Description: handles the object collection using raycast using the right controller and send the item name into game manager.
 */
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectCollector : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference collectAction; // Assign in Inspector (Primary Button on Right Controller)
    
    [Header("Raycast Settings")]
    public Transform rightController; // Assign the right controller transform
    public float collectRange = 2f; // Maximum collection distance
    public LayerMask collectibleLayer; // Set this to the Collectible layer
    
    private GameManager gameManager;
    
    /// <summary>
    /// Finds the GameManager instance at the start of the game.
    /// </summary>
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// Subscribes the CollectObject method to the input action event when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        collectAction.action.performed += CollectObject;
    }

    /// <summary>
    /// Unsubscribes the CollectObject method from the input action event when the script is disabled.
    /// </summary>
    private void OnDisable()
    {
        collectAction.action.performed -= CollectObject;
    }

    /// <summary>
    /// Attempts to collect an object when the assigned input action is performed.
    /// Uses a raycast from the right controller to detect collectible objects.
    /// </summary>
    /// <param name="context">Input action callback context.</param>
    private void CollectObject(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(rightController.position, rightController.forward, out hit, collectRange, collectibleLayer))
        {
            Collectible collectible = hit.collider.GetComponent<Collectible>();
            if (collectible != null)
            {
                // Add the item to the GameManager inventory
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.AddItem(collectible.itemName);
                }

                // Update Firebase via FirebaseManager
                if (FirebaseManager.Instance != null)
                {
                    FirebaseManager.Instance.UpdateInventory(collectible.itemName);
                }
                else
                {
                    Debug.LogError("FirebaseManager instance is not available.");
                }

                // Destroy the collected object
                Destroy(collectible.gameObject);
                Debug.Log($"Collected: {collectible.itemName}");
            }
        }
    }
}
