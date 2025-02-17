/*
 * Author: Chan Hong Wei, Tan Tock Beng, Caspar, Ain
 * Date: 03/02/2025
 * Description: manages the socket for the quiz. checks if the player answered the question correctly. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FlowerQuiz : MonoBehaviour
{
    [Header("Socket")]
    public XRSocketInteractor orchidSocket;

    [Header("Correct Flowers (Prefabs)")]
    public GameObject orchidFlower;
    public GameObject roseFlower;
    public GameObject jasmineFlower;

    [Header("UI Elements")]
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI correctText;
    public TextMeshProUGUI incorrectText;

    [Header("National Flower")]
    public GameObject nationalFlowerPrefab;
    public Transform spawnPoint;

    // Variables to store initial positions and rotations of flowers
    private Vector3 orchidStartPos, roseStartPos, jasmineStartPos;
    private Quaternion orchidStartRot, roseStartRot, jasmineStartRot;
    
    public AudioSource correctAudio;
    public AudioSource incorrectAudio;

    /// <summary>
    /// Initializes the game by storing flower positions, setting up UI, 
    /// and subscribing to socket interactions.
    /// </summary>
    private void Start()
    {
        // Save initial positions & rotations
        orchidStartPos = orchidFlower.transform.position;
        orchidStartRot = orchidFlower.transform.rotation;

        roseStartPos = roseFlower.transform.position;
        roseStartRot = roseFlower.transform.rotation;

        jasmineStartPos = jasmineFlower.transform.position;
        jasmineStartRot = jasmineFlower.transform.rotation;

        // Initialize UI
        instructionsText.gameObject.SetActive(true);
        
        // Subscribe to socket interaction events
        orchidSocket.selectEntered.AddListener(CheckFlowers);
    }

    /// <summary>
    /// Checks if the correct flower has been placed in the socket.
    /// </summary>
    /// <param name="args">Event arguments for the selected object.</param>
    private void CheckFlowers(SelectEnterEventArgs args)
    {
        // Get the objects placed in each socket
        GameObject placedOrchid = GetPlacedObject(orchidSocket);


        // Check if all three flowers are placed correctly
        if (placedOrchid == orchidFlower)
        {
            StartCoroutine(HandleSuccess());
        }
        else
        {
            StartCoroutine(HandleFailure());
        }
    }

    /// <summary>
    /// Retrieves the currently placed object in the given socket.
    /// </summary>
    /// <param name="socket">The socket to check.</param>
    /// <returns>The GameObject placed in the socket, or null if empty.</returns>
    private GameObject GetPlacedObject(XRSocketInteractor socket)
    {
        if (socket.interactablesSelected.Count > 0)
        {
            return socket.interactablesSelected[0].transform.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Handles the success scenario when the correct flower is placed.
    /// Displays success message, removes flowers, and spawns the national flower.
    /// </summary>
    private IEnumerator HandleSuccess()
    {
        instructionsText.gameObject.SetActive(false);
        correctText.gameObject.SetActive(true);
        incorrectText.gameObject.SetActive(false);
        correctAudio.Play();
        yield return new WaitForSeconds(2f); // Wait for a moment

        // Remove flowers & sockets
        DestroyFlowersAndSockets();

        // Spawn the national flower
        Instantiate(nationalFlowerPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    /// <summary>
    /// Handles the failure scenario when an incorrect flower is placed.
    /// Displays failure message and resets the flowers.
    /// </summary>
    private IEnumerator HandleFailure()
    {
        instructionsText.gameObject.SetActive(false);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(true);
        incorrectAudio.Play();
        yield return new WaitForSeconds(2f); // Wait for a moment

        // Reset flowers to their original positions
        ResetFlowers();
        instructionsText.gameObject.SetActive(true);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Destroys placed flowers and the socket.
    /// </summary>
    private void DestroyFlowersAndSockets()
    {
        Destroy(GetPlacedObject(orchidSocket));

        Destroy(orchidSocket.gameObject);
        Destroy(roseFlower.gameObject);
        Destroy(jasmineFlower.gameObject);
    }

    /// <summary>
    /// Resets the flowers to their original positions after an incorrect attempt.
    /// </summary>
    private void ResetFlowers()
    {
        ResetFlower(orchidSocket, orchidFlower, orchidStartPos, orchidStartRot);
        ResetFlower(orchidSocket, roseFlower, roseStartPos, roseStartRot);
        ResetFlower(orchidSocket, jasmineFlower, jasmineStartPos, jasmineStartRot);
    }

    // <summary>
    /// Resets a flower's position and removes it from the socket if necessary.
    /// </summary>
    private void ResetFlower(XRSocketInteractor socket, GameObject flower, Vector3 startPos, Quaternion startRot)
    {
        if (socket.interactablesSelected.Count > 0)
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = socket.interactablesSelected[0];
            socket.interactionManager.SelectExit(socket, interactable); // Forcefully remove from socket
        }

        // Move flower back to its original position
        flower.transform.SetPositionAndRotation(startPos, startRot);
    }
}
