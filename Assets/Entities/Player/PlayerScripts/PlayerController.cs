using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("External References")]
    public SplineTrack mainTrack;


    [Header("Internal References")]
    [SerializeField] private CinemachineSplineCart splineCart;
    public PlayerMovement playerMovement;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private PlayerRespawn playerRespawn;
    [SerializeField] private BoatMovementAnims boatMovementAnims;
    [SerializeField] private TrickComboSystem trickComboSystem;


    private PlayerInput playerInput;



    private void Awake()
    {   
        playerInput = GetComponent<PlayerInput>();
    }


    // Set references on children
    private void Start()
    {
        splineCart.Spline = mainTrack.track;

        playerMovement.splineCart = splineCart;
        playerMovement.mainTrack = mainTrack;

        playerCamera.trackingTarget = playerMovement.transform;
        playerCamera.playerMovement = playerMovement;
        playerCamera.forwardSpeedMultiplier = trickComboSystem.forwardSpeedMultiplier;
        playerCamera.SetUpCameraOutputChannel(playerInput.playerIndex);

        playerRespawn.splineCart = splineCart;
        playerRespawn.playerMovement = playerMovement;
        playerRespawn.playerCamera = playerCamera;

        boatMovementAnims.playerMovement = playerMovement;

        trickComboSystem.playerMovement = playerMovement;
    }



#region Input
    [Header("Input")]
    [SerializeField] private float dashDoubleTapTiming = 0.2f;

    private float forwardInput;
    private float steerInput;
    private bool jumpInput;



    public void OnForward(InputValue inputValue)
    {
        // Get input
        forwardInput = inputValue.Get<float>();
        // Send input data to boat movement
        playerMovement.forwardInput = forwardInput;
    }


    public void OnSteer(InputValue inputValue)
    {
        // Get input data
        steerInput = inputValue.Get<float>();
        // Send input data to boat movement
        playerMovement.steerInput = steerInput;
        playerCamera.steerInput = steerInput;
    }


    public void OnJump(InputValue inputValue)
    {
        jumpInput = inputValue.Get<float>() > 0.5f;
        playerMovement.jumpInput = jumpInput;
        if (jumpInput == true)
        {
            playerMovement.Jump();
        }
    }


    public void OnTrick()
    {
        trickComboSystem.inputBuffer = trickComboSystem.inputBufferDefault;
    }


    public void OnLeftDash()
    {
        playerMovement.DashLeft();
    }


    public void OnRightDash()
    {
        playerMovement.DashRight();
    }

#endregion
}
