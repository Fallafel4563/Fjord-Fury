using System.Linq;
using UnityEngine;

public class RamAbility : MonoBehaviour
{
    public SpeedMultiplierCurve ImmediateComboBoostCurve;


    public void StartAbility(float strength, ForwardSpeedMultiplier forwardSpeedMultiplier, PlayerObstacleCollisions playerObstacleCollisions, float duration, Transform owner)
    {
        GetComponentInChildren<RamCrash>().Enable(owner, strength);
        StartCoroutine(playerObstacleCollisions.ActivateInvulnerable(duration));
        // Set how long the boost will last
        ImmediateComboBoostCurve.holdTime = duration - ImmediateComboBoostCurve.endCurve.keys.Last().time - ImmediateComboBoostCurve.startCurve.keys.Last().time;
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Ram", 1f * strength, ImmediateComboBoostCurve);
    }
}
