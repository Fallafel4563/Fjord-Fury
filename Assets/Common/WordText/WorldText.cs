
using TMPro;
using UnityEngine;

public class WorldText : MonoBehaviour
{
    public TMP_Text textAsset;
    public TMP_ColorGradient shroomColors;
    public TMP_ColorGradient tornadoColors;
    public TMP_ColorGradient ramColors;

    private Transform lookAtTarget;


    public void SetUpText(string text, Color color, int trick, bool useTrickColor, Transform lookTarget = null)
    {
        textAsset.text = text;
        lookAtTarget = lookTarget;

        if (!useTrickColor)
        {
            textAsset.colorGradientPreset = new TMP_ColorGradient(color);
            return;
        }

        switch (trick)
        {
            case 0:
                textAsset.colorGradientPreset = shroomColors;
                break;
            case 1:
                textAsset.colorGradientPreset = tornadoColors;
                break;
            case 2:
                textAsset.colorGradientPreset = ramColors;
                break;
            default:
                textAsset.colorGradientPreset = new TMP_ColorGradient(color);
                break;
        }
    }


    private void Update()
    {
        if (lookAtTarget)
        {
            transform.LookAt(lookAtTarget.transform.position, lookAtTarget.transform.up);
        }
    }
}
