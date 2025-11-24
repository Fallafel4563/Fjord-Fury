using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float jumpCameraYOffset = 0f;
    [SerializeField] private float groundCameraYOffset = 4f;
    [SerializeField] private Vector3 respawnOffset = new(0f, 4f, -7f);
    
    [HideInInspector] public Transform trackingTarget;

    private Vector3 defaultPositionOffset;
    private Vector3 defaultPositionDamping;
    private CinemachineBrain cinemachineBrain;
    private CinemachineCamera cinemachineCamera;
    private CinemachinePositionComposer positionComposer;



    private void Awake()
    {
        // Get component references
        cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
        cinemachineCamera = GetComponent<CinemachineCamera>();
        positionComposer = GetComponent<CinemachinePositionComposer>();
    }


    private void Start()
    {
        // Set default values
        cinemachineCamera.Target.TrackingTarget = trackingTarget;
        defaultPositionOffset = positionComposer.TargetOffset;
        defaultPositionDamping = positionComposer.Damping;
    }


    public void SetUpCameraOutputChannel(int playerIndex)
    {
        // Set the output channel of the cinemachine camera and barin to be different channel form other players so that they render properly in splitscreen
        // Different channels are divided by the exponent of 2, so an value of 2 = channel1, 4 = channel2, 8 = channel3, 16 = channel4, etc....
        int outputChannel = (int)Mathf.Pow(2, playerIndex + 1);
        cinemachineBrain.ChannelMask = (OutputChannels)outputChannel;
        cinemachineCamera.OutputChannel = (OutputChannels)outputChannel;
    }


    public void OnLanded()
    {
        positionComposer.TargetOffset.y = groundCameraYOffset;
    }


    public void OnJumped()
    {
        positionComposer.TargetOffset.y = jumpCameraYOffset;
    }


    public void OnRespawnStarted()
    {
        positionComposer.TargetOffset = respawnOffset;
        positionComposer.Damping = Vector3.zero;
        //TODO: Add lerping to the damping so that it's a bit smoother
    }


    public void OnRespawnFinished()
    {
        positionComposer.TargetOffset = defaultPositionOffset;
        positionComposer.Damping = defaultPositionDamping;
        //TODO: Add lerping to the damping so that it's a bit smoother
    }
}
