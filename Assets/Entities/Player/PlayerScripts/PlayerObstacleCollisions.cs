using UnityEngine;
using UnityEngine.Events;

public class PlayerObstacleCollisions : MonoBehaviour
{
    public float defaultSlowDownValue = 0.5f;
    public float defaultUpwardsForce = 10f;
    public float defaultKnockbackForce = 30f;
    public float invulnerableDuration = 3f;
    public int maxCrashes = 3;
    public float crashDuration = 3f;
    public UnityEvent HitObstacle;

    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public TrickComboSystem trickComboSystem;

    private bool invulnerable = false;
    private float invulnerableTimer = 0f;

    private int crashCounter = 0;
    private float crashTimer = 0f;



    private void Update()
    {
        // End invulnerable after a short duration
        if (invulnerable)
        {
            invulnerableTimer -= Time.deltaTime;
            if (invulnerableTimer <= 0f)
                EndInvulnerable();
        }

        // Reset crash counter after a short duration
        if (crashTimer >= 0f)
        {
            crashTimer -= Time.deltaTime;
            if (crashTimer <= 0f)
            {
                crashCounter = 0;
                playerMovement.crashing = false;
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacles" && invulnerable == false)
        {
            Crash(other);
        }
    }


    private void Crash(Collider other)
    {
        JumpBoatBackwards(other);

        crashCounter++;
        if (crashCounter >= maxCrashes)
            StartInvulnerable();

        HitObstacle.Invoke();
    }


    private void JumpBoatBackwards(Collider other)
    {
        // Get knockback force
        float knockbackFroce = defaultKnockbackForce;
        float upwardsForce = defaultUpwardsForce;
        if (other.TryGetComponent(out Obstacle obstacle))
        {
            upwardsForce = obstacle.upwardsForce;
            knockbackFroce = obstacle.knockbackFroce;
        }


        if (playerMovement.isGrounded)
            // Make the player airborne
            playerMovement.DetachFromCart();
        
        // Stop dashing
        playerMovement.isDashing = false;

        const float KNOCKBACK_OFFSET = 25f;
        float trackLength = playerMovement.currentTrack.track.Spline.GetLength();
        // Get the knockback direction
        Vector3 behindSplinePos = playerMovement.currentTrack.track.EvaluatePosition((playerMovement.distanceWhenJumped - KNOCKBACK_OFFSET) / trackLength);
        Vector3 knockbackDir = (behindSplinePos - transform.position).normalized;
        playerMovement.airVelocity = knockbackDir * knockbackFroce;
        // Apply an additional upwards force
        playerMovement.airVelocity += playerMovement.transform.up * upwardsForce;
        playerMovement.crashing = true;

        // Fail trick when hitting an obstacle
        if (trickComboSystem.performingTrick)
            trickComboSystem.FailTrick();
    }


    private void StartInvulnerable()
    {
        invulnerable = true;
        invulnerableTimer = invulnerableDuration;
        // TODO: Enable invulnerable shader
        Debug.Log("invulnerable == true");
    }


    private void EndInvulnerable()
    {
        invulnerable = false;
        // TODO: Disable invulnerable shader
        Debug.Log("invulnerable == false");
        crashCounter = 0;
    }
}
