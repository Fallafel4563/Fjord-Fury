using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("General")]
    // How fast to lerp between truePosOffset and desiredPosOffset
    [SerializeField] private float minFov = 65f;
    [SerializeField] private float maxFov = 90f;
    [SerializeField] private float fovLerpSpeed = 4f;
    [SerializeField] private float rotLerpSpeed = 7.5f;
    [SerializeField] private float desiredOffsetLerpSpeed = 5f;
    [SerializeField] private Vector3 rotationOffset = new(0f, 1.25f, 5f);


    [Header("Grounded")]
    [SerializeField] private Vector3 groundPosOffset = new(0f, 4f, 4f);
    // How fast the camera should move towards the offset position while on the ground
    [SerializeField] private Vector3 groundPosLerpSpeed = new(5f, 5f, 5f);


    [Header("Airborne")]
    [SerializeField] private Vector3 airPosOffset = new(0f, 2f, 4f);
    // How fast the camera should move towards the offset position while in the air
    [SerializeField] private Vector3 airPosLerpSpeed = new(10f, 10f, 10f);



    public bool isRespawning { get; set; } = false;
    public float steerInput { get; set; }

    private Vector3 posLerpSpeed;       // How fast the camera should move towrds the offset position
    private Vector3 posOffset;          // Where the position offset is
    private Vector3 desiredPosOffset;   // Where the position offset wants to be

    public Transform trackingTarget;
    public PlayerMovement playerMovement;
    public ForwardSpeedMultiplier forwardSpeedMultiplier;
    public Camera activeCamera { get; set; }
    public CinemachineCamera cinemachineCamera { get; set; }
    public CinemachineBrain cinemachineBrain { get; set; }



    private void Awake()
    {
        // Get component references
        cinemachineCamera = GetComponent<CinemachineCamera>();
        cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
        activeCamera = GetComponentInChildren<Camera>();
        // Set default values
        cinemachineCamera.Target.TrackingTarget = trackingTarget;
    }


    private void Start()
    {
        posOffset = groundPosOffset;
        desiredPosOffset = groundPosOffset;
        posLerpSpeed = groundPosLerpSpeed;
    }



    private void Update()
    {
        // Get the 
        desiredPosOffset = (playerMovement.isGrounded || playerMovement.isDrifting) ? groundPosOffset : airPosOffset;
        posLerpSpeed = (playerMovement.isGrounded || playerMovement.isDrifting) ? groundPosLerpSpeed : airPosLerpSpeed;

        // Get the true position offset
        desiredPosOffset.x = steerInput;
        posOffset = Vector3.Lerp(posOffset, desiredPosOffset, desiredOffsetLerpSpeed * Time.deltaTime);
    }



    private void LateUpdate()
    {
        // Only set the position when not respawning
        if (!isRespawning)
            SetCameraPosition();
        SetCameraRotation();

        // Update fov based on the how fast the player is moving
        UpdateFovBasedOnSpeed();
    }



    private void SetCameraPosition()
    {
        Vector3 xOffset = trackingTarget.right * posOffset.x;
        float upwardsVel = Vector3.Dot(transform.up, playerMovement.airVelocity.normalized);
        Vector3 yOffset = trackingTarget.up * (posOffset.y - (Mathf.Clamp(upwardsVel, -1f, 0f) * 5f));
        Vector3 zOffset = -trackingTarget.forward * posOffset.z;
        // Get the position the camera wants to be at
        Vector3 desiredPosition = trackingTarget.position + xOffset + yOffset + zOffset;

        // Move camrea to desired position
        float yPos = Mathf.Lerp(transform.position.y, desiredPosition.y, posLerpSpeed.y * Time.deltaTime);
        transform.position = new(desiredPosition.x, yPos, desiredPosition.z);
    }


    private void SetCameraRotation()
    {
        // Get the position the camera wants to look at
        Vector3 xLookAtPos = trackingTarget.right * rotationOffset.x * steerInput;
        Vector3 yLookAtPos = trackingTarget.up * rotationOffset.y;
        Vector3 zLookAtPos = trackingTarget.forward * rotationOffset.z;
        Vector3 lookAtPos = trackingTarget.position + xLookAtPos + yLookAtPos + zLookAtPos;

        // Get the rotation the camera has to be at to look at the look at position
        Quaternion desiredRotation = Quaternion.LookRotation(lookAtPos - transform.position);
        //desiredRotation.eulerAngles = new(desiredRotation.eulerAngles.x, desiredRotation.eulerAngles.y, desiredRotation.eulerAngles.z + steerInput);
        // Rotate camera towards the desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotLerpSpeed * Time.deltaTime);
    }


    public void SetUpCameraOutputChannel(int playerIndex)
    {
        // Set the output channel of the cinemachine camera and barin to be different channel form other players so that they render properly in splitscreen
        // Different channels are divided by the exponent of 2, so an value of 2 = channel1, 4 = channel2, 8 = channel3, 16 = channel4, etc....
        int outputChannel = (int)Mathf.Pow(2, playerIndex + 1);
        cinemachineBrain.ChannelMask = (OutputChannels)outputChannel;
        cinemachineCamera.OutputChannel = (OutputChannels)outputChannel;
    }


    private void UpdateFovBasedOnSpeed()
    {
        if (forwardSpeedMultiplier.GetForwardSpeedMultiplier("ImmediateComboBoost") == null)
            return;
        
        // Get the difference between the max fov and the min fov
        float fovDiff = maxFov - minFov;
        // How much more fov to add to the min value. Difference * speed multiplier
        float additionalFov = fovDiff * (forwardSpeedMultiplier.GetForwardSpeedMultiplier("ImmediateComboBoost").value - 1f);
        // Add additional fov to the min to get the desired fov
        float desiredFov = minFov + additionalFov;
        // Clamp fov between min and max
        desiredFov = Mathf.Clamp(desiredFov, minFov, maxFov);
        // Set fov
        cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, desiredFov, fovLerpSpeed * Time.deltaTime);
        Debug.LogFormat(
            "Desired fov {0}, Current fov {1}, Speed boost value {2}, Min fov {3} Additional fov {4}, Fov diff {5}",
            desiredFov,
            cinemachineCamera.Lens.FieldOfView,
            forwardSpeedMultiplier.GetForwardSpeedMultiplier("ImmediateComboBoost").value - 1f,
            minFov,
            additionalFov,
            fovDiff
        );
    }
}
