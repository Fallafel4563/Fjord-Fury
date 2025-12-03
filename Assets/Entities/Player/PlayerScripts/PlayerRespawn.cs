using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRespawn : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private float globalDeathYPosition = -200f;
    public UnityEvent RespawnStarted;
    public UnityEvent RespawnFinished;

    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerCamera playerCamera;
    [HideInInspector] public CinemachineSplineCart splineCart;

    private bool respawnActive = false;

    public Action<float> RespawnFadeInStarted;
    public Action<float> RespawnFadeOutStarted;



    private void Update()
    {
        // Trigger backup respawn
        if (transform.position.y < globalDeathYPosition && !respawnActive)
        {
            TriggerRespawn(playerMovement.mainTrack);
        }
    }


    public void TriggerRespawn(SplineTrack respawnTrack)
    {
        respawnActive = true;
        playerCamera.isRespawning = true;
        RespawnStarted.Invoke();

        StartCoroutine(RespawnFade(respawnTrack));
    }


    private IEnumerator RespawnFade(SplineTrack respawnTrack)
    {
        RespawnFadeInStarted?.Invoke(fadeDuration);
        // Wait for fade in
        yield return new WaitForSeconds(fadeDuration);

        // Stop player movement
        playerMovement.enabled = false;

        // Reset boat position
        TrackDistanceInfo distanceInfo = respawnTrack.GetDistanceInfoFromPosition(transform.position);
        transform.position = distanceInfo.nearestSplinePos;
        // Make the boat land on the track
        playerMovement.LandedOnTrack(respawnTrack);
        // Stop the spline cart from moving again (Is enabled when the boat lands on a track)
        splineCart.AutomaticDolly.Enabled = false;

        // Reset camera
        playerCamera.transform.position = playerCamera.trackingTarget.transform.position;
        playerCamera.isRespawning = false;

        // Wait for fade out
        RespawnFadeOutStarted?.Invoke(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        FinishRespawn();
    }



    private void FinishRespawn()
    {
        respawnActive = false;
        // Reset player stuff
        playerMovement.enabled = true;
        //playerMovement.airVelocity = Vector3.zero;
        //playerMovement.wasLastTrackRail = false;
        //playerMovement.SetOverrideSpeed(playerMovement.currentTrack.overrideSpeed);

        // Re-enable spline cart
        splineCart.AutomaticDolly.Enabled = true;

        RespawnFinished.Invoke();
    }
}
