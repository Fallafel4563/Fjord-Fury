using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInputManager))]
public class MultiplayerPlayerSpawner : MonoBehaviour
{
    public static int playerCount = 2;
    public static Dictionary<int, PlayerSelectInfo> players = new();

    [SerializeField] private SplineTrack mainTrack;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject hudPrefab;

    private PlayerInputManager playerInputManager;


    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }


    private void Start()
    {
        // Spawn players from the character select menu
        if (players.Count > 0)
        {
            for (int i = 0; i < players.Count; i++)
            {
                var item = players.ElementAt(i);
                playerInputManager.JoinPlayer(item.Key, item.Key, pairWithDevice: item.Value.inputDevice);
            }
            Debug.Log("Spawned players");
            return;
        }

        // Spawn players when testing a level in editor
        for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
        {
            playerInputManager.JoinPlayer(playerIndex, playerIndex, pairWithDevice: Gamepad.all[0]);
            //continue;
            //if (playerIndex == 0)
            //    playerInputManager.JoinPlayer(playerIndex, playerIndex, pairWithDevices: InputSystem.devices[0]);
            //    //playerInputManager.JoinPlayer(playerIndex, playerIndex, pairWithDevices: [InputSystem.devices[0], InputSystem.devices[1]]);
            //else
            //    Debug.LogFormat("Player {0}", playerIndex);
            //continue;
            // Pair player with game pad, if it exists
            //if (Gamepad.all.Count > playerIndex)
            //    playerInputManager.JoinPlayer(playerIndex, playerIndex, pairWithDevice: Gamepad.all[playerIndex]);
            //// Pair player with keyboard if there's no gamepad
            //else
            //    playerInputManager.JoinPlayer(playerIndex, playerIndex);
        }
    }


    private void OnValidate()
    {
        // Setup the palyer input manager to have the desired default values

        if (playerInputManager == null)
        {
            playerInputManager = GetComponent<PlayerInputManager>();
            playerInputManager.splitScreen = true;
        }
        else if (playerPrefab != null)
        {
            playerInputManager.playerPrefab = playerPrefab;
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
        playerController.selectedCharacter = players[playerInput.playerIndex].characterIndex;

        SetPlayerPos(playerController, playerInput);
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} left", playerInput.playerIndex);
    }


    private void SetPlayerPos(PlayerController playerController, PlayerInput playerInput)
    {
        // Set player position
        float trackLeftPos = -(mainTrack.width / 2f);

        // Get the offset between players
        float playersOffset = mainTrack.width / (playerCount + 1);

        // Get the spawn pos of the player
        float spawnPos = trackLeftPos + (playersOffset * (playerInput.playerIndex + 1));

        // Set the spawn pos of the player
        playerController.playerMovement.transform.localPosition = new(spawnPos, 0f, 0f);
    }
}
