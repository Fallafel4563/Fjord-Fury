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

    public PlayerMovement playerMovement;
    public PlayerCamera playerCamera;
    public CinemachineSplineCart splineCart;

    private bool respawnActive = false;

    public Action<float> RespawnFadeInStarted;
    public Action<float> RespawnFadeOutStarted;



    private void Update()
    {
        // Trigger backup respawn
        if (transform.position.y < globalDeathYPosition && !respawnActive)
        {
            TriggerRespawn(null, defaultRespawnOffset);
        }
    }


    public void TriggerRespawn(SplineTrack respawnTrack, float respawnOffset)
    {
        // Don't trigger multiple respawns when the respawn is active
        if (respawnActive)
            return;
        
        Debug.Log("Respawn started");
        respawnActive = true;
        playerMovement.isRespawning = true;
        playerCamera.isRespawning = true;
        RespawnStarted.Invoke();

        StartCoroutine(RespawnFade(respawnTrack, respawnOffset));
    }


    private IEnumerator RespawnFade(SplineTrack respawnTrack, float respawnOffset)
    {
        RespawnFadeInStarted?.Invoke(fadeDuration);
        // Wait for fade in
        yield return new WaitForSeconds(fadeDuration);

        // Reset boat position
        if (!respawnTrack)
        {
            playerMovement.LandedOnTrack(playerMovement.mainTrack);
            transform.localPosition = Vector3.zero;

            yield return new WaitForEndOfFrame();

            splineCart.SplinePosition = playerMovement.lastMainTrackDistance - respawnOffset;
            Debug.LogFormat(
                "Main track respawn \nSpline length {0} \nLast distance {1} \nOffset {2} \nDesired respawn distance {3} \nActual respawn distance {4} \n",
                playerMovement.mainTrack.track.Spline.GetLength(), // Length
                playerMovement.lastMainTrackDistance, // Last distance
                respawnOffset, // Offset
                playerMovement.lastMainTrackDistance - respawnOffset, // Desried respawn distance
                splineCart.SplinePosition); // Actual respawn distance
        }
        else
        {
            TrackDistanceInfo distanceInfo = respawnTrack.GetDistanceInfoFromPosition(transform.position);
            transform.position = distanceInfo.nearestSplinePos;
            // Make the boat land on the track
            playerMovement.LandedOnTrack(respawnTrack);
            transform.localPosition = Vector3.zero;

            yield return new WaitForEndOfFrame();

            splineCart.SplinePosition = distanceInfo.distance - respawnOffset;
            Debug.LogFormat(
                "Respawn track respawn \nRespawn track name {5} \nRespawn track length {0} \nDistance {1} \nOffset {2} \nDesired respawn distance {3} \nActual respawn distance {4} \n",
                respawnTrack.track.Spline.GetLength(), // Length
                distanceInfo.distance, // Distance
                respawnOffset, // Offset
                distanceInfo.distance - respawnOffset, // Desried respawn distance
                splineCart.SplinePosition, // Actual respawn distance
                respawnTrack.gameObject.name); // Desired respawn track
        }

        yield return new WaitForEndOfFrame();

        // Stop player movement
        playerMovement.enabled = false;
        playerMovement.isRespawning = false;
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
        Debug.Log("Respawn ended");
        respawnActive = false;
        // Reset player stuff
        playerMovement.enabled = true;

        // Re-enable spline cart
        splineCart.AutomaticDolly.Enabled = true;

        RespawnFinished.Invoke();
    }
}
