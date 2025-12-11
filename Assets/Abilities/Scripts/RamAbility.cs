using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RamAbility : MonoBehaviour
{
    PlayerMovement PM;
    public float Duration;
    [SerializeField] private float speedValue;

    public SpeedMultiplierCurve ImmediateComboBoostCurve;

    public void StartAbility(float strength, ForwardSpeedMultiplier forwardSpeedMultiplier)
    {
        StartCoroutine(abilityBoost(strength, forwardSpeedMultiplier));
    }

    IEnumerator abilityBoost(float strength, ForwardSpeedMultiplier forwardSpeedMultiplier)
    {
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", speedValue * strength, ImmediateComboBoostCurve);

        Debug.Log("Start ram");

        yield return new WaitForSeconds(Duration);

        Debug.Log("End ram");
        Destroy(gameObject);
    }
}
