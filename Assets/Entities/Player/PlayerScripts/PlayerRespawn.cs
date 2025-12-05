using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRespawn : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    public float defaultRespawnOffset = 50f;
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
            TriggerRespawn(playerMovement.mainTrack, defaultRespawnOffset, true);
        }
    }


    public void TriggerRespawn(SplineTrack respawnTrack, float respawnOffset, bool forceMainTrack)
    {
        Debug.Log("RESPAWN STARTED");
        respawnActive = true;
        playerCamera.isRespawning = true;
        RespawnStarted.Invoke();

        StartCoroutine(RespawnFade(respawnTrack, respawnOffset, forceMainTrack));
    }


    private IEnumerator RespawnFade(SplineTrack respawnTrack, float respawnOffset, bool forceMainTrack)
    {
        RespawnFadeInStarted?.Invoke(fadeDuration);
        // Wait for fade in
        yield return new WaitForSeconds(fadeDuration);

        Debug.Log("RESET POS");

        // Reset boat position
        if (forceMainTrack)
        {
            playerMovement.LandedOnTrack(playerMovement.mainTrack);
            transform.localPosition = Vector3.zero;

            splineCart.SplinePosition = playerMovement.lastMainTrackDistance - respawnOffset;
        }
        else
        {
            TrackDistanceInfo distanceInfo = respawnTrack.GetDistanceInfoFromPosition(transform.position);
            transform.position = distanceInfo.nearestSplinePos;
            // Make the boat land on the track
            playerMovement.LandedOnTrack(respawnTrack);
            splineCart.SplinePosition = distanceInfo.distance - respawnOffset;
        }

        // Stop player movement
        playerMovement.enabled = false;
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
        Debug.Log("RESPAWN ENDED");
        respawnActive = false;
        // Reset player stuff
        playerMovement.enabled = true;

        // Re-enable spline cart
        splineCart.AutomaticDolly.Enabled = true;

        RespawnFinished.Invoke();
    }
}
