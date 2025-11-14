using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class SplineBoat : MonoBehaviour
{
    [SerializeField] private Transform modelHolder;


    [Header("General movement")]
    [SerializeField] private float baseForwardSpeed = 50f;
    // How far the boat can move forward / backwards form the dolly kart
    [SerializeField] private float frontBackOffsetLimit = 3f;
    [SerializeField] private CinemachineSplineCart dollyKart;
    [SerializeField] private SplineTrack mainTrack;

    [HideInInspector] public float forwardInput;
    [HideInInspector] public float steerInput;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public bool dontChangeMainTrack = false;

    private bool isGrounded = true;
    private bool isDashing = false;
    private bool wasLastTrackRail = false;
    private float steerSpeed;
    private float distanceTraveled;
    private float currentForwardSpeed = 50f;
    private Dictionary<string, SpeedMultiplier> forwardSpeedMultipliers = new();
    private SplineTrack currentTrack;
    private Collider colliderReference;


    [Header("Ground movement")]
    [SerializeField] private float groundSteerSpeed = 15f;
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private float colliderDisabledAfterJumpDuration = 0.5f;
    public UnityEvent OnLanded;
    public UnityEvent OnJumped;
    private int jumpsLeft;
    private float distanceWhenJumped;
    private Vector3 jumpPosition;


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


#region Start, Update, etc...

    private void Start()
    {
        // Get tracks from dollyKart
        mainTrack = dollyKart.Spline.gameObject.GetComponent<SplineTrack>();
        currentTrack = mainTrack;
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


#endregion

#region Landing


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SplineTrack splineTrack) && (isGrounded == false || splineTrack != currentTrack))
        {
            // Don't change main track when it's inside a DontChangeMainTrack trigger
            // Can still change to rails
            if (dontChangeMainTrack == true && splineTrack != mainTrack && splineTrack.IsGrindRail == false)
            {
                return;
            }

            transform.parent = dollyKart.transform;
            transform.localEulerAngles = Vector3.zero;

            // Stop the player from jumping to a part of the track that is too far ahead (On whirlpool for example)
            TrackDistanceInfo distanceInfo = splineTrack.GetDistanceInfoFromPosition(transform.position);
            // Check if the track it lands on is the same as the current main track and that the the boat didn't jump off a rail
            if (splineTrack == mainTrack && wasLastTrackRail == false)
            {
                // Check how far it has travled while jumping (normal jump distance is around 75 (with a gravity of 75 and quickfall speed of 50))
                // If it's above 200 then the player has found a shortcut that we don't want
                Debug.LogFormat("Landed distance: {0}, Jump distance: {1}", distanceInfo.distance, distanceWhenJumped);
                if (Mathf.Abs(distanceInfo.distance - distanceWhenJumped) > 200f)
                {
                    // Get the distance it has jumped
                    float jumpedDistance = Vector3.Distance(jumpPosition, transform.position);
                    Debug.Log(distanceWhenJumped + jumpedDistance);
                    Debug.Log("You jumped too far");

                    // Get the spline pos that is closest to the position it should've had had it not landed on the wrong part of the track
                    Vector3 desiredWorldPos = splineTrack.track.Spline.EvaluatePosition((distanceWhenJumped + jumpedDistance) / splineTrack.track.Spline.GetLength());
                    Debug.LogFormat("Land pos: {0}, New spline pos: {1}", transform.position, desiredWorldPos);

                    // Override distance info with the distance it should've had, had the boat landed on the right part of the track
                    distanceInfo.distance = distanceWhenJumped + jumpedDistance;
                    distanceInfo.nearestSplinePos = desiredWorldPos;
                }
            }

            float newYpos = 0f;
            // Set the new track
            currentTrack = splineTrack;
            dollyKart.Spline = splineTrack.track;

            // Check if the new track is a rail
            if (splineTrack.IsGrindRail == true)
            {
                wasLastTrackRail = true;
                newYpos = splineTrack.width;
            }
            else
            {
                mainTrack = splineTrack;
                wasLastTrackRail = false;
            }

            // Change the speed if the spline track has a faster override speed
            currentForwardSpeed = baseForwardSpeed;
            if (splineTrack.overrideSpeed > baseForwardSpeed)
                currentForwardSpeed = splineTrack.overrideSpeed;

            // Reset stuff
            ySpeed = 0f;
            dashTime = 0f;
            isDashing = false;
            isGrounded = true;

            // Set the dollyKarts position
            dollyKart.SplinePosition = distanceInfo.distance;
            // Make the kart move again
            dollyKart.AutomaticDolly.Enabled = true;

            // Set the landing position
            // Get how far the boat is in the x position (but we don't know if it's to the left or right)
            float xPosition = Vector3.Distance(transform.position, distanceInfo.nearestSplinePos);

            // Check if the boat landed on the right or left side
            Vector3 directionToPlayer = transform.position - distanceInfo.nearestSplinePos;
            float rightDirDot = Vector3.Dot(transform.right, directionToPlayer);
            // If rightDirDot is greater than 0 then the boat landed on the right side of the track
            if (rightDirDot < 0)
                xPosition *= -1f;
            // Set new x position
            transform.localPosition = new Vector3(xPosition, newYpos, 0F);

            // Invoke events
            OnLanded.Invoke();
            splineTrack.OnBoatEnter.Invoke(gameObject);
        }
    }

#endregion

#region Movement


    private void GeneralMovement()
    {
        // Apply steering
        steerSpeed = airSteerSpeed;
        if (isGrounded)
            steerSpeed = groundSteerSpeed;
        transform.position += transform.right * (steerInput * steerSpeed) * Time.deltaTime;
        // Apply dashing
        if (isDashing == true)
        {
            transform.position += transform.right * dashDirection * dashForce * dashTime * Time.deltaTime;
        }
    }


    private void GroundMovement()
    {
        timeSinceJump = 0f;
        // Reset how many jumps the boat has
        jumpsLeft = maxJumps;
        quickfallStarted = false;

        // Move boat forwards
        //Debug.Log(forwardInput);
        float forwardMovement = transform.localPosition.z + forwardInput * groundSteerSpeed * ( 1.2f - (MathF.Abs(transform.localPosition.z) / frontBackOffsetLimit)) * Time.deltaTime;
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
        transform.position += transform.forward * GetCurrentSpeed() * Time.deltaTime;

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


#endregion

#region Speed

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
        return currentForwardSpeed * GetForwardSpeedMultipliers();
    }


    public void SetForwardSpeedMultiplier(string name, float value, SpeedMultiplierCurve multiplierCurve = null)
    {
        // Create new Speed multiplier value
        SpeedMultiplier speedMultiplier = new();
        speedMultiplier.value = value;
        speedMultiplier.timeOnStart = Time.time;
        speedMultiplier.multiplierCurve = multiplierCurve;

        // Add value to dict
        forwardSpeedMultipliers[name.ToLower()] = speedMultiplier;
    }

    public SpeedMultiplier GetForwardSpeedMultiplier(string name)
    {
        if (forwardSpeedMultipliers.ContainsKey(name.ToLower()))
            return forwardSpeedMultipliers[name.ToLower()];
        else
            return null;
    }


    private float GetForwardSpeedMultipliers()
    {
        float totalSpeedMultiplier = 1f;
        for (int i = 0; i < forwardSpeedMultipliers.Count; i++)
        {
            var item = forwardSpeedMultipliers.ElementAt(i);
            string key = item.Key;
            SpeedMultiplier value = item.Value;

            totalSpeedMultiplier += value.GetMultiplierValue(Time.time);
            // Remove the value when it has reached the end
            if (value.shouldDelete == true)
                forwardSpeedMultipliers.Remove(key);
        }
        return totalSpeedMultiplier;
    }

#endregion


#region Actions

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

            // Get the position and distance the boat was at when jumping
            jumpPosition = transform.position;
            distanceWhenJumped = dollyKart.SplinePosition;

            // Stop the dollyKart from moving
            dollyKart.AutomaticDolly.Enabled = false;

            // Detach the boat from the dollyKart so that it has free movement in the air
            dollyKart.transform.DetachChildren();
            // Reset boat pitch so that it won't move upwards when jumping off a loop while going upwards
            transform.localEulerAngles = new(0f, transform.localEulerAngles.y, transform.localEulerAngles.z);

            // Reattach the boat to the main track when jumping of a rail
            if (currentTrack.IsGrindRail)
            {
                // Get the position relative to the main track
                TrackDistanceInfo distanceInfo = mainTrack.GetDistanceInfoFromPosition(jumpPosition);

                // Attach the dollyKart to the main track
                currentTrack = mainTrack;
                dollyKart.Spline = mainTrack.track;
                // Set the new positoin of the dollyKart
                dollyKart.SplinePosition = distanceInfo.distance;

                // Set the base speed
                SetSpeed(baseForwardSpeed);
            }

            // Disable collider for a bit after jumping
            StartCoroutine(DisableColliderBriefly());

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


private IEnumerator DisableColliderBriefly()
    {
        colliderReference.enabled = false;
        yield return new WaitForSeconds(colliderDisabledAfterJumpDuration);
        colliderReference.enabled = true;
    }

#endregion


#region Anim

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

#endregion

#region Respawn

[Header("Respawning")]
    [SerializeField] private float respawnLerpDuration = 1f;
    // How far bellow the track the boat has to be before respawning
    [SerializeField] private float deathYPosition = -25f;
    public UnityEvent OnRespawnStarted;
    public UnityEvent OnRespawnEnded;

    private bool isDead = false;
    private float respawnLerpTime;
    private Vector3 respawnLerpStart;


    private void StartRespawn()
    {
        // Start the respawn lerp
        isDead = true;
        respawnLerpTime = 0f;
        respawnLerpStart = transform.position;

        // Reset stuff
        ySpeed = 0f;
        isGrounded = true;
        wasLastTrackRail = false;
        // Reset speed when respawning
        if (currentTrack.overrideSpeed < baseForwardSpeed)
            currentForwardSpeed = baseForwardSpeed;

        // Invoke event
        OnRespawnStarted.Invoke();
    }


    private void RespawnLerp()
    {
        Vector3 from = respawnLerpStart;
        Vector3 to = dollyKart.transform.position;
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

            // Reattach the boat to the dollyKart
            transform.parent = dollyKart.transform;
            transform.localEulerAngles = Vector3.zero;

            OnRespawnEnded.Invoke();
        }
    }


#endregion
}


#region Speed multiplier classes
public class SpeedMultiplier
{
    public float value;
    public float timeOnStart;
    public SpeedMultiplierCurve multiplierCurve;

    [HideInInspector] public bool shouldDelete = false;


    public float GetMultiplierValue(float time)
    {
        if (multiplierCurve != null)
        {
            // How long the curve has been active for
            float activeTime = time - timeOnStart;

            // When the start curve ends
            float startCurveTime = multiplierCurve.startCurve.keys.Last().time;
            // When the hold time ends
            float endCurveTime = startCurveTime + multiplierCurve.holdTime;
            // When to remove the speed multiplier
            float deleteCurveTime = endCurveTime + multiplierCurve.endCurve.keys.Last().time;

            if (activeTime < startCurveTime)
            {
                return value * multiplierCurve.startCurve.Evaluate(activeTime);
            }
            else if (activeTime < endCurveTime && activeTime > startCurveTime)
            {
                return value;
            }
            else if (activeTime < deleteCurveTime && activeTime > endCurveTime)
            {
                return value * multiplierCurve.endCurve.Evaluate(Mathf.Abs(endCurveTime - activeTime));
            }
            else
                shouldDelete = true;
        }
        return value;
    }
}

[Serializable]
public class SpeedMultiplierCurve
{
    public float holdTime;
    public AnimationCurve startCurve;
    public AnimationCurve endCurve;
}

#endregion