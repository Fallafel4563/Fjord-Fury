using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Leaderboard : MonoBehaviour
{
    public GameObject playerPositionDisplay;
    public GameObject backgroundImage;

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

        AddPlayerPositionDisplay();

        // Connect the palyer 1 input to the actions in the menu
        PlayerInput player1Input = PlayerInput.GetPlayerByIndex(0);
        player1Input.SwitchCurrentActionMap("Ui");
        player1Input.actions["Accept"].performed += OnAccept;
    }


    private void AddPlayerPositionDisplay()
    {
        if  (alreadyDoneIt ==false)
        for (int i = 0; i < MultiplayerPlayerSpawner.players.Count; i++)
        {
            // Get info about the palyer form the multiplayer player spawner
            var player = MultiplayerPlayerSpawner.players.ElementAt(i);

            // Spawn position dispaly object and set its position
            GameObject positionDisplayObject = Instantiate(playerPositionDisplay, Vector3.zero, Quaternion.identity, backgroundImage.transform);
            positionDisplayObject.GetComponent<RectTransform>().anchoredPosition = places[i].anchoredPosition;

            // Get and update the position display
            LeaderboardPlayerPositionDisplay positionDisplay = positionDisplayObject.GetComponent<LeaderboardPlayerPositionDisplay>();
            positionDisplay.UpdateDispaly(player.Key+1, player.Value.totalTimeSpent, player.Value.characterIndex);
        }
        alreadyDoneIt = true;
    }


    private void OnAccept(CallbackContext callbackContext)
    {
        nextSceneLoading.SceneToLoad = levelToLoad;
        nextSceneLoading.LoadSceneCoroutine();
    }
}
