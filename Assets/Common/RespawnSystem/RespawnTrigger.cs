using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    // Should the trigger force the player to respawn in the last main track they thouched
    [SerializeField] private bool forceMainTrack = false;
    [SerializeField] private float repsawnOffset = 50f;
    // The track the player should respawn on when hitting this trigger
    [SerializeField] private SplineTrack respawnTrack;


    private void Start()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderer.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerRespawn playerRespawn))
        {
            // Don't respawn the boat when it's on the ground
            if (playerRespawn.playerMovement.isGrounded)
                return;
            
            playerRespawn.TriggerRespawn(respawnTrack, repsawnOffset, forceMainTrack);
        }
    }
}
