using UnityEngine;

public class BoatMovementAnims : MonoBehaviour
{
    [HideInInspector] public PlayerMovement playerMovement;
    public TrickComboSystem trickComboSystem;
    

    [Header("General")]
    public float lerpSpeed = 1f;


    [Header("Grounded")]
    public float groundTilt = 5f;


    [Header("Airborne")]
    public float airTilt = 5f;


    private void Update()
    {
        if (playerMovement.isGrounded)
            GroundedAnim();
        else
            AirborneAnim();
    }


    private void GroundedAnim()
    {
        // Scale
        transform.localScale = Vector3.one;


        // Rotation
        Vector3 newRotation = transform.localEulerAngles;
        // Pitch
        newRotation.x = 0f;
        // Yaw
        float yFrom = newRotation.y;
        float yTo = playerMovement.steerInput * playerMovement.steerSpeed * 2f + playerMovement.dashTime * playerMovement.dashDirection * 200f * groundTilt;
        newRotation.y = Mathf.LerpAngle(yFrom, yTo, Time.deltaTime * 5f);
        // Roll
        newRotation.z = Mathf.LerpAngle(newRotation.z, -playerMovement.steerInput * playerMovement.steerSpeed, Time.deltaTime * 5f);
        // Apply rotation
        transform.localEulerAngles = newRotation;
    }



    private void AirborneAnim()
    {
        /*
        // Scale
        float horizontalScale = Mathf.Clamp(playerMovement.timeSinceJump * playerMovement.timeSinceJump * 1.5f, 0.5f, 1f);
        float verticalScale = Mathf.Clamp(1 - playerMovement.timeSinceJump * playerMovement.timeSinceJump, Mathf.Max(playerMovement.timeSinceJump, 1f), 2f);
        transform.localScale = new(horizontalScale, verticalScale, horizontalScale);
        */

        // Rotation
        Vector3 newRotation = transform.localEulerAngles;
        // Pitch
        newRotation.x = -playerMovement.airVelocity.y * 2f;
        newRotation.x = Mathf.Clamp(newRotation.x, -89f, 89f);
        // Yaw
        float yFrom = newRotation.y;
        float yTo = playerMovement.steerInput * playerMovement.steerSpeed * 2f + playerMovement.dashTime * playerMovement.dashDirection * 200f * airTilt;
        newRotation.y = Mathf.LerpAngle(yFrom, yTo, Time.deltaTime * 5f);
        // Roll
        newRotation.z = playerMovement.isDashing ? newRotation.z + 400f * playerMovement.dashDirection * Time.deltaTime : Mathf.LerpAngle(newRotation.z, -playerMovement.steerInput * playerMovement.steerSpeed, Time.deltaTime * 5f);
        // Apply rotation
        transform.localEulerAngles = newRotation;
    }

    public void TrickAnim()
    {
        // Play trick animation
        trickComboSystem.animator.SetInteger("Trick Index", trickComboSystem.trickIndex);
        trickComboSystem.animator.SetTrigger("Regular Trick");
        
    }
}
