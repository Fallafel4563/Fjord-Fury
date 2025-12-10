using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RamAbility : MonoBehaviour
{
    PlayerMovement PM;
    public float Duration;
    [SerializeField] private float speedValue;

    public SpeedMultiplierCurve ImmediateComboBoostCurve;

    public void StartAbility(ForwardSpeedMultiplier forwardSpeedMultiplier)
    {
        StartCoroutine(abilityBoost(forwardSpeedMultiplier));
    }

    IEnumerator abilityBoost(ForwardSpeedMultiplier forwardSpeedMultiplier)
    {
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", 1f + speedValue, ImmediateComboBoostCurve);

        Debug.Log("Start ram");

        yield return new WaitForSeconds(Duration);

        Debug.Log("End ram");
        Destroy(gameObject);
    }
}
