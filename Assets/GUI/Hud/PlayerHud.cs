using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public TMP_Text playerIndexText;
    public TMP_Text trickTricksText;
    public TMP_Text trickScoreText;
    public BoostMeter boostMeter;


    private Canvas canvas;


    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }


    public void SetupHud(int playerIndex, Camera renderCamera)
    {
        playerIndexText.text = string.Format("Player {0}", playerIndex + 1);

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
}
