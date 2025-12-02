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
        // Create new Speed multiplier value
        SpeedMultiplier speedMultiplier = new();
        speedMultiplier.value = value;
        speedMultiplier.timeOnStart = Time.time;
        speedMultiplier.multiplierCurve = multiplierCurve;

        // Add value to dict
        forwardSpeedMultipliers[name.ToLower()] = speedMultiplier;
    }


    public SpeedMultiplier GetForwardSpeedMultiplier(string name)
    {
        if (forwardSpeedMultipliers.ContainsKey(name.ToLower()))
            return forwardSpeedMultipliers[name.ToLower()];
        else
            return null;
    }
}


[Serializable]
public class SpeedMultiplier
{
    public float value;
    public float timeOnStart;
    public SpeedMultiplierCurve multiplierCurve;

    [HideInInspector] public bool shouldDelete = false;


    public float GetMultiplierValue(float time)
    {
        if (multiplierCurve != null)
        {
            // How long the curve has been active for
            float activeTime = time - timeOnStart;

            // When the start curve ends
            float startCurveTime = multiplierCurve.startCurve.keys.Last().time;
            // When the hold time ends
            float endCurveTime = startCurveTime + multiplierCurve.holdTime;
            // When to remove the speed multiplier
            float deleteCurveTime = endCurveTime + multiplierCurve.endCurve.keys.Last().time;

            if (activeTime < startCurveTime)
            {
                return value * multiplierCurve.startCurve.Evaluate(activeTime) + 1f;
            }
            else if (activeTime < endCurveTime && activeTime > startCurveTime)
            {
                return value;
            }
            else if (activeTime < deleteCurveTime && activeTime > endCurveTime)
            {
                return value * multiplierCurve.endCurve.Evaluate(Mathf.Abs(endCurveTime - activeTime)) + 1f;
            }
            else
                shouldDelete = true;
        }
        return value;
    }
}

[Serializable]
public class SpeedMultiplierCurve
{
    public float holdTime;
    public AnimationCurve startCurve;
    public AnimationCurve endCurve;
}
