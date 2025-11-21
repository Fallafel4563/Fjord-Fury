using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInputManager))]
public class MultiplayerPlayerSpawner : MonoBehaviour
{
    [SerializeField] private int playerCount = 2;
    [SerializeField] private SplineTrack mainTrack;
    [SerializeField] private GameObject playerPrefab;

    private PlayerInputManager playerInputManager;



    private void Start()
    {
        // Spawn players
        for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
        {
            // Pair player with game pad, if it exists
            if (Gamepad.all.Count > playerIndex)
                playerInputManager.JoinPlayer(playerIndex, playerIndex, pairWithDevice: Gamepad.all[playerIndex]);
            // Pair player with keyboard if there's no gamepad
            else
                playerInputManager.JoinPlayer(playerIndex, playerIndex);
        }
    }


    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} joined", playerInput.playerIndex);

        // Set the main track of the player that joined
        PlayerController playerController = playerInput.GetComponent<PlayerController>();
        playerController.mainTrack = mainTrack;
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} left", playerInput.playerIndex);
    }


    private void OnValidate()
    {
        if (playerInputManager == null)
        {
            playerInputManager = GetComponent<PlayerInputManager>();
        }
    }
}
