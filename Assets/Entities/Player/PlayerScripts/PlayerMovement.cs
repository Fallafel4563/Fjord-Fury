using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


public class PlayerMovement : MonoBehaviour
{
    public Transform circleRotParent;
    public ForwardSpeedMultiplier forwardSpeedMultiplier;

    // Input variables
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public bool driftInput;
    [HideInInspector] public float forwardInput;
    [HideInInspector] public float steerInput;
    [HideInInspector] public bool dontChangeMainTrack = false;

    // State variables
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public bool crashing = false;


    private void Start()
    {
        currentTrack = mainTrack;
        // Set the players movement speed to be the tracks override speed
        SetOverrideSpeed(mainTrack.overrideSpeed);
    }


    private void Update()
    {
        // Update the current forward speed

        float crashMult = 1f;
        if (forwardSpeedMultiplier.GetForwardSpeedMultiplier("HitObstacle") != null)
            crashMult = 0.5f;

        currentForwardSpeed = overrideSpeed * forwardSpeedMultiplier.GetTotalMultiplierValue() * crashMult;

        // Get the current steer speed based on the ground state of the boat
        steerSpeed = isGrounded ? groundSteerSpeed : airSteerSpeed;

        if (isGrounded)
            ApplyGroundMovement();
        else
            ApplyAirMovement();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SplineTrack splineTrack) && (!isGrounded || splineTrack != currentTrack))
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

    [HideInInspector] public bool wasLastTrackRail = false;
    [HideInInspector] public float currentForwardSpeed = 40f;
    [HideInInspector] public float overrideSpeed = 40f;
    [HideInInspector] public float steerSpeed;
    [HideInInspector] public Vector3 HorizontalVelocity;
    [HideInInspector]public CinemachineSplineCart splineCart;
    [HideInInspector] public SplineTrack mainTrack;
    [HideInInspector] public SplineTrack currentTrack;


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


        // Save position and distance when the boat jumped
        positionWhenJumped = transform.position;
        distanceWhenJumped = splineCart.SplinePosition;
        // Get the rotation the boat should have when in the air. The boat will lerp it's current rotation to this rotation when airborne
        // This is done to avoid having the boat "ignore" gravity if it's facing upwards when jumping (since it adds force in the direction the boat is facing when airborne)
        // CREDITS: Steego - https://discussions.unity.com/t/align-up-direction-with-normal-while-retaining-look-direction/852614/3
        bool areParallel = Mathf.Approximately(Mathf.Abs(Vector3.Dot(transform.forward, Vector3.up)), 1f);
        Vector3 newForward = areParallel ? Vector3.up : Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        desiredAirRotation = Quaternion.LookRotation(newForward, Vector3.up);

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
    }


    public void SetOverrideSpeed(float newOverrideSpeed)
    {
        if (newOverrideSpeed > 0f)
            overrideSpeed = newOverrideSpeed;
        else
            overrideSpeed = baseForwardSpeed;
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
        if (Mathf.Abs(transform.localPosition.x) > (currentTrack.width / 2f))
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

    [HideInInspector] public Vector3 airVelocity = Vector3.zero;

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
        float gravity = fallSpeed;
        // Add quickfall speed if the player is no longer holding the jump key
        if (!jumpInput && !isJumping)
        {
            isJumping = false;
            gravity = fallSpeed + quickfallSpeed;
        }
        // Apply gravity
        airVelocity += Vector3.down * gravity * Mathf.Pow(timeSinceJump + 0.5f, 2f) * Time.deltaTime;

        // Apply air velocity
        transform.position += airVelocity * Time.deltaTime;

        // Rotate boat when steering
        desiredAirRotation *= Quaternion.AngleAxis(steerInput * airSteerRotSpeed * Time.deltaTime, transform.up);
        // Lerp the rotation that was set when jumping (also when falling off the track)
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredAirRotation, 5f * Time.deltaTime);
    }

#endregion



#region Jumping
    [Header("Jumping")]
    public float jumpPower = 15f;
    public UnityEvent Jumped;

    [HideInInspector] public float timeSinceJump;
    [HideInInspector] public float distanceWhenJumped;
    [HideInInspector] Vector3 positionWhenJumped;


    public void Jump()
    {
        if (isGrounded)
        {
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
        isJumping = false;
        timeSinceJump = 0f;
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
        Vector3 directionToPlayer = transform.position - distanceInfo.nearestSplinePos;
        float rightDirDot = Vector3.Dot(transform.right, directionToPlayer);
        // If rightDirDot is greater than 0 then the boat landed on the right side of the track
        if (rightDirDot < 0)
            xPosition *= -1f;
        // Set new boat position
        transform.localPosition = new Vector3(xPosition, 0f, 0F);
    }


    private void LandOnCircleTrack(TrackDistanceInfo distanceInfo)
    {
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


    #region Drifting

    [Header("Drifting")]
    public float driftSpeed = 0f;

    private void StartDrift()
    {
        //
    }


#endregion
}
