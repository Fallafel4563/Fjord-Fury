using TMPro;
using UnityEngine;

public class WorldText : MonoBehaviour
{
    public TMP_Text textAsset;


    public void SetUpText(string text, Color color)
    {
        textAsset.text = text;
        textAsset.color = Color.white;
    }
}
