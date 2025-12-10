using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostMeter : MonoBehaviour
{
    public Image boostMeterIcon;
    public TMP_Text trickReaction;
    public List<Image> barSections = new();
    public List<Color> colorPalette = new();

    public List<Sprite> abilityIcons = new();

    private List<string> trickActiveList = new List<string> {"Mushroom charged", "Ram Charaged", "Tornado charged"};
    private List<string> trickNumberList = new List<string> {"Imp-pressive", "Trolltastic", "Untrollable", "Trolldracular", "Hobgoblike", "Orgewhelming"};


    private void Start()
    {
        OnResetBoostMeter();
    }


    public void OnUpdateBoostMeter(int firstTrickIndex, int combo, int barIndex)
    {
        boostMeterIcon.sprite = abilityIcons[firstTrickIndex - 1];

        if (combo == 3)
        {
            trickReaction.text = trickActiveList[firstTrickIndex - 1];
        }

        if (combo >= 4)
        {
            int indexToUse = combo - 4;
            if (indexToUse >= trickNumberList.Count)
                indexToUse = trickNumberList.Count - 1;
            trickReaction.text = trickNumberList[indexToUse];
        }

        Image sectionToChange = barSections[barIndex];
        int currentColorIndex = colorPalette.IndexOf(sectionToChange.color);
        if (currentColorIndex < colorPalette.Count - 1)
            sectionToChange.color = colorPalette[currentColorIndex + 1];
    }


    public void OnResetBoostMeter()
    {
        boostMeterIcon.sprite = null;
        for (int i = 0; i < barSections.Count; i++)
        {
            barSections[i].color = colorPalette[0];
        }
    }


    public void OnResetTrickReaction()
    {
        trickReaction.text = "";
    }
}
