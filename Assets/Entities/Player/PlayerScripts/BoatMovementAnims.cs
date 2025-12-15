using UnityEngine;
using UnityEngine.Events;

public class BoatMovementAnims : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public TrickComboSystem trickComboSystem;
    public UnityEvent TrickAnimFinishedFX;

    

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
        float yTo = playerMovement.steerInput * playerMovement.steerSpeed * groundTilt;
        newRotation.y = Mathf.LerpAngle(yFrom, yTo, Time.deltaTime * 5f);
        // Roll
        newRotation.z = Mathf.LerpAngle(newRotation.z, -playerMovement.steerInput * playerMovement.steerSpeed, Time.deltaTime * 5f);
        // Apply rotation
        transform.localEulerAngles = newRotation;

        trickComboSystem.animator.SetBool("Grinding", playerMovement.currentTrack.isCircle);
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
        float yTo = playerMovement.steerInput * playerMovement.steerSpeed * airTilt;
        newRotation.y = Mathf.LerpAngle(yFrom, yTo, Time.deltaTime * 5f);
        // Roll
        newRotation.z = Mathf.LerpAngle(newRotation.z, -playerMovement.steerInput * playerMovement.steerSpeed, Time.deltaTime * 5f);
        // Apply rotation
        transform.localEulerAngles = newRotation;

        trickComboSystem.animator.SetBool("Grinding", false);
    }

    public void TrickAnim()
    {
        // Play trick animation
        trickComboSystem.animator.SetInteger("Trick Index", trickComboSystem.trickIndex);
        trickComboSystem.animator.SetTrigger("Regular Trick");

        
    }

    public void OnTrickAnimFinished()
    {
        if (trickComboSystem.performingTrick)
            trickComboSystem.OnTrickCompleted();
        
        TrickAnimFinishedFX.Invoke();
    }


    public void OnDoubleJump()
    {
        trickComboSystem.animator.SetTrigger("DoubleJump");
    }
}
