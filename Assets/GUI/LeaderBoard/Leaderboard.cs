using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Leaderboard : MonoBehaviour
{
    public GameObject playerPositionDisplay;
    public GameObject backgroundImage;
    public GameObject playerMapProgress;

    private string levelToLoad;
    private NextSceneLoading nextSceneLoading;

    public RectTransform[] places;
    bool alreadyDoneIt = false;


    private void Awake()
    {
        nextSceneLoading = GetComponent<NextSceneLoading>();
    }


    private void OnEnable()
    {
        LevelEndTrigger.AllPlayersCompleted += OnAllPlayersComplted;
    }


    private void OnDisable()
    {
        LevelEndTrigger.AllPlayersCompleted -= OnAllPlayersComplted;
    }


    private void Start()
    {
        backgroundImage.SetActive(false);
    }


    private void OnAllPlayersComplted(string nextLevelToLoad)
    {
        levelToLoad = nextLevelToLoad;
        backgroundImage.SetActive(true);
        playerMapProgress.SetActive(false);

        AddPlayerPositionDisplay();

        // Connect the palyer 1 input to the actions in the menu
        PlayerInput player1Input = PlayerInput.GetPlayerByIndex(0);
        player1Input.SwitchCurrentActionMap("Ui");
        player1Input.actions["Accept"].performed += OnAccept;
    }


    private void AddPlayerPositionDisplay()
    {

        List<LeaderboardPlayerPositionListInfo> playerSelectInfos = GetPlayerPositionList();
        playerSelectInfos.Sort( (a, b) => a.timeSpent.CompareTo(b.timeSpent) );

        for (int i = 0; i < playerSelectInfos.Count; i++)
        {
            // Get info about the palyer form the multiplayer player spawner
            LeaderboardPlayerPositionListInfo player = playerSelectInfos[i];

            // Spawn position dispaly object and set its position
            GameObject positionDisplayObject = Instantiate(playerPositionDisplay, Vector3.zero, Quaternion.identity, backgroundImage.transform);
            positionDisplayObject.GetComponent<RectTransform>().anchoredPosition = places[i].anchoredPosition;


            // Get and update the position display
            LeaderboardPlayerPositionDisplay positionDisplay = positionDisplayObject.GetComponent<LeaderboardPlayerPositionDisplay>();
            positionDisplay.UpdateDispaly(player.playerIndex, player.timeSpent, player.characterIndex);
        }
        alreadyDoneIt = true;
    }


    private void OnAccept(CallbackContext callbackContext)
    {
        nextSceneLoading.SceneToLoad = levelToLoad;
        nextSceneLoading.LoadScene();
    }


    private List<LeaderboardPlayerPositionListInfo> GetPlayerPositionList()
    {
        List<LeaderboardPlayerPositionListInfo> infos = new();
        for (int i = 0; i < MultiplayerPlayerSpawner.players.Count; i++)
        {
            var player = MultiplayerPlayerSpawner.players.ElementAt(i);
            LeaderboardPlayerPositionListInfo info = new()
            {
                playerIndex = player.Key,
                characterIndex = player.Value.characterIndex,
                timeSpent = player.Value.totalTimeSpent
            };
            infos.Add(info);
        }
        return infos;
    }
}


public struct LeaderboardPlayerPositionListInfo
{
    public int playerIndex;
    public int characterIndex;
    public float timeSpent;
}
