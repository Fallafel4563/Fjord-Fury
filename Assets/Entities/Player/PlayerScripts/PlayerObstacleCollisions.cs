using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerObstacleCollisions : MonoBehaviour
{
    public float invulnerableDuration = 3f;
    public TrickComboSystem trickComboSystem;
    public ForwardSpeedMultiplier forwardSpeedMultiplier;
    public PlayerMovement playerMovement;
    // TODO: Invulnerable shader
    // TODO: Crash sound
    // TODO: Ethereal sound

    public bool invulnerable { get; set; } = false;
    public bool ramBoostActive { get; set; } = false;


    public UnityEvent HitObstacleOnGround;
    public UnityEvent HitObstacleInAir;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Obstacle obstacle))
        {
            if (!invulnerable && (!obstacle.causeHarm || obstacle.owner != this.transform))
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

        // This is completely redundant since the HitObstacle event is connected to the trick and combo system
        if (trickComboSystem.performingTrick)
            trickComboSystem.FailTrick();

        // TODO: Play crash sound
        StartCoroutine(ActivateInvulnerable());

        if (playerMovement.isGrounded || playerMovement.isDrifting)
            HitObstacleOnGround.Invoke();
        else
            HitObstacleInAir.Invoke();
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
