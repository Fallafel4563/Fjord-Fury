using UnityEngine;

public class LagSpikeSaftey : MonoBehaviour
{
    public LayerMask raycastLayer;

    private Vector3 oldPosition;


    private void Awake()
    {
        oldPosition = transform.position;
    }


    void Update()
    {
        Vector3 rayDirection = oldPosition - (transform.position + transform.up);
        RaycastHit raycastHit;
        Debug.DrawLine(oldPosition, transform.position + transform.up, Color.black, 60f);
        if (Physics.Raycast(oldPosition, -rayDirection.normalized, out raycastHit, rayDirection.magnitude, raycastLayer))
        {
            Debug.LogFormat("Hit {0}", raycastHit.collider.gameObject.name);
            transform.position = raycastHit.point;
        }

        oldPosition = transform.position + transform.up;
    }
}
