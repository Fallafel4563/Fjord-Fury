using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForwardSpeedMultiplier : MonoBehaviour
{
    private Dictionary<string, SpeedMultiplier> forwardSpeedMultipliers = new();


    public float GetTotalMultiplierValue()
    {
        float totalSpeedMultiplier = 1f;
        for (int i = 0; i < forwardSpeedMultipliers.Count; i++)
        {
            var item = forwardSpeedMultipliers.ElementAt(i);
            string key = item.Key;
            SpeedMultiplier value = item.Value;

            // Multiply the multipliers together
            totalSpeedMultiplier *= value.GetMultiplierValue(Time.time);
            // Remove the value when it has reached the end
            if (value.shouldDelete == true)
                forwardSpeedMultipliers.Remove(key);
        }
        return totalSpeedMultiplier;
    }


    public void SetForwardSpeedMultiplier(string name, float value, SpeedMultiplierCurve multiplierCurve = null)
    {
        if (forwardSpeedMultipliers.ContainsKey(name))
        {
            // Don't restart the multipler curve form the beginning startCruve if the new speed multipler allready exists
            // Rather skip the start curve and go directly to the hold value
            if (multiplierCurve != null)
            {
                if (multiplierCurve.startCurve.keys.Count() > 0)
                    forwardSpeedMultipliers[name].timeOffset = multiplierCurve.startCurve.keys.Last().time;
            }
            else
                forwardSpeedMultipliers[name].timeOffset = 0f;

            forwardSpeedMultipliers[name].timeOnStart = Time.time;
            forwardSpeedMultipliers[name].value = value;
            forwardSpeedMultipliers[name].multiplierCurve = multiplierCurve;
        }
        else
        {
            // Create new Speed multiplier value
            SpeedMultiplier speedMultiplier = new();
            speedMultiplier.value = value;
            speedMultiplier.timeOnStart = Time.time;
            speedMultiplier.multiplierCurve = multiplierCurve;

            // Add value to dict
            forwardSpeedMultipliers[name] = speedMultiplier;
        }
    }


    public SpeedMultiplier GetForwardSpeedMultiplier(string name)
    {
        if (forwardSpeedMultipliers.ContainsKey(name))
            return forwardSpeedMultipliers[name];
        else
            return null;
    }
}


[Serializable]
public class SpeedMultiplier
{
    public float value;
    public float timeOnStart;
    public float timeOffset = 0f;
    public SpeedMultiplierCurve multiplierCurve;

    [HideInInspector] public bool shouldDelete = false;
    [HideInInspector] public float activeTime = 0f;


    public float GetMultiplierValue(float time)
    {
        if (multiplierCurve != null)
        {
            // How long the curve has been active for
            activeTime = time - timeOnStart + timeOffset;

            // Make sure the speedmultier curves has points that it can use
            if (multiplierCurve.startCurve.keys.Count() > 0 && multiplierCurve.endCurve.keys.Count() > 0)
            {
                // When the start curve ends
                float startCurveTime = multiplierCurve.startCurve.keys.Last().time;
                // When the hold time ends
                float endCurveTime = startCurveTime + multiplierCurve.holdTime;
                // When to remove the speed multiplier
                float deleteCurveTime = endCurveTime + multiplierCurve.endCurve.keys.Last().time;
    
                if (activeTime < startCurveTime)
                {
                    float returnValue = Mathf.Lerp(1, value, multiplierCurve.startCurve.Evaluate(activeTime));
                    //Debug.LogFormat("Start curve value {0}, value {1}", returnValue, value);
                    return returnValue;
                }
                else if (activeTime < endCurveTime && activeTime > startCurveTime)
                {
                    return value;
                }
                else if (activeTime < deleteCurveTime && activeTime > endCurveTime)
                {
                    float returnValue = Mathf.Lerp(1, value, multiplierCurve.endCurve.Evaluate(Mathf.Abs(endCurveTime - activeTime)));
                    //Debug.LogFormat("End curve value {0}, value {1}", returnValue, value);
                    return returnValue;
                }
                else
                    shouldDelete = true;
            }
            else
            {
                // Stop multiplers from being permanent if there isn't a valid start or end curve
                float holdTimeEnd = multiplierCurve.holdTime;
                if (activeTime < holdTimeEnd)
                    return value;
                else
                    shouldDelete = true;
            }
        }
        return value;
    }
}

[Serializable]
public class SpeedMultiplierCurve
{
    public float holdTime = 3f;
    public AnimationCurve startCurve;
    public AnimationCurve endCurve;

    public float GetLength()
    {
        return holdTime + startCurve.keys.Last().time + endCurve.keys.Last().time;
    }
}
