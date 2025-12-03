using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRespawn : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private float deathYPosition = -100f;
    public UnityEvent RespawnStarted;
    public UnityEvent RespawnFinished;

    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerCamera playerCamera;
    [HideInInspector] public CinemachineSplineCart splineCart;

    private bool respawnActive = false;



    private void Update()
    {
        Vector3 relativePos = splineCart.transform.InverseTransformPoint(playerMovement.transform.position);
        if (!respawnActive && relativePos.y < deathYPosition)
            StartCoroutine(nameof(TriggerRespawn));
    }


    private IEnumerator TriggerRespawn()
    {
        // Reset spline cart
        StartRespawn();
        // Wait for fade in
        yield return new WaitForSeconds(fadeDuration);

        // Reset boat position
        playerMovement.enabled = false;
        float respawnHeight = 3f;
        if (playerMovement.currentTrack.isCircle)
            respawnHeight += playerMovement.currentTrack.width;
        
        playerMovement.transform.position = splineCart.transform.position + splineCart.transform.up * respawnHeight;
        // Reset camera
        playerCamera.transform.position = playerCamera.trackingTarget.transform.position;
        playerCamera.isRespawning = false;

        // Wait for fade out
        yield return new WaitForSeconds(fadeDuration);
        FinishRespawn();
    }



    public void StartRespawn()
    {
        respawnActive = true;

        splineCart.AutomaticDolly.Enabled = false;
        // Respawn the palyer 50 units behind where they jumped off the track
        splineCart.SplinePosition = playerMovement.distanceWhenJumped - 50f;

        playerCamera.isRespawning = true;

        RespawnStarted.Invoke();
    }



    public void FinishRespawn()
    {
        // Reset player stuff
        playerMovement.enabled = true;
        playerMovement.airVelocity = Vector3.zero;
        playerMovement.wasLastTrackRail = false;
        playerMovement.SetOverrideSpeed(playerMovement.currentTrack.overrideSpeed);

        splineCart.AutomaticDolly.Enabled = true;

        respawnActive = false;
        RespawnFinished.Invoke();
    }
}
