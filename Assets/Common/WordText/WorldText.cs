using TMPro;
using UnityEngine;

public class WorldText : MonoBehaviour
{
    public TMP_Text textAsset;
    public TMP_ColorGradient shroomColors;
    public TMP_ColorGradient tornadoColors;
    public TMP_ColorGradient ramColors;


    public void SetUpText(string text, Color color, int trick)
    {
        textAsset.text = text;

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
                textAsset.color = color;
                break;
        }
    }
}
