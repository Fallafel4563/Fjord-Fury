using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private GameObject allPlayersReadyBanner;
    [SerializeField] private List<GameObject> characterSelectPositions = new();

    private bool allPlayersReady = false;
    private int playerCount = 0;
    private int readyPlayerCount = 0;
    private Dictionary<int, int> playerChoiceDict = new();


    private void Start()
    {
        allPlayersReadyBanner.SetActive(false);
    }


    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} joined", playerInput.playerIndex);

        playerCount++;

        // Set the position of the menu thingy
        playerInput.transform.SetParent(characterSelectPositions[playerInput.playerIndex].transform, false);
        playerInput.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        // Hide banner when a new player joins
        if (playerCount > readyPlayerCount && allPlayersReadyBanner.activeInHierarchy)
            allPlayersReadyBanner.SetActive(false);

        // Connect to events
        CharacterSelectMenuThing characterSelectMenuThing = playerInput.GetComponent<CharacterSelectMenuThing>();
        characterSelectMenuThing.CharacterSelected += OnCharacterSelected;
        characterSelectMenuThing.CharacterDeselected += OnCharacterDeselected;
        characterSelectMenuThing.StartGame += OnStartGame;
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} left", playerInput.playerIndex);

        playerCount--;

        // Disconnect from events
        CharacterSelectMenuThing characterSelectMenuThing = playerInput.GetComponent<CharacterSelectMenuThing>();
        characterSelectMenuThing.CharacterSelected -= OnCharacterSelected;
        characterSelectMenuThing.CharacterDeselected -= OnCharacterDeselected;
        characterSelectMenuThing.StartGame -= OnStartGame;
    }


    private void OnCharacterSelected(int playerIndex, int characterIndex)
    {
        Debug.LogFormat("Player {0} chose character {1}", playerIndex, characterIndex);
        // Save player choice
        playerChoiceDict.Add(playerIndex, characterIndex);

        readyPlayerCount++;
        if (readyPlayerCount >= playerCount && playerCount >= 2)
        {
            allPlayersReady = true;
            allPlayersReadyBanner.SetActive(true);
        }
    }


    private void OnCharacterDeselected(int playerIndex)
    {
        if (playerChoiceDict.ContainsKey(playerIndex))
            playerChoiceDict.Remove(playerIndex);

        if (readyPlayerCount >= playerCount && playerCount >= 2)
        {
            allPlayersReadyBanner.SetActive(false);
            allPlayersReady = false;
        }
        readyPlayerCount--;
    }


    private void OnStartGame()
    {
        if (allPlayersReady)
        {
            MultiplayerPlayerSpawner.playerCount = playerCount;
            SceneManager.LoadScene("DevScene");
        }
    }
}
