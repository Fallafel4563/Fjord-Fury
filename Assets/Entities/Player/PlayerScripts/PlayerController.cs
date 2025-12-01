using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("External References")]
    public SplineTrack mainTrack;

    [HideInInspector] public PlayerHud playerHud;


    [Header("Internal References")]
    [SerializeField] private CinemachineSplineCart splineCart;
    public PlayerMovement playerMovement;
    public PlayerCamera playerCamera;
    [SerializeField] private PlayerRespawn playerRespawn;
    [SerializeField] private BoatMovementAnims boatMovementAnims;
    [SerializeField] private TrickComboSystem trickComboSystem;
    [SerializeField] private ForwardSpeedMultiplier forwardSpeedMultiplier;
    [SerializeField] private PlayerObstacleCollisions playerObstacleCollisions;


    private PlayerInput playerInput;



    private void Awake()
    {   
        playerInput = GetComponent<PlayerInput>();
    }


    private void OnEnable()
    {
        trickComboSystem.TrickScoreUpdated += playerHud.TrickScoreUpdated;
    }

    private void OnDisable()
    {
        trickComboSystem.TrickScoreUpdated -= playerHud.TrickScoreUpdated;
    }


    // Set references on different systems
    private void Start()
    {
        splineCart.Spline = mainTrack.track;

        playerMovement.splineCart = splineCart;
        playerMovement.mainTrack = mainTrack;
        playerMovement.forwardSpeedMultiplier = forwardSpeedMultiplier;

        playerCamera.playerMovement = playerMovement;
        playerCamera.trackingTarget = playerMovement.transform;
        playerCamera.forwardSpeedMultiplier = forwardSpeedMultiplier;
        playerCamera.SetUpCameraOutputChannel(playerInput.playerIndex);

        playerRespawn.splineCart = splineCart;
        playerRespawn.playerMovement = playerMovement;
        playerRespawn.playerCamera = playerCamera;

        boatMovementAnims.playerMovement = playerMovement;
        boatMovementAnims.trickComboSystem = trickComboSystem;

        trickComboSystem.playerMovement = playerMovement;
        trickComboSystem.forwardSpeedMultiplier = forwardSpeedMultiplier;
        trickComboSystem.boatMovementAnims = boatMovementAnims;

        playerObstacleCollisions.playerMovement = playerMovement;
        playerObstacleCollisions.trickComboSystem = trickComboSystem;

        playerHud.SetupHud(playerInput.playerIndex, playerCamera.activeCamera);
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
