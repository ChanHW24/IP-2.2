using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class Map : MonoBehaviour
{
    public GameObject[] mapPieces; // Assign the map pieces in the Inspector
    public GameObject newMapPrefab; // Assign the collectible map prefab
    public Transform spawnPoint; // Assign the spawn location for the new map
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] sockets; // Assign the four sockets

    [Header("Input Actions")]
    public InputActionReference collectMapAction; // Assigned in Inspector (Primary Button)
    public InputActionReference displayMapAction; // Assigned in Inspector (Secondary Button)

    [Header("Map Settings")]
    public GameObject collectedMapPrefab; // Assign the map prefab to spawn on the left hand
    public Transform leftControllerAttachPoint; // Attach point on left controller

    [Header("Teleportation")]
    public Transform[] teleportPoints; // Assign 4 teleport points in Inspector
    public GameObject playerRig; // Assign XR Rig or Player GameObject
    
    private bool[] mapPiecesPlaced; // Track whether each map piece is placed
    private bool hasCollectedMap = false;
    private GameObject instantiatedMap = null; // Store the completed map instance
    private GameObject spawnedMap = null; // Store the displayed map instance

    private void OnEnable()
    {
        collectMapAction.action.performed += CollectMap;
        displayMapAction.action.performed += DisplayMap;
    }

    private void OnDisable()
    {
        collectMapAction.action.performed -= CollectMap;
        displayMapAction.action.performed -= DisplayMap;
    }

    void Start()
    {
        mapPiecesPlaced = new bool[sockets.Length];

        for (int i = 0; i < sockets.Length; i++)
        {
            int index = i; // Avoid closure issue
            sockets[i].selectEntered.AddListener((arg) => OnMapPiecePlaced(index, arg.interactableObject));
        }
        
        if (teleportPoints == null || teleportPoints.Length == 0)
        {
            Debug.LogError("Teleport points not assigned in Inspector!");
        }

        if (playerRig == null)
        {
            Debug.LogError("PlayerRig is missing! Assign XR Origin in Inspector.");
        }
        else
        {
            Debug.Log("PlayerRig is assigned.");
        }
    }

    void OnMapPiecePlaced(int index, UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        if (interactable != null && interactable.transform.gameObject == mapPieces[index])
        {
            mapPiecesPlaced[index] = true;
            Debug.Log($"Correct piece placed in socket {index + 1}");
        }
        else
        {
            Debug.Log($"Incorrect piece placed in socket {index + 1}, resetting.");
            sockets[index].interactionManager.SelectExit(sockets[index], interactable); // Force ejection
        }

        if (AllMapPiecesPlaced())
        {
            SpawnNewMap();
            RemoveOldPieces();
        }
    }

    bool AllMapPiecesPlaced()
    {
        foreach (bool placed in mapPiecesPlaced)
        {
            if (!placed) return false;
        }
        return true;
    }

    void SpawnNewMap()
    {
        instantiatedMap = Instantiate(newMapPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
    void CollectMap(InputAction.CallbackContext context)
    {
        if (!hasCollectedMap && instantiatedMap != null)
        {
            hasCollectedMap = true;
            Destroy(instantiatedMap); // Remove the instantiated map from the world
            Debug.Log("Map Collected!");
        }
    }
    
    void DisplayMap(InputAction.CallbackContext context)
    {
        if (hasCollectedMap)
        {
            if (spawnedMap == null)
            {
                spawnedMap = Instantiate(collectedMapPrefab, leftControllerAttachPoint.position, leftControllerAttachPoint.rotation);
                spawnedMap.transform.SetParent(leftControllerAttachPoint);
                Debug.Log("Map displayed on left hand.");
            }
            else
            {
                Destroy(spawnedMap);
                spawnedMap = null;
                Debug.Log("Map removed from left hand.");
            }
        }
        else
        {
            Debug.Log("Player hasn't collected the map yet.");
        }
    }

    void RemoveOldPieces()
    {
        foreach (GameObject piece in mapPieces)
        {
            if (piece != null)
            {
                Destroy(piece);
            }
        }

        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.RemoveAllListeners();
                Destroy(socket.gameObject);
            }
        }

        Debug.Log("Map pieces and sockets have been despawned.");
    }
    
    public void TeleportTo(int index)
    {
        if (index >= 0 && index < teleportPoints.Length && playerRig != null)
        {
            playerRig.transform.position = teleportPoints[index].position;
            playerRig.transform.rotation = teleportPoints[index].rotation;
            Debug.Log($"Teleported to point {index + 1}");
        }
        else
        {
            Debug.LogError("Invalid teleport index or missing playerRig!");
        }
    }
}
