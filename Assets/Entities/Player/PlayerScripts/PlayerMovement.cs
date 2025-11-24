using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;


public class PlayerMovement : MonoBehaviour
{
    // Input variables
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public float forwardInput;
    [HideInInspector] public float steerInput;
    [HideInInspector] public bool dontChangeMainTrack = false;

    // State variables
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public bool isDashing = false;



    private void Start()
    {
        currentTrack = mainTrack;
        overrideSpeed = baseForwardSpeed;
    }


    private void Update()
    {
        ApplyGeneralMovement();

        if (isGrounded)
            ApplyGroundMovement();
        else
            ApplyAirMovement();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SplineTrack splineTrack) && (!isGrounded || splineTrack != currentTrack))
        {
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
    [HideInInspector]public CinemachineSplineCart splineCart;
    [HideInInspector] public SplineTrack mainTrack;
    [HideInInspector] public SplineTrack currentTrack;



    private void ApplyGeneralMovement()
    {
        // Get the current steer speed based on the ground state of the boat
        steerSpeed = isGrounded ? airSteerSpeed : groundSteerSpeed;

        // Apply steering
        if (isDashing)
            // Apply dashing steering if dashing
            transform.position += transform.right * dashDirection * dashForce * dashTime * Time.deltaTime;
        else
            // Apply normal steering if not dashing
            transform.position += transform.right * steerInput * steerSpeed * Time.deltaTime;
    }


    public void AttachToCart()
    {
        transform.parent = splineCart.transform;
        // Reset position
        transform.localEulerAngles = Vector3.zero;
    }


    public void DetachFromCart()
    {
        // Stop the cart
        splineCart.AutomaticDolly.Enabled = false;

        // Detach the boat form the cart so that it has free movement while in the air
        splineCart.transform.DetachChildren();

        // Reset boat pitch so that it moves properly while in the air
        transform.localEulerAngles = new(0f, transform.localEulerAngles.y, transform.localEulerAngles.z);

        // Invoke events
        Jumped.Invoke();
        currentTrack.OnBoatExit.Invoke(gameObject);

        // Reattach the cart to the main track when jumping off a rail
        if (currentTrack.IsGrindRail)
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


    public void SetOverrideSpeed(float newOverRideSpeed)
    {
        if (newOverRideSpeed > 0f)
            overrideSpeed = newOverRideSpeed;
        else
            overrideSpeed = baseForwardSpeed;
    }


#endregion



#region Grounded
    [Header("Grounded")]
    public float groundSteerSpeed = 15f;
    public float frontBackOffsetLimit = 3f;



    private void ApplyGroundMovement()
    {
        GroundResetStuff();

        // Update the cart speed
        UpdateCartSpeed();


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


        // Jump off the track when there's only 10m left of the track
        // Get how long the track is
        float trackLength = currentTrack.track.Spline.GetLength();
        // Get position where the boat should jump off track
        float jumpOffDistance = trackLength - 10f;
        // Jump off track when within the jump off distance
        if (splineCart.SplinePosition > jumpOffDistance)
        {
            currentTrack.OnBoatReachedEnd.Invoke(gameObject);
            Jump();
        }
    }


    private void UpdateCartSpeed()
    {
        if (splineCart.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
            autoDolly.Speed = currentForwardSpeed;
    }


    private void GroundResetStuff()
    {
        ySpeed = 0f;
        ResetJumping();
        // Only reset dashing when grounded
        DashCooldown();
    }


#endregion



#region Airborne
    [Header("Airborne")]
    public float airSteerSpeed = 10f;
    public float fallSpeed = 50f;
    public float quickfallSpeed = 75f;

    [HideInInspector] public float ySpeed;



    private void ApplyAirMovement()
    {
        // Move boat forwards
        transform.position += transform.forward * currentForwardSpeed * Time.deltaTime;

        // Get gravity
        float gravity = fallSpeed;
        // Add quickfall speed if no longer jumping
        if (!jumpInput && !isJumping)
        {
            isJumping = false;
            gravity = fallSpeed + quickfallSpeed;
        }

        // Apply gravity
        ySpeed -= gravity * Time.deltaTime * MathF.Pow(timeSinceJump + 0.5f, 2f);
        transform.localPosition += Vector3.up * ySpeed * Time.deltaTime;
    }

#endregion



#region Jumping
    [Header("Jumping")]
    public int maxJumps = 1;
    public float jumpPower = 15f;
    public UnityEvent Jumped;

    [HideInInspector] public float timeSinceJump;

    private int jumpsLeft;
    private float distanceWhenJumped;
    private Vector3 positionWhenJumped;



    public void Jump()
    {
        if (jumpsLeft > 0)
        {
            // Reduce how many jumps the boat has left
            jumpsLeft--;

            // Reset stuff
            isGrounded = false;
            isDashing = false;
            timeSinceJump = 0f;

            // Move boat upwards
            ySpeed = jumpPower;

            // Save position and distance when the boat jumped
            positionWhenJumped = transform.position;
            distanceWhenJumped = splineCart.SplinePosition;

            // Detach the boat form the spline cart
            DetachFromCart();
        }
    }


    private void ResetJumping()
    {
        isJumping = false;
        timeSinceJump = 0f;
        jumpsLeft = maxJumps;
    }

#endregion



#region Landing
    [Header("Landing")]
    public UnityEvent Landed;



    private void LandedOnTrack(SplineTrack splineTrack)
    {
        // Don't change main track when it's inside a DontChangeMainTrack trigger
        // Can still change to rails
        if (dontChangeMainTrack && splineTrack != mainTrack && !splineTrack.IsGrindRail)
        {
            return;
        }

        TrackDistanceInfo distanceInfo = splineTrack.GetDistanceInfoFromPosition(transform.position);

         // Stop the player from jumping to a part of the track that is too far ahead (On whirlpool for example)
        // Check if the track it lands on is the same as the current main track and that the the boat didn't jump off a rail
        if (splineTrack == mainTrack && wasLastTrackRail == false)
        {
            // Check how far it has travled while jumping (normal jump distance is around 75 (with a gravity of 75 and quickfall speed of 50))
            // If it's above 200 then the player has found a shortcut that we don't want
            Debug.LogFormat("Landed distance: {0}, Jump distance: {1}", distanceInfo.distance, distanceWhenJumped);
            if (Mathf.Abs(distanceInfo.distance - distanceWhenJumped) > 200f)
            {
                // Get the distance it has jumped
                float jumpedDistance = Vector3.Distance(positionWhenJumped, transform.position);
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


        // Reset  stuff
        dashTime = 0f;
        isGrounded = true;


        // Update current and main track
        currentTrack = splineTrack;
        splineCart.Spline = currentTrack.track;
        // Update main track if the new track isn't a rail track
        if (!splineTrack.IsGrindRail)
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


        // Reattach the boat to the track
        AttachToCart();
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


        // Invoke events
        Landed.Invoke();
        splineTrack.OnBoatEnter.Invoke(gameObject);
    }

#endregion



#region Dashing
    [Header("Dashing")]
    public float dashForce = 100f;
    public float dashDuration = 0.3f;
    public float dashDirection = 0f;
    public UnityEvent DashedLeft;
    public UnityEvent DashedRight;

    [HideInInspector] public float dashTime;



    public void DashLeft()
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashDirection = -1f;
            DashedLeft.Invoke();
        }
    }


    public void DashRight()
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashDirection = 1f;
            DashedRight.Invoke();
        }
    }


    private void DashCooldown()
    {
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

#endregion
}
