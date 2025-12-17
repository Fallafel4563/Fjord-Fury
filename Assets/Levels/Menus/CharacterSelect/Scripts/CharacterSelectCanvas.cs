using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectCanvas : MonoBehaviour
{
    [SerializeField] private NextSceneLoading nextSceneLoading;
    [SerializeField] private GameObject allPlayersReadyBanner;
    [SerializeField] private List<CharacterJoinPanel> characterJoinPanels = new();

    private bool allPlayersReady = false;
    private int readyPlayerCount = 0;
    private Dictionary<int, PlayerSelectInfo> playerChoiceDict = new();


    private void Start()
    {
        allPlayersReadyBanner.SetActive(false);
        // TODO: Spawn in the first player automatically
    }


    public void OnPlayerJoined(PlayerInput playerInput)
    {

        // Hide banner when a new player joins
        if (PlayerInput.all.Count > readyPlayerCount && allPlayersReadyBanner.activeInHierarchy)
            allPlayersReadyBanner.SetActive(false);

        CharacterJoinPanel joinPanel = characterJoinPanels[playerInput.playerIndex];
        joinPanel.SetPanelState(CharacterJoinPanel.PanelState.Choosing);

        // Set
        playerInput.transform.SetParent(joinPanel.transform, false);
        playerInput.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        // Connect to character select options events
        CharacterSelectOptions characterSelectOptions = playerInput.GetComponent<CharacterSelectOptions>();
        characterSelectOptions.CharacterSelected += OnCharacterSelected;
        characterSelectOptions.CharacterDeselected += OnCharacterDeselected;
        characterSelectOptions.StartGame += OnStartGame;
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        CharacterJoinPanel joinPanel = characterJoinPanels[playerInput.playerIndex];
        joinPanel.SetPanelState(CharacterJoinPanel.PanelState.Inactive);

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

        CharacterJoinPanel joinPanel = characterJoinPanels[playerIndex];
        joinPanel.SetPanelState(CharacterJoinPanel.PanelState.Selected);


        // Show banner if all player are ready
        readyPlayerCount++;
        if (readyPlayerCount >= PlayerInput.all.Count && PlayerInput.all.Count >= 2)
        {
            allPlayersReady = true;
            allPlayersReadyBanner.SetActive(true);
        }
    }


    private void OnCharacterDeselected(int playerIndex)
    {
        CharacterJoinPanel joinPanel = characterJoinPanels[playerIndex];
        joinPanel.SetPanelState(CharacterJoinPanel.PanelState.Choosing);
        // Remove choice form dict when a player deselects their character
        if (playerChoiceDict.ContainsKey(playerIndex))
            playerChoiceDict.Remove(playerIndex);

        // Hide all player ready banner when a player deselects a character
        if (readyPlayerCount >= PlayerInput.all.Count && PlayerInput.all.Count >= 2)
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
            nextSceneLoading.LoadSceneCoroutine();
        }
    }
}
