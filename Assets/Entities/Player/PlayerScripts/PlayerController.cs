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
    public CinemachineSplineCart splineCart;
    public PlayerMovement playerMovement;
    public PlayerCamera playerCamera;
    [SerializeField] private PlayerRespawn playerRespawn;
    [SerializeField] private TrickComboSystem trickComboSystem;
    [SerializeField] private GameObject skins;

    public int selectedCharacter { get; set; } = 0;
    public int playerIndex { get; private set; }

    private PlayerInput playerInput;



    // Set references on different systems
    private void Awake()
    {   
        playerInput = GetComponent<PlayerInput>();

        splineCart.Spline = mainTrack.track;
        playerMovement.mainTrack = mainTrack;
    }


    private void OnEnable()
    {
        // Connect events to hud
        if (playerHud)
        {
            trickComboSystem.UpdateBoostMeterVisibility += playerHud.UpdateBoostMeterVisibility;
            trickComboSystem.UpdateBoostMeter += playerHud.boostMeter.OnUpdateBoostMeter;
            trickComboSystem.ResetBoostMeter += playerHud.boostMeter.OnResetBoostMeter;

            playerRespawn.RespawnFadeInStarted += playerHud.OnRespawnFadeInStarted;
            playerRespawn.RespawnFadeOutStarted += playerHud.OnRespawnFadeOutStarted;
        }
    }

    private void OnDisable()
    {
        // Disconnect events from hud
        if (playerHud)
        {
            trickComboSystem.UpdateBoostMeterVisibility -= playerHud.UpdateBoostMeterVisibility;
            trickComboSystem.UpdateBoostMeter -= playerHud.boostMeter.OnUpdateBoostMeter;
            trickComboSystem.ResetBoostMeter -= playerHud.boostMeter.OnResetBoostMeter;

            playerRespawn.RespawnFadeInStarted -= playerHud.OnRespawnFadeInStarted;
            playerRespawn.RespawnFadeOutStarted -= playerHud.OnRespawnFadeOutStarted;
        }
    }


    // Additional setup on systems
    private void Start()
    {
        playerCamera.SetUpCameraOutputChannel(playerInput.playerIndex);
        playerHud.SetupHud(playerInput.playerIndex, playerCamera.activeCamera);

        SetActiveSkin();
    }


    private void SetActiveSkin()
    {
        // Hide all skins
        for (int i = 0; i < skins.transform.childCount; i++)
        {
            skins.transform.GetChild(i).gameObject.SetActive(false);
        }

        // Enable the selected character skin
        skins.transform.GetChild(selectedCharacter).gameObject.SetActive(true);
    }



#region Input
    [Header("Input")]
    public bool inputEnabled = true;
    private float forwardInput;
    private float steerInput;
    private bool jumpInput;
    private bool driftInput;



    public void OnForward(InputValue inputValue)
    {
        if (!inputEnabled)
            return;
        
        // Get input
        forwardInput = inputValue.Get<float>();
        // Send input data to boat movement
        playerMovement.forwardInput = forwardInput;
    }


    public void OnSteer(InputValue inputValue)
    {
        if (!inputEnabled)
            return;
        
        // Get input data
        steerInput = inputValue.Get<float>();
        // Send input data to boat movement
        playerMovement.steerInput = steerInput;
        playerCamera.steerInput = steerInput;
    }


    public void OnJump(InputValue inputValue)
    {
        if (!inputEnabled)
            return;
        
        jumpInput = inputValue.Get<float>() > 0.5f;
        playerMovement.jumpInput = jumpInput;
        if (jumpInput == true)
        {
            playerMovement.Jump();
        }
    }


    private void OnDrift(InputValue inputValue)
    {
        if (!inputEnabled)
            return;
        
        driftInput = inputValue.Get<float>() > 0.5f;
        playerMovement.driftInput = driftInput;
        if (driftInput && !playerMovement.isDrifting)
            playerMovement.StartDrift();
        else if (playerMovement.isDrifting)
            playerMovement.EndDrift();
    }


    public void OnShortTrick()
    {
        if (!inputEnabled)
            return;
        
        trickComboSystem.ActivateTrick(0);
    }


    public void OnMediumTrick()
    {
        if (!inputEnabled)
            return;
        
        trickComboSystem.ActivateTrick(1);
    }


    public void OnLongTrick()
    {
        if (!inputEnabled)
            return;
        
        trickComboSystem.ActivateTrick(2);
    }

#endregion
}
