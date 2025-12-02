using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public TMP_Text playerIndexText;
    public TMP_Text trickTricksText;
    public TMP_Text trickScoreText;

    private Canvas canvas;


    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }


    private void Start()
    {
        UpdateTricksText("");
        TrickScoreUpdated("");
    }


    public void SetupHud(int playerIndex, Camera renderCamera)
    {
        playerIndexText.text = string.Format("Player {0}", playerIndex + 1);

        canvas.worldCamera = renderCamera;
        canvas.planeDistance = 0.5f;
    }


    public void UpdateTricksText(string tricks)
    {
        trickTricksText.text = tricks;
    }


    public void TrickScoreUpdated(string score)
    {
        trickScoreText.text = score;
    }
}
