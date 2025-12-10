using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public BoostMeter boostMeter;
    public GameObject levelEndScreen;
    public TMP_Text finishedTimeText;

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


    public void UpdateBoostMeter(int i, int a, int x)
    {
        boostMeter.OnUpdateBoostMeter(i, a, x);
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
}
