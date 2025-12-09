using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelStart : MonoBehaviour
{
    [SerializeField] private float raceCountdownDuration = 3f;
    [SerializeField] private List<Sprite> countdownImages = new();

    private float raceCountdownTime = 0f;
    private int countdownImageIndex = 0;
    private List<PlayerController> players = new();

    public static Action<float> LevelStarted;
    public static Action<Sprite> UpdateCountDownImage;


    private void Start()
    {
        raceCountdownTime = raceCountdownDuration + 1f;
    }


    private void Update()
    {
        if (raceCountdownTime > 0f)
        {
            raceCountdownTime -= Time.deltaTime;

            int newCountdownIndex = (int)raceCountdownTime;
            switch (newCountdownIndex)
            {
                case 0:
                    UpdateCountdownIndex(newCountdownIndex);
                    break;
                case 1:
                    UpdateCountdownIndex(newCountdownIndex);
                    break;
                case 2:
                    UpdateCountdownIndex(newCountdownIndex);
                    break;
            }

            // Start race when countdown is over
            if (raceCountdownTime <= 0f)
            {
                StartRace();
            }
        }
    }


    private void UpdateCountdownIndex(int newIndex)
    {
        if (countdownImageIndex != newIndex)
        {
            countdownImageIndex = newIndex;
            UpdateCountDownImage?.Invoke(countdownImages[countdownImageIndex]);
        }
    }



    private void StartRace()
    {
        UpdateCountDownImage?.Invoke(countdownImages.Last());
        LevelStarted?.Invoke(Time.time);

        for (int i = 0; i < players.Count; i++)
        {
            players[i].inputEnabled = true;
            players[i].splineCart.AutomaticDolly.Enabled = true;
        }
    }


    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerController playerController = playerInput.GetComponent<PlayerController>();
        playerController.splineCart.AutomaticDolly.Enabled = false;
        playerController.inputEnabled = false;

        players.Add(playerController);
    }
}
