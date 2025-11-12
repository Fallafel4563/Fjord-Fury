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

    [HideInInspector] public float forwardInput;
    [HideInInspector] public float steerInput;
    [HideInInspector] public bool isJumping = false;

    private bool isGrounded = true;
    private bool isDashing = false;
    private bool isDead = false;
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
    [SerializeField] private CinemachineCamera boatCamera;
    [SerializeField] private float respawnLerpDuration = 0.5f;
    // How far bellow the track the boat has to be before respawning
    [SerializeField] private float respawnYPosition = -25f;
    [SerializeField] private float respawnDelayDuration = 0.5f;

    private float respawnLerpTime;
    private bool isRespawnLerpActive = false;
    private Transform camDeathPos;



    private void Start()
    {
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
            boatCamera.transform.position = camDeathPos.position;
        }
        RespawnLerp();

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
        if (transform.localPosition.y < respawnYPosition && isDead == false)
        {
            isDead = true;
            isJumping = false;
            camDeathPos = boatCamera.transform;
            dollyKart.AutomaticDolly.Enabled = true;
            StartCoroutine(RespawnDelay());
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


    private void OnTriggerEnter(Collider other)
    {
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
                if (splineTrack.IsGrindRail == false)
                {
                    mainTrack = splineTrack;
                }
            }

            // Landing
            dashTime = 0f;
            isDashing = false;
            isGrounded = true;
            splineTrack.OnBoatEnter.Invoke();
            // Return camera back to it's default height when landing
            boatCamera.GetComponent<CinemachinePositionComposer>().TargetOffset.y = 4f;


            EvalInfo dupeditt = splineTrack.EvaluateBasedOnWorldPosition(transform.position);
            dollyKart.SplinePosition = dupeditt.distance;

            ySpeed = 0f;
            dollyKart.AutomaticDolly.Enabled = true;
            transform.parent = dollyKart.transform;

            // Set the landing position
            float rightPosition = Vector3.Distance(transform.position, dupeditt.SplinePos);
            float leftPosition = rightPosition * -1f;
            bool useLeftPos = transform.localPosition.x > 0;
            float newPosition = useLeftPos ? rightPosition : leftPosition;

            transform.localPosition = new Vector3(newPosition, 0f, 0f);
        }
    }


    public void Jump()
    {
        if (jumpsLeft > 0)
        {
            jumpsLeft -= 1;
            isDashing = false;
            isGrounded = false;
            timeSinceJump = 0f;

            // Move the boat upwards
            ySpeed = jumpPower;
            // Reduce the height of the camera when jumping
            boatCamera.GetComponent<CinemachinePositionComposer>().TargetOffset.y = 0f;

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
                EvalInfo dupeEditt = mainTrack.EvaluateBasedOnWorldPosition(globalPosition);
                Vector3 newPosition = globalPosition - dupeEditt.SplinePos;

                // Set the position of the dolly kart
                dollyKart.SplinePosition = dupeEditt.distance;

                // Set the position of the boat
                transform.localPosition = newPosition;
                SetSpeed(baseForwardSpeed);
            }
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


    private void RespawnLerp()
    {
        if (respawnLerpTime > 0)
        {
            Vector3 from = transform.localPosition;
            Vector3 to = new(isRespawnLerpActive ? 0.0f : transform.localPosition.x, 0.0f, 0.0f);
            float weight = 1 - respawnLerpTime;
            transform.localPosition = Vector3.Lerp(from, to, weight);
            respawnLerpTime -= Time.deltaTime;
        }
        else
        {
            isRespawnLerpActive = false;
            // Return camera back to it's default height when respawning
            boatCamera.GetComponent<CinemachinePositionComposer>().TargetOffset.y = 4f;
        }
    }


    private IEnumerator DisableColliderBriefly()
    {
        colliderReference.enabled = false;
        yield return new WaitForSeconds(colliderDisabledAfterJumpDuration);
        colliderReference.enabled = true;
    }


    private IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(respawnDelayDuration);
        isDead = false;
        isRespawnLerpActive = true;
        respawnLerpTime = respawnLerpDuration;
        isGrounded = true;
        ySpeed = 0f;
        transform.localEulerAngles = Vector3.zero;
    }
}
