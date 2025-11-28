using UnityEngine;
using UnityEngine.Events;

public class PlayerObstacleCollisions : MonoBehaviour
{
    public float defaultSlowDownValue = -0.5f;
    public float defaultUpwardsForce = 10f;
    public float defaultKnockbackForce = 30f;
    public SpeedMultiplierCurve slowDownCurve;
    public UnityEvent HitObstacle;

    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        forwardSpeedMultiplier = GetComponent<ForwardSpeedMultiplier>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacles")
        {
            // Get knockback force
            float knockbackFroce = defaultKnockbackForce;
            float upwardsForce = defaultUpwardsForce;
            float slowDownValue = defaultSlowDownValue;
            if (other.TryGetComponent(out Obstacle obstacle))
            {
                slowDownValue = obstacle.slowDownValue;
                upwardsForce = obstacle.upwardsForce;
                knockbackFroce = obstacle.knockbackFroce;
            }


            if (playerMovement.isGrounded)
                // Make the player airborne
                playerMovement.DetachFromCart();
            
            // Stop dashing
            // TODO: Fail trick if dashing
            playerMovement.isDashing = false;

            const float KNOCKBACK_OFFSET = 25f;
            float trackLength = playerMovement.currentTrack.track.Spline.GetLength();
            // Get the knockback direction
            Vector3 behindSplinePos = playerMovement.currentTrack.track.EvaluatePosition((playerMovement.distanceWhenJumped - KNOCKBACK_OFFSET) / trackLength);
            Vector3 knockbackDir = (behindSplinePos - transform.position).normalized;
            playerMovement.airVelocity = knockbackDir * knockbackFroce;
            // Apply an additional upwards force
            playerMovement.airVelocity += playerMovement.transform.up * upwardsForce;

            // Apply a slowing effect
            forwardSpeedMultiplier.SetForwardSpeedMultiplier("HitObstacle", slowDownValue, slowDownCurve);
        }
    }
}
