using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour
{
    public TMP_Text buttonText;

    private Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
    }


    private void Update()
    {
        Color buttonColor = button.GetComponent<CanvasRenderer>().GetColor();
        buttonText.color = new(buttonColor.r, buttonColor.g, buttonColor.b, 1f);
    }
}
