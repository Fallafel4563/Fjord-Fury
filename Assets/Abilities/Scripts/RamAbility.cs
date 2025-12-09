using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RamAbility : MonoBehaviour
{
    PlayerMovement PM;
    public float Duration;

    public void StartAbility()
    {
        StartCoroutine(abilityBoost());
    }

    IEnumerator abilityBoost()
    {
        float originalSpeed = PM.baseForwardSpeed;
        PM.baseForwardSpeed *= 2f;

        yield return new WaitForSeconds(Duration);

        PM.baseForwardSpeed = originalSpeed;
        Destroy(gameObject);
    }
}
