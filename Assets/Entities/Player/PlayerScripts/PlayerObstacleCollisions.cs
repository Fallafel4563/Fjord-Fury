using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerObstacleCollisions : MonoBehaviour
{
    public float invulnerableDuration = 3f;
    // TODO: Invulnerable shader
    [HideInInspector] public bool invulnerable = false;

    [HideInInspector] public bool ramBoostActive = false;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public TrickComboSystem trickComboSystem;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;

    // TODO: Crash sound
    // TODO: Ethereal sound

    public UnityEvent HitObstacle;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Obstacle obstacle))
        {
            if (!invulnerable && !obstacle.causeHarm && obstacle.owner != transform)
            {
                Crash(obstacle);
            }
        }
    }


    private void Crash(Obstacle obstacle)
    {
        obstacle.OnPlayerCrashed();
        if (ramBoostActive && obstacle.owner == null)
            return;
        
        if (obstacle.bounceHeight > 0f)
        {
            playerMovement.DetachFromCart();
            playerMovement.airVelocity += transform.up * obstacle.bounceHeight;
        }

        if (!obstacle.causeHarm)
            return;

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("CrashBoost", obstacle.crashSpeedMultiplier, obstacle.crashSpeedMultiplierCurve);

        if (trickComboSystem.performingTrick)
            trickComboSystem.FailTrick();


        // TODO: Play crash sound
        StartCoroutine(ActivateInvulnerable());

        HitObstacle.Invoke();
    }


    private IEnumerator ActivateInvulnerable()
    {
        invulnerable = true;
        // TODO: Enable invulnerable shader

        yield return new WaitForSeconds(invulnerableDuration);

        invulnerable = false;
        // TODO: Disable invulnerable shader
    }
}
