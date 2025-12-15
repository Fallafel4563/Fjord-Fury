using UnityEngine;
using UnityEngine.UI;

public class CharacterJoinPanel : MonoBehaviour
{
    /*
        This script handles the graphics of the individual join panels:
            Fading in and out
            Showing and hiding text
     */

    [SerializeField] Image image;
    [SerializeField] GameObject text;

    [SerializeField] Color inactiveColor;
    [SerializeField] Color choosingColor;
    [SerializeField] Color selectedColor;

    [SerializeField] float fadeTime = .2f;

    [SerializeField] AnimationCurve fadeCurve;

    Color baseColor;
    Color fadedColor;

    float animationTimePoint = 0;
    bool fading;

    public enum PanelState
    {
        Inactive,
        Choosing,
        Selected
    }

    void Start()
    {
        SetPanelState(PanelState.Inactive);
    }

    public void SetPanelState(PanelState panelState)
    {
        switch (panelState)
        {
            case PanelState.Inactive:
                FadeTo(inactiveColor);
                text.SetActive(true);
                break;
            case PanelState.Choosing:
                FadeTo(choosingColor);
                text.SetActive(false);
                break;
            case PanelState.Selected:
                FadeTo(selectedColor);
                break;
        }
    }

    void FadeTo(Color color)
    {
        animationTimePoint = 0;
        fading = true;

        baseColor = image.color;
        fadedColor = color;
    }

    void Update()
    {
        if (!fading)
            return;

        animationTimePoint += Time.deltaTime / fadeTime;

        image.color = Color.Lerp(baseColor, fadedColor, fadeCurve.Evaluate(animationTimePoint));

        if (animationTimePoint >= 1)
        {
            fading = false;
        }
    }
}
