using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public BoostMeter boostMeter;
    public GameObject levelEndScreen;
    public GameObject firstPlaceShine;
    public TMP_Text finishedTimeText, placementText;
    public Image placementImage;
    private int playerIndex;
    private Canvas canvas;


    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }


    private void OnEnable()
    {
        LevelEndTrigger.PlayerReachedLevelEnd += OnPlayerReachedLevelEnd;
    }


    private void OnDisable()
    {
        LevelEndTrigger.PlayerReachedLevelEnd -= OnPlayerReachedLevelEnd;
    }


    private void Start()
    {
        levelEndScreen.SetActive(false);
    }


    public void SetupHud(int index, Camera renderCamera)
    {
        playerIndex = index;

        canvas.worldCamera = renderCamera;
        canvas.planeDistance = 0.5f;
    }



    public void UpdateBoostMeterVisibility(bool visible)
    {
        boostMeter.gameObject.SetActive(visible);
    }


    public void UpdateBoostMeter(UpdateBoostMeterInfo updateBoostMeterInfo)
    {
        boostMeter.OnUpdateBoostMeter(updateBoostMeterInfo);
    }


    public void OnRespawnFadeInStarted(float fadeDuration)
    {
        //
    }

    
    public void OnRespawnFadeOutStarted(float fadeDuration)
    {
        //
    }


    private void OnPlayerReachedLevelEnd(int index, float timeSpent)
    {
        if (index == playerIndex)
        {
            levelEndScreen.SetActive(true);
            finishedTimeText.text = string.Format("{0} secs", timeSpent);
        }
    }

    public void SetFirstPlayerShine(int playerPlacement)
    {

        firstPlaceShine.SetActive(playerPlacement == 1);
    
    }
}
