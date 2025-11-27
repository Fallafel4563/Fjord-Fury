using UnityEngine;
using UnityEngine.Events;

public class PlayerObstacleCollisions : MonoBehaviour
{
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;
    public float slowDownValue = -0.5f;
    public SpeedMultiplierCurve slowDownCurve;

    public UnityEvent HitObstacle;



    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        forwardSpeedMultiplier = GetComponent<ForwardSpeedMultiplier>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacles")
        {
            Debug.Log("Hit Obstacle");
            // Make the player move backwards
            playerMovement.Jump();
            //playerMovement.hitObstacleSpeedMult = -1f;

            // Apply a slowing effect
            forwardSpeedMultiplier.SetForwardSpeedMultiplier("HitObstacle", slowDownValue, slowDownCurve);
        }
    }
}
