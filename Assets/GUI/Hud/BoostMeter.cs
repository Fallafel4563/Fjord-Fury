using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostMeter : MonoBehaviour
{
    public Image abilityIcon;
    public List<RectTransform> trickTypeIncrease = new();
    public List<Sprite> abilityIcons = new();


    private int biggerIndex = 0;
    private int longerIndex = 0;
    private int strongerIndex = 0;


    private void Start()
    {
        OnResetBoostMeter();
    }


    public void OnUpdateBoostMeter(UpdateBoostMeterInfo updateBoostMeterInfo)
    {
        Debug.LogFormat("Combo {0}, First {1}, Threshold {2}, Type {3}", updateBoostMeterInfo.combo, updateBoostMeterInfo.firstTrickIndex, updateBoostMeterInfo.abilityActivationThreshold, updateBoostMeterInfo.trickType);
        if (updateBoostMeterInfo.combo < updateBoostMeterInfo.abilityActivationThreshold)
        {
            abilityIcon.sprite = abilityIcons[updateBoostMeterInfo.firstTrickIndex];
        }
        else if (updateBoostMeterInfo.combo >= updateBoostMeterInfo.abilityActivationThreshold)
        {
            abilityIcon.sprite = abilityIcons[updateBoostMeterInfo.firstTrickIndex + 3];
        }

        // Set the bars visible
        if (updateBoostMeterInfo.combo > 1)
        {
            int childIndex = 0;
            switch (updateBoostMeterInfo.trickType)
            {
                case 0:
                    childIndex = biggerIndex;
                    biggerIndex++;
                    break;
                case 1:
                    childIndex = longerIndex;
                    longerIndex++;
                    break;
                case 2:
                    childIndex = strongerIndex;
                    strongerIndex++;
                    break;
            }

            if (childIndex < trickTypeIncrease[updateBoostMeterInfo.trickType].childCount)
                trickTypeIncrease[updateBoostMeterInfo.trickType].GetChild(childIndex).gameObject.SetActive(true);
        }
    }


    public void OnResetBoostMeter()
    {
        biggerIndex = 0;
        longerIndex = 0;
        strongerIndex = 0;

        abilityIcon.sprite = null;

        for (int i = 0; i < trickTypeIncrease.Count; i++)
        {
            Transform trickTypeTransform = trickTypeIncrease[i];
            for (int i2 = 0; i2 < trickTypeTransform.childCount; i2++)
            {
                trickTypeTransform.GetChild(i2).gameObject.SetActive(false);
            }
        }
    }
}


public struct UpdateBoostMeterInfo
{
    public int combo;
    public int trickType;
    public int firstTrickIndex;
    public int abilityActivationThreshold;
}
