using System.Collections;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DriftSystem : MonoBehaviour
{
    [SerializeField] private float driftSlowdownMultiplier = 0.8f;
    [SerializeField] private float minDriftBoostTime = 1f;

    private float driftTimePassed = 0f;
    private float boostTimePassed = 0f;

    [SerializeField] private AnimationCurve releaseBoostCurve;
    [SerializeField] private SpeedMultiplierCurve driftReleaseMultiplierCurve;

    private float startYRotation;
    private float rotationOffset;
    private float driftDirection;
    [SerializeField] private float maxRotation = 30f;
    [SerializeField] private float minRotation = 20f;

    //[SerializeField] private ParticleSystem driftStartTrail;
    //[SerializeField] private ParticleSystem driftBoostTrail;
    //[SerializeField] private ParticleSystem superBoostTrail;

    [HideInInspector] public bool isDrifting = false;
    [HideInInspector] public float steerInput = 0f;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;


    public void StartDrift()
    {
        if (playerMovement.isGrounded && !playerMovement.currentTrack.isCircle && !Mathf.Approximately(steerInput, 0f))
        {
            forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drifting", driftSlowdownMultiplier);

            driftTimePassed = 0f;
            boostTimePassed = 0f;
            playerMovement.clampXAxis = false;
            driftDirection = Mathf.Sign(steerInput);

            startYRotation = transform.rotation.y;

            // TODO: Enable drfit boost trail

            isDrifting = true;
        }
    }


    private void Update()
    {
        if (isDrifting)
        {
            float lerpTarget = (minRotation + maxRotation / 2f);
            if (Mathf.Approximately(steerInput, driftDirection))
            {
                lerpTarget = maxRotation;
                Debug.Log("Max rotation");
            }
            else if (Mathf.Approximately(steerInput, driftDirection * -1f))
            {
                Debug.Log("Min rotation");
                lerpTarget = minRotation;
            }

            rotationOffset = Mathf.Lerp(rotationOffset, lerpTarget, Time.deltaTime * 5f);

            transform.rotation = Quaternion.Euler(transform.rotation.x, startYRotation + rotationOffset, transform.rotation.z);

            forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drifting Rotation", Mathf.Cos(rotationOffset * Mathf.Deg2Rad));

            transform.localPosition += new Vector3(Mathf.Sin(rotationOffset * Mathf.Deg2Rad) * playerMovement.currentForwardSpeed * Time.deltaTime, 0f, 0f);
            Debug.LogFormat("Sin {0}, Rot {1}, Z {2}", Mathf.Sin(rotationOffset * Mathf.Deg2Rad), rotationOffset, transform.localPosition.z);

            driftTimePassed += Time.deltaTime;

            if (driftTimePassed > minDriftBoostTime)
            {
                // TODO: Enable drift boost trail
                // TODO: Disable drift trail
            }
        }
        else
        {
            rotationOffset = Mathf.Lerp(rotationOffset, 0f, Time.deltaTime);
        }
    }


    public void EndDrift()
    {
        if (!isDrifting)
            return;

        isDrifting = false;

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drifting", 1f);
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drifting Rotation", 1f);

        // TODO: Disable boost trail
        // TODO: Disable start trail

        playerMovement.clampXAxis = true;

        if (playerMovement.isGrounded)
            playerMovement.LandedOnTrack(playerMovement.currentTrack);

        // Start boost when the drift ends
        if (minDriftBoostTime < driftTimePassed)
            DriftBoost();
    }


    private IEnumerator DriftBoost()
    {
        // TODO: Enable super boost trail

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Drift Release Boost", releaseBoostCurve.Evaluate(boostTimePassed), driftReleaseMultiplierCurve);
        yield return new WaitForSeconds(driftReleaseMultiplierCurve.GetLength());

        // TODO: Disable super boost trail
    }
}
