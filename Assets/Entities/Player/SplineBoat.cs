using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class SplineBoat : MonoBehaviour
{
    [SerializeField] private Transform modelHolder;


    [Header("General movement")]
    [SerializeField] private float baseForwardSpeed = 50f;
    // How far the boat can move forward / backwards form the dolly kart
    [SerializeField] private float frontBackOffsetLimit = 3f;
    [SerializeField] private CinemachineSplineCart dollyKart;
    [SerializeField] private SplineTrack mainTrack;
    public UnityEvent OnLanded;

    [HideInInspector] public float forwardInput;
    [HideInInspector] public float steerInput;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public bool dontChangeMainTrack = false;

    private bool isGrounded = true;
    private bool isDashing = false;
    private float steerSpeed;
    private float distanceTraveled;
    private float currentForwardSpeed = 50f;
    private Dictionary<string, float> forwardSpeedMultipliers = new();
    private SplineTrack currentTrack;
    private Collider colliderReference;


    [Header("Ground movement")]
    [SerializeField] private float groundSteerSpeed = 15f;
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private float colliderDisabledAfterJumpDuration = 0.5f;
    public UnityEvent OnJumped;
    private int jumpsLeft;


    [Header("Air movement")]
    [SerializeField] private float airSteerSpeed = 10f;
    [SerializeField] private float gravity = 50f;
    [SerializeField] private float quickfallSpeed = 75f;
    private bool quickfallStarted = false;
    private float ySpeed;
    private float timeSinceJump;


    [Header("Dashing")]
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashForce = 100f;
    public UnityEvent OnLeftDashed;
    public UnityEvent OnRightDashed;

    private float dashTime;
    private float dashDirection;


    [Header("Respawning")]
    [SerializeField] private float respawnLerpDuration = 1f;
    // How far bellow the track the boat has to be before respawning
    [SerializeField] private float deathYPosition = -25f;
    public UnityEvent OnRespawnStarted;
    public UnityEvent OnRespawnEnded;

    private bool isDead = false;
    private float respawnLerpTime;
    private Vector3 respawnLerpStart;



    private void Start()
    {
        // Set the start base speed
        currentForwardSpeed = baseForwardSpeed;
        colliderReference = GetComponent<CapsuleCollider>();
    }


    private void Update()
    {
        // Lerp to death cam position
        if (isDead == true)
        {
            RespawnLerp();
            return;
        }

        // Update dollyKart speed
        SetSpeed(GetCurrentSpeed());

        // Apply general movement
        GeneralMovement();

        // Apply state specific movement
        if (isGrounded == true)
            GroundMovement();
        else
            AirMovement();

        // Animate model
        ModelAnim();
    }


    private void OnTriggerEnter(Collider other)
    {
        // Don't change main track when it's inside a DontChangeMainTrack trigger
        if (dontChangeMainTrack == true && isGrounded == true)
        {
            Debug.Log("I want to saty on the main track");
            return;
        }
        if (other.TryGetComponent(out SplineTrack splineTrack) && (isGrounded == false || splineTrack != currentTrack))
        {
            // Change track when landing on a rail
            if (currentTrack != splineTrack)
            {
                // Change the speed if the spline track has a faster override speed
                bool hasFasterSpeed = splineTrack.overrideSpeed > baseForwardSpeed;
                currentForwardSpeed = hasFasterSpeed ? splineTrack.overrideSpeed : baseForwardSpeed;

                // Set the new track
                dollyKart.Spline = splineTrack.track;
                currentTrack = splineTrack;
                // Set the new track to a main track if it isn't a rail
                if (splineTrack.IsGrindRail == false)
                {
                    mainTrack = splineTrack;
                }
            }

            // Landing
            // Reset stuff when landing
            ySpeed = 0f;
            dashTime = 0f;
            isDashing = false;
            isGrounded = true;

            // Set the dollyKarts position
            TrackDistanceInfo distanceInfo = splineTrack.GetDistanceInfoFromPosition(transform.position);
            dollyKart.SplinePosition = distanceInfo.distance;
            // Make the kart move again
            dollyKart.AutomaticDolly.Enabled = true;

            // Set the landing position
            float xPosition = Vector3.Distance(transform.position, distanceInfo.nearestSplinePos);

            // Check which side of the track the palyer lands on
            Vector3 directionToPlayer = transform.position - distanceInfo.nearestSplinePos;
            float rightDirDot = Vector3.Dot(transform.right, directionToPlayer);
            // If rightDirDot is greater than 0 then the boat landed on the right side of the track
            if (rightDirDot < 0)
                xPosition *= -1f;

            transform.localPosition = new Vector3(xPosition, 0f, 0F);

            // Invoke events
            OnLanded.Invoke();
            splineTrack.OnBoatEnter.Invoke(gameObject);
        }
    }


    private void GeneralMovement()
    {
        // Apply steering
        steerSpeed = airSteerSpeed;
        if (isGrounded)
            steerSpeed = groundSteerSpeed;
        transform.localPosition += Vector3.right * (steerInput * steerSpeed) * Time.deltaTime;
        // Apply dashing
        if (isDashing == true)
        {
            transform.localPosition += Vector3.right * dashDirection * dashForce * dashTime * Time.deltaTime;
        }
    }


    private void GroundMovement()
    {
        timeSinceJump = 0f;
        // Reset how many jumps the boat has
        jumpsLeft = maxJumps;
        quickfallStarted = false;

        // Move boat forwards
        float forwardMovement = transform.localPosition.z + forwardInput * groundSteerSpeed * (1.1f - (MathF.Abs(transform.localPosition.z) / frontBackOffsetLimit)) * Time.deltaTime;
        // Limit how far forward/backwards the boat can travel
        float forwardLimit = Mathf.Clamp(forwardMovement, -frontBackOffsetLimit, frontBackOffsetLimit);
        // Apply forwards movement
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, forwardLimit);

        // Stop the boat from going off the sides of the track
        // Divided by 2 since the end of the width is only half of the width
        if (Mathf.Abs(transform.localPosition.x) > (currentTrack.width / 2f))
        {
            float sidewaysLimit = Mathf.Sign(transform.localPosition.x) * (currentTrack.width / 2.0f);
            // Apply sideways limit
            transform.localPosition = new Vector3(sidewaysLimit, transform.localPosition.y, transform.localPosition.z);
        }

        // Calculate how far the boat has traveled on the current track
        distanceTraveled = currentTrack.track.Spline.GetLength();

        // Do something when the boat reaches the end of a track
        if (dollyKart.SplinePosition > (0.999f * distanceTraveled))
        {
            // Jump off the track if it's a rail
            if (currentTrack.IsGrindRail)
            {
                Jump();
            }

            currentTrack.OnBoatReachedEnd.Invoke(gameObject);
        }

        // Dash cooldown
        // Stop dashing when dashTime is less or equal to 0
        if (dashTime <= 0f)
        {
            isDashing = false;
            dashTime = 0f;
        }
        // Reduce dashTime when dashing
        else
        {
            dashTime -= Time.deltaTime;
            dashTime = Mathf.Max(dashTime, 0f);
        }
    }


    private void AirMovement()
    {
        // Move boat forwards
        transform.localPosition += Vector3.forward * GetCurrentSpeed() * Time.deltaTime;

        // Get fall speed
        float fallSpeed = gravity;
        if (isJumping == false || quickfallStarted == true)
        {
            fallSpeed = gravity + quickfallSpeed;
            quickfallStarted = true;
        }
        ySpeed -= fallSpeed * Time.deltaTime * MathF.Pow(timeSinceJump + 0.5f, 2f);
        // Apply fall speed
        transform.localPosition += Vector3.up * ySpeed * Time.deltaTime;

        // Start respawn if the boat falls far bellow the track
        if (transform.localPosition.y < deathYPosition && isDead == false)
        {
            StartRespawn();
        }
    }

    // Set the new speed of the dolly kart
    private void SetSpeed(float newSpeed)
    {
        if (dollyKart.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
        {
            autoDolly.Speed = newSpeed;
        }
    }


    private float GetCurrentSpeed()
    {
        return currentForwardSpeed * GetTotalSpeedMultipliers();
    }


    // Get the total speed multipler form the forwardSpeedMultipliers dict
    public float GetTotalSpeedMultipliers()
    {
        float multiplier = 1f;
        for (int i = 0; i < forwardSpeedMultipliers.Count; i++)
        {
            var item = forwardSpeedMultipliers.ElementAt(i);
            float value = item.Value;
            multiplier += value;
        }
        return multiplier;
    }


    public void Jump()
    {
        if (jumpsLeft > 0)
        {
            // Reduce how many jumps the player has remaining
            jumpsLeft -= 1;
            // Reset stuff
            isDashing = false;
            isGrounded = false;
            timeSinceJump = 0f;

            // Move the boat upwards
            ySpeed = jumpPower;

            // Detach the boat form the track
            dollyKart.AutomaticDolly.Enabled = false;

            // Disable collider for a bit after jumping
            StartCoroutine(DisableColliderBriefly());

            // Reattach the boat to the main track when jumping of a rail
            if (currentTrack.IsGrindRail)
            {
                Vector3 globalPosition = transform.position;
                // Attach the boat to the main track when jumping off a rail
                currentTrack = mainTrack;
                dollyKart.Spline = mainTrack.track;

                // Get the position relative to the main track
                TrackDistanceInfo distanceInfo = mainTrack.GetDistanceInfoFromPosition(globalPosition);
                Vector3 newPosition = globalPosition - distanceInfo.nearestSplinePos;

                // Set the position of the dolly kart
                dollyKart.SplinePosition = distanceInfo.distance;

                // Set the position of the boat
                transform.localPosition = newPosition;
                SetSpeed(baseForwardSpeed);
            }

            // Invoke events
            OnJumped.Invoke();
            currentTrack.OnBoatExit.Invoke(gameObject);
        }
    }


    public void DashLeft()
    {
        if (isDashing == false)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashDirection = -1f;
            OnLeftDashed.Invoke();
        }
    }


    public void DashRight()
    {
        if (isDashing == false)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashDirection = 1f;
            OnRightDashed.Invoke();
        }
    }


    private void ModelAnim()
    {
        Vector3 newRotation = modelHolder.transform.localEulerAngles;

        newRotation.x = -ySpeed * 2f;

        float y_from = newRotation.y;
        float y_to = steerInput * steerSpeed * 2f + dashTime * dashDirection * 200f * (isGrounded ? 5f : 1f);
        newRotation.y = Mathf.LerpAngle(y_from, y_to, Time.deltaTime * 5f);

        newRotation.z = (isGrounded == false && isDashing) ? newRotation.z + 400f * dashDirection * Time.deltaTime : Mathf.LerpAngle(newRotation.z, -steerInput * steerSpeed, Time.deltaTime * 5f);

        float horizontal_scale = Mathf.Clamp(timeSinceJump * timeSinceJump * 1.5f, 0.5f, 1f);
        float vertical_scale = Mathf.Clamp(1 - timeSinceJump * timeSinceJump, Mathf.Max(timeSinceJump, 1f), 2f);
        modelHolder.localScale = isGrounded ? Vector3.one : new Vector3(horizontal_scale, vertical_scale, horizontal_scale);

        // Apply rotations
        modelHolder.transform.localEulerAngles = newRotation;
    }


    private void StartRespawn()
    {
        // Start the respawn lerp
        isDead = true;
        respawnLerpTime = 0f;
        respawnLerpStart = transform.localPosition;

        // Reset stuff
        ySpeed = 0f;
        isGrounded = true;

        // Invoke event
        OnRespawnStarted.Invoke();
    }


    private void RespawnLerp()
    {
        Vector3 from = respawnLerpStart;
        Vector3 to = Vector3.zero;
        // Get the % of how far the boat should've moved between from and to
        respawnLerpTime += Time.deltaTime;
        float weight = respawnLerpTime / respawnLerpDuration;

        // Move boat to the to position over
        transform.localPosition = Vector3.Lerp(from, to, weight);

        // Stop respawning when it has respawned for respawnLerpDuration
        if (respawnLerpTime >= respawnLerpDuration)
        {
            isDead = false;
            dollyKart.AutomaticDolly.Enabled = true;
            OnRespawnEnded.Invoke();
        }
    }


    private IEnumerator DisableColliderBriefly()
    {
        colliderReference.enabled = false;
        yield return new WaitForSeconds(colliderDisabledAfterJumpDuration);
        colliderReference.enabled = true;
    }
}
