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

    private void OnEnable()
    {
        collectAction.action.performed += CollectObject;
    }

    private void OnDisable()
    {
        collectAction.action.performed -= CollectObject;
    }

    private void CollectObject(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(rightController.position, rightController.forward, out hit, collectRange, collectibleLayer))
        {
            Collectible collectible = hit.collider.GetComponent<Collectible>();
            if (collectible != null)
            {
                // Find the GameManager and add the item name
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
