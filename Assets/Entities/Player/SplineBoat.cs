using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SplineBoat : MonoBehaviour
{
    [SerializeField] private CinemachineCamera boat_camera;
    [SerializeField] private CinemachineSplineCart dollyKart;
    [SerializeField] private SplineTrack mainTrack;
    [SerializeField] private SplineTrack currentTrack;
    [SerializeField] private Transform modelHolder;
    [SerializeField] private Collider colliderReference;

    [SerializeField] private float baseForwardSpeed = 40f;
    [SerializeField] private float frontBackOffsetLimit = 3f;
    [SerializeField] private float steerSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravityForce = 20f;
    [SerializeField] private float quickFallGravity = 40f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashForce = 100f;
    [SerializeField] private float respawnLerpDuration = 0.5f;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isDead = false;
    private bool isRespawnLerpActive = false;
    private float forwardInput;
    private float steerInput;
    private float dashDirection;
    private float dashTime;
    private float verticalSpeed;
    private float respawnLerpTime;
    private float timeSinceJump;
    private Transform camDeathPos;

    public UnityEvent OnLeftDashed;
    public UnityEvent OnRightDashed;


    private void Start()
    {
        GetMainTrack();
        GetBoatCamera();
        SetSpeed(baseForwardSpeed);
    }


    private void Update()
    {
        // Lerp to death cam position
        if (isDead == true)
        {
            boat_camera.transform.position = camDeathPos.position;
        }
        RespawnLerp();

        // Apply Steering
        transform.localPosition += Vector3.right * steerInput * steerSpeed * Time.deltaTime;
        // Apply dashing
        if (isDashing == true)
            transform.localPosition += Vector3.right * dashDirection * dashForce * dashTime * Time.deltaTime;
        

        if (isGrounded == true)
        {
            // Ground Movement
            // How much the boat should move forward every frame
            float forward_force = transform.localPosition.z + forwardInput * steerSpeed * (1.1f - Mathf.Abs(transform.localPosition.z) / frontBackOffsetLimit) * Time.deltaTime;
            // Limit how far the boat can move forward / backwards
            float forward_limt = Mathf.Clamp(forward_force, -frontBackOffsetLimit, frontBackOffsetLimit);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, forward_limt);

            // Reset time since jump (Why 0.2? I don't know)
            timeSinceJump = 0.2f;
            
            // Stop the boat from going off the track
            // Divided by 2 since the end of the width is only half of the width
            if (Mathf.Abs(transform.localPosition.x) > (currentTrack.width / 2f))
            {
                float sideways_limit = (currentTrack.width / 2.0f) * Mathf.Sign(transform.localPosition.x);
                transform.localPosition = new Vector3(sideways_limit, transform.localPosition.y, transform.localPosition.z);
            }

            // Do something when the boat reaches the end of the track
            if (dollyKart.SplinePosition > 0.99f * currentTrack.track.Spline.GetLength())
            {
                // Jump off the track if it's a rail
                if (currentTrack.IsGrindRail)
                {
                    Jump();
                }
                //currentTrack.OnEnd.Invoke();
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
        else // Airborne
        {
            timeSinceJump += Time.deltaTime;
            
            // Forward movement
            transform.localPosition += Vector3.forward * baseForwardSpeed * Time.deltaTime;
            // Apply vertical movement (Jumping and gravity)
            transform.localPosition += Vector3.up * verticalSpeed * Time.deltaTime;
            // Reduce vertical speed by gravity force
            verticalSpeed -= gravityForce * (timeSinceJump * timeSinceJump) * Time.deltaTime;

            // Apply quickfalling when not holding jump key
            if (isJumping == false)
                verticalSpeed -= quickFallGravity * (timeSinceJump * timeSinceJump) * Time.deltaTime;
            
            // Stop jumping when holding the jump key and when the boat is falling
            if (isJumping == true && verticalSpeed < 0f)
                isJumping = false;
            
            // Start respawn after falling off the track
            if (verticalSpeed < 0 && transform.localPosition.y < -25f && isDead == false)
            {
                isDead = true;
                camDeathPos = boat_camera.transform;

                isJumping = false;
                dollyKart.AutomaticDolly.Enabled = true;

                const float RESPAWN_DEALY_DURATION = 0.5f;
                StartCoroutine(RespawnDelay(RESPAWN_DEALY_DURATION));
            }
        }

        ModelAnim();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SplineTrack splineTrack) && (isGrounded == false || splineTrack != currentTrack))
        {
            // Change track when landing on a rail
            if (currentTrack != splineTrack)
            {
                // Change the speed
                SetSpeed(splineTrack.overrideSpeed);
                // Set the new track
                dollyKart.Spline = splineTrack.track;
                currentTrack = splineTrack;
                if (splineTrack.IsGrindRail == false)
                {
                    mainTrack = splineTrack;
                }
            }

            // Landing
            dashTime = 0.0f;
            isDashing = false;
            isGrounded = true;
            splineTrack.OnBoatEnter.Invoke();

            EvalInfo dupeditt = splineTrack.EvaluateBasedOnWorldPosition(transform.position);
            dollyKart.SplinePosition = dupeditt.distance;

            verticalSpeed = 0.0f;
            dollyKart.AutomaticDolly.Enabled = true;
            transform.parent = dollyKart.transform;

            float leftPosition = Vector3.Distance(transform.position, dupeditt.SplinePos) * -1f;
            float rightPosition = Vector3.Distance(transform.position, dupeditt.SplinePos);
            bool useRightPos = Vector3.Distance(dupeditt.SplinePos + leftPosition * transform.right, transform.position) > Vector3.Distance(dupeditt.SplinePos + rightPosition * transform.right, transform.position);
            float newPosition = useRightPos ? rightPosition : leftPosition;

            transform.localPosition = new Vector3(newPosition, 0f, 0f);
        }
    }


    // Gets the main track
    private void GetMainTrack()
    {
        //
    }


    // Gets the boat camera
    private void GetBoatCamera()
    {
        //
    }


    // Set the new speed of the dolly kart
    private void SetSpeed(float newSpeed)
    {
        if (dollyKart.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
        {
            autoDolly.Speed = newSpeed;
        }
    }


    private void Jump()
    {
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

        isJumping = true;
        isDashing = false;
        isGrounded = false;
        timeSinceJump = 0.2f;
        dollyKart.AutomaticDolly.Enabled = false;
        verticalSpeed = jumpForce;
        // Briefly disable the collider when jumping
        const float DISABLE_COLLIDER_DURATION = 0.15f;
        StartCoroutine(DisableColliderBriefly(DISABLE_COLLIDER_DURATION));
    }


    private void ModelAnim()
    {
        Vector3 newRotation = transform.localEulerAngles;

        newRotation.x = -verticalSpeed * 2f;

        float y_from = newRotation.y;
        float y_to = steerInput * steerSpeed * 2f + dashTime * dashDirection * 200f * (isGrounded ? 5f : 1f);
        newRotation.y = Mathf.LerpAngle(y_from, y_to, Time.deltaTime * 5f);

        newRotation.z = (isGrounded == false && isDashing) ? newRotation.z + 400f * dashDirection * Time.deltaTime : Mathf.LerpAngle(newRotation.z, -steerInput * steerSpeed, Time.deltaTime * 5f);

        float horizontal_scale = Mathf.Clamp(timeSinceJump * timeSinceJump * 1.5f, 0.5f, 1f);
        float vertical_scale = Mathf.Clamp(1 - timeSinceJump * timeSinceJump, Mathf.Max(timeSinceJump, 1f), 2f);
        modelHolder.localScale = isGrounded ? Vector3.one : new Vector3(horizontal_scale, vertical_scale, horizontal_scale);

        // Apply rotations
        transform.localEulerAngles = newRotation;
    }


    public void OnHorizontal(InputValue inputValue)
    {
        steerInput = inputValue.Get<float>();
    }

    public void OnVertical(InputValue inputValue)
    {
        forwardInput = inputValue.Get<float>();
    }


    public void OnJump(InputValue inputValue)
    {
        if (isGrounded == true && inputValue.Get() != null)
        {
            Jump();
        }
        else if (inputValue.Get() == null)
        {
            isJumping = false;
        }
    }

    public void OnLeftDash(InputValue inputValue)
    {
        if (isDashing == false)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashDirection = -1f;
            OnLeftDashed.Invoke();
        }
    }


    public void OnRightDash(InputValue inputValue)
    {
        if (isDashing == false)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashDirection = 1f;
            OnRightDashed.Invoke();
        }
    }


    private void RespawnLerp()
    {
        if (respawnLerpTime > 0)
        {
            Vector3 from = transform.localPosition;
            Vector3 to = new Vector3(isRespawnLerpActive ? 0.0f : transform.localPosition.x, 0.0f, 0.0f);
            float weight = 1 - respawnLerpTime;
            transform.localPosition = Vector3.Lerp(from, to, weight);
            respawnLerpTime -= Time.deltaTime;
        }
        else
        {
            isRespawnLerpActive = false;
        }
    }


    private IEnumerator DisableColliderBriefly(float disabledDuration)
    {
        colliderReference.enabled = false;
        yield return new WaitForSeconds(disabledDuration);
        colliderReference.enabled = true;
    }


    private IEnumerator RespawnDelay(float RespawnDelay)
    {
        yield return new WaitForSeconds(RespawnDelay);
        isDead = false;
        isRespawnLerpActive = true;
        respawnLerpTime = respawnLerpDuration;
        isGrounded = true;
        verticalSpeed = 0f;
        transform.localEulerAngles = Vector3.zero;
    }
}
