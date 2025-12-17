using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RamAbility : MonoBehaviour
{
    PlayerMovement PM;
    public float Duration;
    [SerializeField] private float speedValue;

    public SpeedMultiplierCurve ImmediateComboBoostCurve;

    public void StartAbility(float strength, ForwardSpeedMultiplier forwardSpeedMultiplier, PlayerObstacleCollisions poc)
    {
        StartCoroutine(abilityBoost(strength, forwardSpeedMultiplier, poc));
    }

    IEnumerator abilityBoost(float strength, ForwardSpeedMultiplier forwardSpeedMultiplier, PlayerObstacleCollisions poc)
    {
        yield return new WaitForSeconds(0.1f);
        poc.invulnerable = true;
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", speedValue * strength, ImmediateComboBoostCurve);

        Debug.Log("Start ram, " + (speedValue * strength).ToString());

        yield return new WaitForSeconds(Duration);

        Debug.Log("End ram");
        poc.invulnerable = false;
        Destroy(gameObject);
    }
}
