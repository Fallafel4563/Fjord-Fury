using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;


public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController;
    public CinemachineSplineCart splineCart;
    public Transform circleRotParent;
    public ForwardSpeedMultiplier forwardSpeedMultiplier;

    // Input variables
    public bool jumpInput { get; set; }
    public bool driftInput { get; set; }
    public float forwardInput { get; set; }
    public float steerInput { get; set; }
    public bool dontChangeMainTrack { get; set; } = false;

    // State variables
    public bool isGrounded { get; set; } = true;
    public Vector3 oldPosition;


    private void Start()
    {
        currentTrack = mainTrack;
        // Set the players movement speed to be the tracks override speed
        SetOverrideSpeed(mainTrack.overrideSpeed);

        oldPosition = transform.position;
    }


    private void Update()
    {
        // Update the current forward speed
        currentForwardSpeed = overrideSpeed * forwardSpeedMultiplier.GetTotalMultiplierValue();

        // Get the current steer speed based on the ground state of the boat
        steerSpeed = isGrounded ? groundSteerSpeed : airSteerSpeed;

        if (isDrifting)
            ApplyDriftingMovement();
        else if (isGrounded && !isDrifting)
            ApplyGroundMovement();
        else
            ApplyAirMovement();

        oldPosition = transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SplineTrack splineTrack) && (!isGrounded || splineTrack != currentTrack) && !isDrifting)
        {
            // This fixes a null reference error when spawning the player (SplineCart reference isn't set the same frame the player spawns)
            // and the boat hits a track during that frame so we get a null reference error without this if statement
            if (splineCart)
                LandedOnTrack(splineTrack);
        }
    }



    #region General
    [Header("General")]
    public float baseForwardSpeed = 40f;

    public bool clampXAxis { get; set; } = true;
    public bool wasLastTrackRail { get; set; } = false;
    public float currentForwardSpeed { get; set; } = 40f;
    public float overrideSpeed { get; set; } = 40f;
    public float steerSpeed { get; set; }
    public Vector3 HorizontalVelocity { get; set; }
    public SplineTrack mainTrack { get; set; }
    public SplineTrack currentTrack { get; set; }


    // Seering that is applied when not on a circle track
    private void NonCicleSteering()
    {
        HorizontalVelocity = transform.right * steerInput * steerSpeed * Time.deltaTime;
        transform.position += HorizontalVelocity;
    }


    public void AttachToTrack(bool isTrackCircle)
    {
        if (isTrackCircle)
        {
            // Set circle rot parent as parent and reset boat
            transform.parent = circleRotParent.transform;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }
        else
        {
            // Set boat parent to spline cart and reset position
            transform.parent = splineCart.transform;
            transform.localEulerAngles = Vector3.zero;
        }
    }


    public void DetachFromCart()
    {
        if (isGrounded)
            // Set the air velocity when jumping. Also set the velocity forwads to avoid having the boat stop for a breif moment when jumping (But only when grounded)
            airVelocity = transform.forward * currentForwardSpeed;

        // Reset stuff
        isGrounded = false;
        timeSinceJump = 0f;
        jumpsLeft--;


        // Save position and distance when the boat jumped
        positionWhenJumped = transform.position;
        distanceWhenJumped = splineCart.SplinePosition;
        // Get the rotation the boat should have when in the air. The boat will lerp it's current rotation to this rotation when airborne
        // This is done to avoid having the boat "ignore" gravity if it's facing upwards when jumping (since it adds force in the direction the boat is facing when airborne)
        desiredAirRotation = GetRotationFromNewUpVector(Vector3.up);

        // Stop the splineCart
        splineCart.AutomaticDolly.Enabled = false;

        // Detach the boat form the cart so that it has free movement while in the air
        if (transform.parent = circleRotParent)
            circleRotParent.transform.DetachChildren();
        else
            splineCart.transform.DetachChildren();

        // Invoke events
        currentTrack.OnBoatExit.Invoke(gameObject);

        // Reattach the cart to the main track when jumping off a rail
        if (!currentTrack.shouldRespawnOnTrack)
        {
            // Get the position relative to the main track
            TrackDistanceInfo distanceInfo = mainTrack.GetDistanceInfoFromPosition(positionWhenJumped);

            // Attach the cart to the main track
            currentTrack = mainTrack;
            splineCart.Spline = mainTrack.track;
            // Set the new positoin of the cart
            splineCart.SplinePosition = distanceInfo.distance;
        }
        else
        {
            lastMainTrackDistance = distanceWhenJumped;
        }
    }


    public void SetOverrideSpeed(float newOverrideSpeed)
    {
        if (newOverrideSpeed > 0f)
            overrideSpeed = newOverrideSpeed;
        else
            overrideSpeed = baseForwardSpeed;
    }


    // CREDITS: Steego - https://discussions.unity.com/t/align-up-direction-with-normal-while-retaining-look-direction/852614/3
    private Quaternion GetRotationFromNewUpVector(Vector3 newUp)
    {
        bool areParallel = Mathf.Approximately(Mathf.Abs(Vector3.Dot(transform.forward, newUp)), 1f);
        Vector3 newForward = areParallel ? newUp : Vector3.ProjectOnPlane(transform.forward, newUp).normalized;
        return Quaternion.LookRotation(newForward, newUp);
    }


    #endregion



    #region Grounded
    [Header("Grounded")]
    public float groundSteerSpeed = 15f;
    public float circleTrackSteerSpeed = 7.5f;
    public float frontBackOffsetLimit = 3f;



    private void ApplyGroundMovement()
    {
        GroundResetStuff();

        // Update the cart speed
        UpdateCartSpeed();

        if (currentTrack.isCircle)
            CircleTrackMovement();
        else
            RoadTrackMovement();
    }


    // Movement on a spline track that is a raod
    private void RoadTrackMovement()
    {
        NonCicleSteering();

        // Move boat forwards
        float forwardPosLimit = 1.2f - (MathF.Abs(transform.localPosition.z) / frontBackOffsetLimit);
        float forwardMovement = transform.localPosition.z + forwardInput * groundSteerSpeed * forwardPosLimit * Time.deltaTime;
        // Limit how far forward/backwards the boat can travel
        //float forwardLimit = Mathf.Clamp(forwardMovement, -frontBackOffsetLimit, frontBackOffsetLimit);
        // Apply forwards movement
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, forwardMovement);


        // Stop the boat from going off the sides of the track
        // Divided by 2 since the end of the width is only half of the width
        if (Mathf.Abs(transform.localPosition.x) > (currentTrack.width / 2f) && clampXAxis)
        {
            float sidewaysPos = Mathf.Sign(transform.localPosition.x) * (currentTrack.width / 2.0f);
            // Apply sideways limit
            transform.localPosition = new Vector3(sidewaysPos, transform.localPosition.y, transform.localPosition.z);
        }


        // Fall off track when reaching the end
        // Get how long the track is
        float trackLength = currentTrack.track.Spline.GetLength();
        // Get position where the boat should jump off track
        float jumpOffDistance = trackLength - 1f;
        // Jump off track when within the jump off distance
        if (splineCart.SplinePosition > jumpOffDistance && !currentTrack.track.Splines[0].Closed)
        {
            if (currentTrack.jumpOffAtEnd)
                Jump();
            else
                // Detach the boat form the spline cart
                DetachFromCart();

            currentTrack.OnBoatReachedEnd.Invoke(gameObject);
        }
    }



    // Movement on a spline track that is a cirlce
    private void CircleTrackMovement()
    {
        // Set position of the boat to be the width of the track
        transform.localPosition = new(transform.localPosition.z, currentTrack.width, transform.localPosition.z);
        // Get the desired rotation
        Quaternion desiredRot = Quaternion.Euler(circleRotParent.eulerAngles.x, circleRotParent.eulerAngles.y, circleRotParent.eulerAngles.z + steerInput * -1f * 25f);
        // Change rotation
        circleRotParent.rotation = Quaternion.Lerp(circleRotParent.rotation, desiredRot, circleTrackSteerSpeed * Time.deltaTime);

        // Fall off track when reaching the end
        // Get how long the track is
        float trackLength = currentTrack.track.Spline.GetLength();
        // Get position where the boat should jump off track
        float jumpOffDistance = trackLength - 1f;
        // Jump off track when within the jump off distance
        if (splineCart.SplinePosition > jumpOffDistance && !currentTrack.track.Splines[0].Closed)
        {
            if (currentTrack.jumpOffAtEnd)
                Jump();
            else
                // Detach the boat form the spline cart
                DetachFromCart();

            currentTrack.OnBoatReachedEnd.Invoke(gameObject);
        }
    }


    private void UpdateCartSpeed()
    {
        if (splineCart.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
            autoDolly.Speed = currentForwardSpeed;
    }


    private void GroundResetStuff()
    {
        airVelocity = Vector3.zero;
        canGroundPound = true;
        startedGroundPound = false;
        ResetJumping();
    }


    #endregion



    #region Airborne
    [Header("Airborne")]
    public float airSteerSpeed = 10f;
    public float fallSpeed = 50f;
    public float quickfallSpeed = 75f;
    // How it should rotate when steering in the air
    public float airSteerRotSpeed = 0.5f;
    public float resetAirRotationSpeed = 10f;

    public Vector3 airVelocity { get; set; } = Vector3.zero;

    //NOTE: This can also be used to set the rotation of the boat when drifting. Just remember to change the name
    private Quaternion desiredAirRotation;



    private void ApplyAirMovement()
    {
        NonCicleSteering();

        // Move boat forwards
        // Get how fast the boat is moving forwards
        float forwardVel = Vector3.Dot(airVelocity, transform.forward);
        // Only apply forwards movement if the boat is going slower than currentForwardSpeed
        if (forwardVel < currentForwardSpeed)
            airVelocity += transform.forward * currentForwardSpeed * Time.deltaTime;

        // Get gravity
        float gravity = fallSpeed + quickfallSpeed;
        // Add groundpound fall speed when starting ground pound
        if (startedGroundPound)
            gravity += groundPoundFallSpeed;

        // Apply gravity
        airVelocity += Vector3.down * gravity * Mathf.Pow(timeSinceJump + 0.5f, 2f) * Time.deltaTime;

        // Apply air velocity
        transform.position += airVelocity * Time.deltaTime;

        // Rotate boat when steering
        desiredAirRotation *= Quaternion.AngleAxis(steerInput * airSteerRotSpeed * Time.deltaTime, transform.up);
        // Lerp the rotation that was set when jumping (also when falling off the track)
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredAirRotation, resetAirRotationSpeed * Time.deltaTime);
    }

    #endregion



    #region Jumping
    [Header("Jumping")]
    public int maxJumps = 2;
    public float jumpPower = 15f;
    public UnityEvent Jumped;
    public UnityEvent DoubleJumped;

    public int jumpsLeft { get; private set; }
    public float timeSinceJump { get; set; }
    public float distanceWhenJumped { get; set; }
    public float lastMainTrackDistance { get; set; }
    public Vector3 positionWhenJumped { get; set; }

    public void ShroomBounce(float bouncePower)
    {
        // Detach the boat form the spline cart
        DetachFromCart();

        // Stop all upwards velocity
        float upwardsVel = Vector3.Dot(airVelocity, transform.up);
        airVelocity -= transform.up * upwardsVel;
        // Set the upwards air velocity to be the equal to jump power
        // Set the air velocity when jumping. Also set the velocity forwads to avoid having the boat stop for a breif moment when jumping
        airVelocity += transform.up * bouncePower;


        // Invoke events
        Jumped.Invoke();
    }

    public void Jump()
    {

        if (isGrounded || isDrifting || jumpsLeft > 0 && !startedGroundPound)
        {
            if (!isGrounded && !isDrifting)
            {
                Debug.Log("Double jump");
                DoubleJumped.Invoke();
            }

            if (isDrifting)
                EndDrift();
            
            // Detach the boat form the spline cart
            DetachFromCart();

            // Stop all upwards velocity
            float upwardsVel = Vector3.Dot(airVelocity, transform.up);
            airVelocity -= transform.up * upwardsVel;
            // Set the upwards air velocity to be the equal to jump power
            // Set the air velocity when jumping. Also set the velocity forwads to avoid having the boat stop for a breif moment when jumping
            airVelocity += transform.up * jumpPower;

            // Invoke events
            Jumped.Invoke();
        }
    }


    private void ResetJumping()
    {
        timeSinceJump = 0f;
        jumpsLeft = maxJumps;
    }

#endregion



#region Landing
    [Header("Landing")]
    public UnityEvent Landed;



    public void LandedOnTrack(SplineTrack splineTrack)
    {
        // Don't change main track when it's inside a DontChangeMainTrack trigger
        // Can still change to rails
        if (dontChangeMainTrack && splineTrack != mainTrack && splineTrack.shouldRespawnOnTrack)
        {
            return;
        }

        TrackDistanceInfo distanceInfo = splineTrack.GetDistanceInfoFromPosition(transform.position);

        // Reset  stuff
        isGrounded = true;

        // Update current and main track
        currentTrack = splineTrack;
        splineCart.Spline = currentTrack.track;
        // Update main track if the new track isn't a rail track
        if (splineTrack.shouldRespawnOnTrack)
        {
            mainTrack = splineTrack;
            wasLastTrackRail = false;
        }
        else
            wasLastTrackRail = true;

        // Set the carts new position
        splineCart.SplinePosition = distanceInfo.distance;
        // Renable the carts movement
        splineCart.AutomaticDolly.Enabled = true;


        // Set the override speed if the boat lands on a fast track
        SetOverrideSpeed(splineTrack.overrideSpeed);

        // Change landing logic based on if the track is a circle or not
        if (splineTrack.isCircle)
            LandOnCircleTrack(distanceInfo);
        else
            LandOnRoadTrack(distanceInfo);


        // Invoke events
        Landed.Invoke();
        splineTrack.OnBoatEnter.Invoke(gameObject);
    }



    private void LandOnRoadTrack(TrackDistanceInfo distanceInfo)
    {
        AttachToTrack(currentTrack.isCircle);

        // Get how far the boat is in the x position (but we don't know if it's to the left or right)
        float xPosition = Vector3.Distance(transform.position, distanceInfo.nearestSplinePos);

        // Check if the boat landed on the right or left side
        const float DEBUG_DRAW_DURATION = 60f;
        Vector3 splinePosToPlayerDir = transform.position - distanceInfo.nearestSplinePos;
        Debug.DrawLine(transform.position, distanceInfo.nearestSplinePos, Color.red, DEBUG_DRAW_DURATION);

        Vector3 splineUpwardsDirection = currentTrack.track.EvaluateUpVector(distanceInfo.normalizedDistance);
        Debug.DrawLine(distanceInfo.nearestSplinePos, distanceInfo.nearestSplinePos + splineUpwardsDirection * 10f, Color.green, DEBUG_DRAW_DURATION);

        Vector3 sideCross = Vector3.Cross(splineUpwardsDirection, splinePosToPlayerDir);
        Debug.DrawLine(transform.position, transform.position + sideCross, Color.yellow, DEBUG_DRAW_DURATION);

        Vector3 splineTagent = currentTrack.track.EvaluateTangent(distanceInfo.normalizedDistance);
        Debug.DrawLine(distanceInfo.nearestSplinePos, distanceInfo.nearestSplinePos + splineTagent.normalized * 10f, Color.magenta, DEBUG_DRAW_DURATION);

        // Compare side corss to spline tangent to see which side the player landed on
        bool landedOnTheLeftSide = Vector3.Dot(sideCross, splineTagent.normalized) > 0;
        if (landedOnTheLeftSide)
            xPosition *= -1f;
        
        // Set new boat position
        transform.localPosition = new Vector3(xPosition, 0f, 0F);
        //Debug.Break();
    }


    private void LandOnCircleTrack(TrackDistanceInfo distanceInfo)
    {
        if (isDrifting)
            EndDrift();
        // Reattach the circle rot to the SplineCart
        circleRotParent.parent = splineCart.transform;
        circleRotParent.localPosition = Vector3.zero;

        Vector3 dirToTrack = transform.position - distanceInfo.nearestSplinePos;
        // Set the rotation of the circle rot parent to match where the boat is landing
        float desiredAngle = Vector3.SignedAngle(Vector3.up, dirToTrack, splineCart.transform.forward);
        circleRotParent.eulerAngles = new(circleRotParent.eulerAngles.x, circleRotParent.eulerAngles.y, desiredAngle);

        AttachToTrack(currentTrack.isCircle);
    }
    #endregion


    #region Drift
    [Header("Drift")]
    public float minDriftBoostTime = 0.5f;
    public float diftSlowdownMultipler = 0.8f;
    public float driftRotationMultipler = 0.3f;
    public float driftMaxRotation = 30f;
    public float dirftMinRotation = 10f;
    public float sidewaysDriftForce = 20f;
    // TODO: Dirft start trail
    // TODO: Drift boost trail
    // TODO: Super boost trail
    public LayerMask driftLayter;
    public AnimationCurve releaseBoostCurve;
    public SpeedMultiplierCurve driftReleaseMultiplerCurve;
    public bool isDrifting { get; private set; } = false;
    public bool driftRayHittingGround { get; private set; } = false;

    private float driftTimePassed;
    private float boostTimePassed;
    private float currentRotation;
    private float rotationOffset;
    private int driftDirection;


    [Header("Groundpound")]
    public float groundPoundFallSpeed = 50f;
    public bool canGroundPound { get; private set; } = true;
    public bool startedGroundPound { get; private set; } = false;

    public UnityEvent GroundpoundStarted;

    public void StartDrift()
    {
        if (isGrounded && Mathf.Abs(steerInput) > 0f && !currentTrack.isCircle)
        {
            InitiateDrift();
        }
        else if (canGroundPound)
        {
            canGroundPound = false;
            startedGroundPound = true;
            GroundpoundStarted.Invoke();
        }
    }


    private void InitiateDrift()
    {
        isDrifting = true;
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drifting", diftSlowdownMultipler);

        driftTimePassed = 0f;
        boostTimePassed = 0f;
        driftDirection = (int)Mathf.Sign(steerInput);
        currentRotation = transform.localEulerAngles.y;

        DetachFromCart();
        // TODO: Enable drift start trail
    }


    private void ApplyDriftingMovement()
    {
        RaycastHit raycastHit;
        Debug.DrawLine(transform.position + transform.up * 2f, transform.position + -transform.up * 5f, Color.black, 60f);
        if (Physics.Raycast(transform.position + transform.up * 2f, -transform.up, out raycastHit, 5f, driftLayter))
        {
            driftRayHittingGround = true;
            
            float lerpTarget = dirftMinRotation + driftMaxRotation / 2f;
            if (Mathf.Approximately(steerInput, driftDirection))
            {
                lerpTarget = driftMaxRotation;
                Debug.Log("Max rotation");
            }
            else if (Mathf.Approximately(steerInput, driftDirection * -1f))
            {
                Debug.Log("Min rotation");
                lerpTarget = dirftMinRotation;
            }

            rotationOffset = Mathf.LerpAngle(rotationOffset, lerpTarget, 5f * Time.deltaTime);

            // TODO: Rotate hodel holder

            currentRotation = Mathf.LerpAngle(currentRotation, (lerpTarget / 5f) * driftDirection, 5f * Time.deltaTime);

            // Align the boats rotation with the spline normal
            // Get the "normal" of the spline. (Using the splines up vector is smoother than the meshes normal)
            TrackDistanceInfo distanceInfo = currentTrack.GetDistanceInfoFromPosition(transform.position);
            Vector3 splineNormal = currentTrack.track.Spline.EvaluateUpVector(distanceInfo.normalizedDistance);
            desiredAirRotation = GetRotationFromNewUpVector(splineNormal);
            desiredAirRotation *= Quaternion.AngleAxis(currentRotation, splineNormal);
            // Apply rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredAirRotation, 15f * Time.deltaTime);

            // Move boat forwards
            Vector3 forwardMovement = transform.forward * currentForwardSpeed * Time.deltaTime;
            Vector3 sidewaysMovement = -transform.right * driftDirection * sidewaysDriftForce * Time.deltaTime;
            transform.position = raycastHit.point + forwardMovement + sidewaysMovement;

            driftTimePassed += Time.deltaTime;
            if (driftTimePassed > minDriftBoostTime)
            {
                // TODO: Enable boost trail
                // TODO: Disable drift start trail
                boostTimePassed += Time.deltaTime;
            }
        }
        else
        {
            driftRayHittingGround = false;
            EndDrift();
        }
    }


    public void EndDrift()
    {
        TrackDistanceInfo distanceInfo = currentTrack.GetDistanceInfoFromPosition(transform.position);
        positionWhenJumped = transform.position;
        distanceWhenJumped = distanceInfo.distance;
        splineCart.SplinePosition = distanceInfo.distance;

        float trackDirMult = 1f;

        isDrifting = false;
        // Land on track when still "on" a track
        if (driftRayHittingGround && !jumpInput)
        {
            // Scale drift based on how much the player is facing the splines forward direction
            Vector3 trackForward = currentTrack.track.EvaluateTangent(distanceInfo.normalizedDistance);
            trackDirMult = Vector3.Dot(transform.forward, trackForward.normalized);
            trackDirMult = Mathf.Clamp(trackDirMult, 0f, 1f);
            LandedOnTrack(currentTrack);
        }
        else
        {
            desiredAirRotation = GetRotationFromNewUpVector(Vector3.up);
            airVelocity = transform.forward * currentForwardSpeed;
        }

        StopAllCoroutines();
        EndDriftBoost();
        if (minDriftBoostTime < driftTimePassed)
        {
            StartCoroutine(ApplyDriftBoost(trackDirMult));
        }
    }


    private void EndDriftBoost()
    {
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drifting", 1f);
        
        // TODO: Disable boost trail
        // TODO: Disable drift start trail
    }


    private IEnumerator ApplyDriftBoost(float trackDirMult)
    {
        // TODO: Enable super boost trail
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drift Release Boost", 1f + (releaseBoostCurve.Evaluate(boostTimePassed) * trackDirMult), driftReleaseMultiplerCurve);
        yield return new WaitForSeconds(driftReleaseMultiplerCurve.GetLength());
        // TODO: Disable super boost trail
    }


#endregion
}
