using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private float jumpCameraYOffset = 0f;
    [SerializeField] private float groundCameraYOffset = 4f;
    [SerializeField] private Vector3 respawnOffset = new Vector3(0f, 4f, -7f);

    private Vector3 defaultPositionOffset;
    private Vector3 defaultPositionDamping;
    private CinemachinePositionComposer positionComposer;


    private void Start()
    {
        positionComposer = GetComponent<CinemachinePositionComposer>();
        defaultPositionOffset = positionComposer.TargetOffset;
        defaultPositionDamping = positionComposer.Damping;
    }


    public void OnLanded()
    {
        positionComposer.TargetOffset.y = groundCameraYOffset;
    }


    public void OnJumped()
    {
        positionComposer.TargetOffset.y = jumpCameraYOffset;
    }


    public void OnRespawnStart()
    {
        positionComposer.TargetOffset = respawnOffset;
        positionComposer.Damping = Vector3.zero;
        //TODO: Add lerping to the damping so that it's a bit smoother
    }


    public void OnRespawnEnd()
    {
        positionComposer.TargetOffset = defaultPositionOffset;
        positionComposer.Damping = defaultPositionDamping;
        //TODO: Add lerping to the damping so that it's a bit smoother
    }
}
