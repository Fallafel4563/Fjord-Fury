using UnityEngine;

public class LagSpikeSaftey : MonoBehaviour
{
    public LayerMask raycastLayer;
    public PlayerMovement playerMovement;

    private Vector3 oldPosition;


    private void Awake()
    {
        oldPosition = transform.position;
    }


    void Update()
    {
        if (Time.deltaTime > 0.1f && playerMovement.airVelocity.magnitude > 10f)
        {
            Vector3 rayDirection = oldPosition - (transform.position + transform.up);
            RaycastHit raycastHit;
            if (Physics.Raycast(oldPosition, -rayDirection.normalized, out raycastHit, rayDirection.magnitude, raycastLayer))
            {
                playerMovement.airVelocity = Vector3.zero;
                transform.position = raycastHit.point;
            }
        }
        Debug.DrawLine(oldPosition, transform.position + transform.up, Color.black, 60f);
        oldPosition = transform.position + transform.up;
    }
}
