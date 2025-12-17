using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class PlayerHud : MonoBehaviour
{
    public BoostMeter boostMeter;
    public GameObject levelEndScreen;
    public GameObject firstPlaceShine;
    public GameObject respawnFadeObject;
    public Animator animator;
    public TMP_Text finishedTimeText, placementText;
    public Image placementImage;
    private int playerIndex;
    private Canvas canvas;
    
    public List<Sprite> placementIcons = new();

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
        canvas.planeDistance = 0.3001f;
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
        respawnFadeObject.SetActive(false);
        respawnFadeObject.SetActive(true);
        animator.Play("RespawnFade");
    }

    
    public void OnRespawnFadeOutStarted(float fadeDuration)
    {
        //
    }


    private void OnPlayerReachedLevelEnd(int index, float timeSpent, int playerPlacement)
    {
        if (index == playerIndex)
        {
            levelEndScreen.SetActive(true);
            finishedTimeText.text = string.Format("{0} secs", timeSpent);
            SetFirstPlayerShine(playerPlacement);
        }
    }

    public void SetFirstPlayerShine(int playerPlacement)
    {

        firstPlaceShine.SetActive(playerPlacement == 0);
    
    }




}
