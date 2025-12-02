using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class CharacterSelectCanvas : MonoBehaviour
{
    [SerializeField] private GameObject allPlayersReadyBanner;
    [SerializeField] private List<GameObject> characterSelectPositions = new();

    private bool allPlayersReady = false;
    private int playerCount = 0;
    private int readyPlayerCount = 0;
    private Dictionary<int, PlayerSelectInfo> playerChoiceDict = new();


    private void Start()
    {
        allPlayersReadyBanner.SetActive(false);
        // TODO: Spawn in the first player automatically
    }


    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerCount++;

        // Hide banner when a new player joins
        if (playerCount > readyPlayerCount && allPlayersReadyBanner.activeInHierarchy)
            allPlayersReadyBanner.SetActive(false);

        GameObject selectPosition = characterSelectPositions[playerInput.playerIndex];
        selectPosition.transform.GetChild(0).gameObject.SetActive(false);

        // Set
        playerInput.transform.SetParent(selectPosition.transform, false);
        playerInput.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        // Connect to character select options events
        CharacterSelectOptions characterSelectOptions = playerInput.GetComponent<CharacterSelectOptions>();
        characterSelectOptions.CharacterSelected += OnCharacterSelected;
        characterSelectOptions.CharacterDeselected += OnCharacterDeselected;
        characterSelectOptions.StartGame += OnStartGame;
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        playerCount--;

        GameObject selectPosition = characterSelectPositions[playerInput.playerIndex];
        selectPosition.transform.GetChild(0).gameObject.SetActive(true);

        // Disconnect from character select options events
        CharacterSelectOptions characterSelectOptions = playerInput.GetComponent<CharacterSelectOptions>();
        characterSelectOptions.CharacterSelected -= OnCharacterSelected;
        characterSelectOptions.CharacterDeselected -= OnCharacterDeselected;
        characterSelectOptions.StartGame -= OnStartGame;
    }


    private void OnCharacterSelected(int playerIndex, PlayerSelectInfo playerSelectInfo)
    {
        // Save player choice
        playerChoiceDict.Add(playerIndex, playerSelectInfo);

        // Show banner if all player are ready
        readyPlayerCount++;
        if (readyPlayerCount >= playerCount && playerCount >= 2)
        {
            allPlayersReady = true;
            allPlayersReadyBanner.SetActive(true);
        }
    }


    private void OnCharacterDeselected(int playerIndex)
    {
        // Remove choice form dict when a player deselects their character
        if (playerChoiceDict.ContainsKey(playerIndex))
            playerChoiceDict.Remove(playerIndex);

        // Hide all player ready banner when a player deselects a character
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
            // Give the multiplayer spawn information about which player chose what character
            MultiplayerPlayerSpawner.players = playerChoiceDict;
            SceneManager.LoadScene("DevScene");
        }
    }
}
