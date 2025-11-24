using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRespawn : MonoBehaviour
{
    public float respawnLerpDuration = 1f;
    private float deathYPosition = -100f;
    public UnityEvent RespawnStarted;
    public UnityEvent RespawnFinished;

    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public CinemachineSplineCart splineCart;

    private bool respawnActive = false;
    private float respawnLerpTime;
    private Vector3 respawnLerpStart;



    private void Update()
    {
        Vector3 relativePos = splineCart.transform.InverseTransformPoint(playerMovement.transform.position);
        if (respawnActive)
            RespawnLerp();
        else if (relativePos.y < deathYPosition && !respawnActive)
            StartRespawn();
    }



    private void RespawnLerp()
    {
        Vector3 from = respawnLerpStart;
        Vector3 to = splineCart.transform.position;
        // Get the % of how far the boat should've moved between from and to
        respawnLerpTime += Time.deltaTime;
        float weight = respawnLerpTime / respawnLerpDuration;

        // Move boat to the to position over
        playerMovement.transform.position = Vector3.Lerp(from, to, weight);

        // Stop respawning when it has respawned for respawnLerpDuration
        if (respawnLerpTime >= respawnLerpDuration)
        {
            FinishRespawn();
        }
    }



    public void StartRespawn()
    {
        respawnLerpTime = 0f;
        respawnLerpStart = playerMovement.transform.position;

        splineCart.AutomaticDolly.Enabled = false;

        respawnActive = true;
        playerMovement.enabled = false;
        RespawnStarted.Invoke();
    }



    public void FinishRespawn()
    {
        // Reset player stuff
        playerMovement.enabled = true;
        playerMovement.AttachToCart();
        playerMovement.ySpeed = 0f;
        playerMovement.isGrounded = true;
        playerMovement.wasLastTrackRail = false;
        playerMovement.SetOverrideSpeed(playerMovement.currentTrack.overrideSpeed);

        splineCart.AutomaticDolly.Enabled = true;

        respawnActive = false;
        RespawnFinished.Invoke();
    }
}
