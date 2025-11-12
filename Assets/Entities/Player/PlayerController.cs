using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private float dashDoubleTapTiming;

    [Header("Movement")]
    [SerializeField] private SplineBoat splineBoat;

    private float forwardInput;
    private float steerInput;
    private bool isJumping;


    private void Start()
    {
        SetUpCameraOutputChannel();
    }


    public void OnForward(InputValue inputValue)
    {
        // Get input
        forwardInput = inputValue.Get<float>();
        // Send input data to boat movement
        splineBoat.forwardInput = forwardInput;
    }

    public void OnSteer(InputValue inputValue)
    {
        // Get input data
        steerInput = inputValue.Get<float>();
        // Send input data to boat movement
        splineBoat.steerInput = steerInput;
    }

    public void OnJump(InputValue inputValue)
    {
        isJumping = inputValue.Get<float>() > 0.5f;
        splineBoat.isJumping = isJumping;
        if (isJumping == true)
        {
            splineBoat.Jump();
        }
    }

    public void OnLeftDash()
    {
        splineBoat.DashLeft();
    }

    public void OnRightDash()
    {
        splineBoat.DashRight();
    }


    private void SetUpCameraOutputChannel()
    {
        // Set the output channel of the cinemachine camera and barin to be different channel form other players so that they render properly in splitscreen
        // Different channels are divided by the exponent of 2, so an value of 2 = channel1, 4 = channel2, 8 = channel3, 16 = channel4, etc....
        int outputChannel = (int)Mathf.Pow(2, playerInput.playerIndex + 1);
        cinemachineBrain.ChannelMask = (OutputChannels)outputChannel;
        cinemachineCamera.OutputChannel = (OutputChannels)outputChannel;
    }
}
