using System;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    public static int playerCount = 0;

    public string nextLevelToLoad = "";
    public int playersCompleted = 0;

    private float levelStartTime;

    public static Action<string> AllPlayersCompleted;
    public static Action<int, float> PlayerReachedLevelEnd;


    private void OnEnable()
    {
        LevelStart.LevelStarted += OnLevelStarted;
    }


    private void OnDisable()
    {
        LevelStart.LevelStarted -= OnLevelStarted;
    }


    private void Start()
    {
        playersCompleted = 0;
    }


    private void OnLevelStarted(float startTime)
    {
        levelStartTime = startTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement playerMovement))
        {
            int playerIndex = playerMovement.playerController.playerIndex;
            float timeSpent = Time.time - levelStartTime;

            // Modify the data in the players dict
            PlayerSelectInfo playerSelectInfo = MultiplayerPlayerSpawner.players[playerIndex];
            playerSelectInfo.totalTimeSpent += timeSpent;
            MultiplayerPlayerSpawner.players[playerIndex] = playerSelectInfo;

            playerMovement.playerController.inputEnabled = false;
            playerMovement.splineCart.AutomaticDolly.Enabled = false;
            playerMovement.enabled = false;

            PlayerReachedLevelEnd?.Invoke(playerIndex, timeSpent);
            playersCompleted++;
            if (playersCompleted >= playerCount)
            {
                AllPlayersCompleted?.Invoke(nextLevelToLoad);
            }
        }
    }
}
