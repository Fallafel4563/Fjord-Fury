using UnityEngine;

public class RamAbility : MonoBehaviour
{
    public SpeedMultiplierCurve ImmediateComboBoostCurve;


    public void StartAbility(float strength, ForwardSpeedMultiplier forwardSpeedMultiplier)
    {
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Ram", 1f * strength, ImmediateComboBoostCurve);
    }
}
