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

        // Set the main track reference on the spawned player
        CinemachineSplineCart dollyCart = playerInput.GetComponentInChildren<CinemachineSplineCart>();
        dollyCart.Spline = mainTrack.track;
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} left", playerInput.playerIndex);
    }


    private void OnValidate()
    {
        // Setup the palyer input manager to have the desired default values
        if (playerInputManager == null)
        {
            playerInputManager = GetComponent<PlayerInputManager>();
            playerInputManager.playerPrefab = playerPrefab;
            playerInputManager.splitScreen = true;
            //playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        }
    }
}
