using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    // The track the player should respawn on when hitting this trigger
    [SerializeField] private SplineTrack respawnTrack;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerRespawn playerRespawn))
        {
            // Don't respawn the boat when it's on the ground
            if (playerRespawn.playerMovement.isGrounded)
                return;
            
            playerRespawn.TriggerRespawn(respawnTrack);
        }
    }
}
