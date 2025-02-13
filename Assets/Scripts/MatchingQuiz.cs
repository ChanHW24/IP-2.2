using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MatchingQuiz : MonoBehaviour
{
    [Header("Sockets")]
    public XRSocketInteractor orchidSocket;
    public XRSocketInteractor roseSocket;
    public XRSocketInteractor jasmineSocket;

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

    [Header("Panels")]
    public  GameObject panel;
    public  GameObject panel1;
    public  GameObject panel2;
    
    private Vector3 orchidStartPos, roseStartPos, jasmineStartPos;
    private Quaternion orchidStartRot, roseStartRot, jasmineStartRot;

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
        /*instructionsText.text = "Place the correct flower onto their respective names.";*/
        instructionsText.gameObject.SetActive(true);
        
        panel.SetActive(true);
        panel1.SetActive(true);
        panel2.SetActive(true);
        
        // Subscribe to socket interaction events
        orchidSocket.selectEntered.AddListener(CheckFlowers);
        roseSocket.selectEntered.AddListener(CheckFlowers);
        jasmineSocket.selectEntered.AddListener(CheckFlowers);
    }

    private void CheckFlowers(SelectEnterEventArgs args)
    {
        // Get the objects placed in each socket
        GameObject placedOrchid = GetPlacedObject(orchidSocket);
        GameObject placedRose = GetPlacedObject(roseSocket);
        GameObject placedJasmine = GetPlacedObject(jasmineSocket);

        // Check if all three flowers are placed correctly
        if (placedOrchid == orchidFlower && placedRose == roseFlower && placedJasmine == jasmineFlower)
        {
            StartCoroutine(HandleSuccess());
        }
        else if (placedOrchid != null && placedRose != null && placedJasmine != null) // If all sockets are occupied but incorrect
        {
            StartCoroutine(HandleFailure());
        }
    }

    private GameObject GetPlacedObject(XRSocketInteractor socket)
    {
        if (socket.interactablesSelected.Count > 0)
        {
            return socket.interactablesSelected[0].transform.gameObject;
        }
        return null;
    }

    private IEnumerator HandleSuccess()
    {
        instructionsText.gameObject.SetActive(false);
        correctText.gameObject.SetActive(true);
        incorrectText.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f); // Wait for a moment

        // Remove flowers & sockets
        DestroyFlowersAndSockets();

        panel.SetActive(false);
        panel1.SetActive(false);
        panel2.SetActive(false);
        
        // Spawn the national flower
        Instantiate(nationalFlowerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
    private IEnumerator HandleFailure()
    {
        instructionsText.gameObject.SetActive(false);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f); // Wait for a moment

        // Reset flowers to their original positions
        ResetFlowers();
        instructionsText.gameObject.SetActive(true);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);
    }

    private void DestroyFlowersAndSockets()
    {
        Destroy(GetPlacedObject(orchidSocket));
        Destroy(GetPlacedObject(roseSocket));
        Destroy(GetPlacedObject(jasmineSocket));

        Destroy(orchidSocket.gameObject);
        Destroy(roseSocket.gameObject);
        Destroy(jasmineSocket.gameObject);
    }

    private void ResetFlowers()
    {
        ResetFlower(orchidSocket, orchidFlower, orchidStartPos, orchidStartRot);
        ResetFlower(roseSocket, roseFlower, roseStartPos, roseStartRot);
        ResetFlower(jasmineSocket, jasmineFlower, jasmineStartPos, jasmineStartRot);
    }

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
