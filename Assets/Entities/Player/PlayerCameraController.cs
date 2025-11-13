using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private float jumpCameraYOffset = 0f;
    [SerializeField] private float groundCameraYOffset = 4f;

    private CinemachineCamera cinemachineCamera;
    private CinemachinePositionComposer positionComposer;


    private void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        positionComposer = GetComponent<CinemachinePositionComposer>();
    }


    public void ApplyJumpOffset()
    {
        positionComposer.TargetOffset.y = jumpCameraYOffset;
    }


    public void ApplyGroundOffset()
    {
        positionComposer.TargetOffset.y = groundCameraYOffset;
    }


    public void ResetCamera()
    {
        ApplyGroundOffset();
        Transform trackTransform = cinemachineCamera.Target.TrackingTarget.transform;
        positionComposer.ForceCameraPosition(trackTransform.position, trackTransform.rotation);
    }
}
