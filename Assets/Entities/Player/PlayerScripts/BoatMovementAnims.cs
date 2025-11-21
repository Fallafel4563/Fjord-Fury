using UnityEngine;

public class BoatMovementAnims : MonoBehaviour
{
    [HideInInspector] public PlayerMovement playerMovement;

    [Header("General")]
    public float lerpSpeed = 5f;


    [Header("Grounded")]
    public float groundTilt = 1f;


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
        // Set scale
        transform.localScale = Vector3.one;

        // Set rotation
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = -playerMovement.ySpeed * 2f;

        float yFrom = newRotation.y;
        float yTo = playerMovement.steerInput * playerMovement.steerSpeed * 2f + playerMovement.dashTime * playerMovement.dashDirection * 200f * groundTilt;
        newRotation.y = Mathf.LerpAngle(yFrom, yTo, Time.deltaTime * 5f);

        newRotation.z = (playerMovement.isGrounded == false && playerMovement.isDashing) ? newRotation.z + 400f * playerMovement.dashDirection * Time.deltaTime : Mathf.LerpAngle(newRotation.z, -playerMovement.steerInput * playerMovement.steerSpeed, Time.deltaTime * 5f);

        transform.localEulerAngles = newRotation;
    }



    private void AirborneAnim()
    {
        // Set scale
        float horizontalScale = Mathf.Clamp(playerMovement.timeSinceJump * playerMovement.timeSinceJump * 1.5f, 0.5f, 1f);
        float verticalScale = Mathf.Clamp(1 - playerMovement.timeSinceJump * playerMovement.timeSinceJump, Mathf.Max(playerMovement.timeSinceJump, 1f), 2f);
        transform.localScale = new(horizontalScale, verticalScale, horizontalScale);

        // Set rotation
        // Set rotation
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = -playerMovement.ySpeed * 2f;

        float yFrom = newRotation.y;
        float yTo = playerMovement.steerInput * playerMovement.steerSpeed * 2f + playerMovement.dashTime * playerMovement.dashDirection * 200f * airTilt;
        newRotation.y = Mathf.LerpAngle(yFrom, yTo, Time.deltaTime * 5f);

        newRotation.z = (playerMovement.isGrounded == false && playerMovement.isDashing) ? newRotation.z + 400f * playerMovement.dashDirection * Time.deltaTime : Mathf.LerpAngle(newRotation.z, -playerMovement.steerInput * playerMovement.steerSpeed, Time.deltaTime * 5f);

        transform.localEulerAngles = newRotation;
    }
}
