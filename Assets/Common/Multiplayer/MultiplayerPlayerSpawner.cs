using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInputManager))]
public class MultiplayerPlayerSpawner : MonoBehaviour
{
    public int playerCount = 1;
    public static Dictionary<int, PlayerSelectInfo> players = new();

    [SerializeField] private SplineTrack mainTrack;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject hudPrefab;

    private PlayerInputManager playerInputManager;
    private LevelStart levelStart;


    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.playerPrefab = playerPrefab;

        levelStart = GetComponent<LevelStart>();
    }


    private void Start()
    {
        // Spawn players from the character select menu
        if (players.Count > 0)
        {
            SpawnPlayersFromDict();
        }
        else
        {
            SpawnPlayersWhenTesting();
        }

        if (levelStart.enabled)
            StartCoroutine(levelStart.StartCountdown());
    }


    // Spawn players from the players dict
    private void SpawnPlayersFromDict()
    {
        for (int i = 0; i < players.Count; i++)
        {
            var item = players.ElementAt(i);
            playerInputManager.JoinPlayer(item.Key, item.Key, pairWithDevice: item.Value.inputDevice);
        }
    }


    private void SpawnPlayersWhenTesting()
    {
        // Spawn players when testing a level in the editor
        for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
        {
            // Add an entry to the players dict
            PlayerSelectInfo playerSelectInfo = new()
            {
                characterIndex = 0,
                totalTimeSpent = 0f,
                inputDevice = InputSystem.devices[0],
            };
            players.Add(playerIndex, playerSelectInfo);

            playerInputManager.JoinPlayer(playerIndex, playerIndex);
        }
    }


    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} joined", playerInput.playerIndex);

        // Spawn hud
        GameObject hud = Instantiate(hudPrefab);
        PlayerHud playerHud = hud.GetComponent<PlayerHud>();

        PlayerController playerController = playerInput.GetComponent<PlayerController>();
        // Set the main track reference on the spawned player
        playerController.mainTrack = mainTrack;
        // Set the position of the player to be on the main track
        playerController.transform.position = mainTrack.transform.position;
        // Connect hud to player
        playerController.playerHud = playerHud;
        // Tell palyer controller which character the player that is controlling it chose
        if (players.Count > 0)
            playerController.selectedCharacter = players[playerInput.playerIndex].characterIndex;

        SetPlayerPos(playerController, playerInput);
    }


    private void SetPlayerPos(PlayerController playerController, PlayerInput playerInput)
    {
        // Set player position
        float trackLeftPos = -(mainTrack.width / 2f);

        // Get the offset between players
        float playersOffset = mainTrack.width / (PlayerInput.all.Count + 1);

        // Get the spawn pos of the player
        float spawnPos = trackLeftPos + (playersOffset * (playerInput.playerIndex + 1));

        // Set the spawn pos of the player
        playerController.playerMovement.transform.localPosition = new(spawnPos, 0f, 0f);
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} left", playerInput.playerIndex);
    }
}
