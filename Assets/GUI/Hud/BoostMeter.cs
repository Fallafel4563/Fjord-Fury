using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostMeter : MonoBehaviour
{
    public Image boostMeterIcon;
    public List<Image> barSections = new();
    public List<Color> colorPalette = new();
    public List<Sprite> abilityIcons = new();


    private void Start()
    {
        OnResetBoostMeter();
    }


    public void OnUpdateBoostMeter(UpdateBoostMeterInfo updateBoostMeterInfo)
    {
        Debug.LogFormat("Combo {0}, First {1}, Threshold {2}, Bar {3}", updateBoostMeterInfo.combo, updateBoostMeterInfo.firstTrickIndex, updateBoostMeterInfo.abilityActivationThreshold, updateBoostMeterInfo.barIndex);
        if (updateBoostMeterInfo.combo < updateBoostMeterInfo.abilityActivationThreshold)
        {
            boostMeterIcon.sprite = abilityIcons[updateBoostMeterInfo.firstTrickIndex];
        }
        else if (updateBoostMeterInfo.combo >= updateBoostMeterInfo.abilityActivationThreshold)
        {
            boostMeterIcon.sprite = abilityIcons[updateBoostMeterInfo.firstTrickIndex + 3];
        }
        Image sectionToChange = barSections[updateBoostMeterInfo.barIndex];
        int currentColorIndex = colorPalette.IndexOf(sectionToChange.color);
        if (currentColorIndex < colorPalette.Count)
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
}


public struct UpdateBoostMeterInfo
{
    public int combo;
    public int barIndex;
    public int firstTrickIndex;
    public int abilityActivationThreshold;
}
