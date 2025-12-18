using UnityEngine;

public class RailAttractor : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SplineTrack splineTrack))
        {
            if (splineTrack.isCircle == true)
            {
                playerMovement.LandedOnTrack(splineTrack);
            }
        }
    }


    private void FixedUpdate()
    {
        transform.position = playerMovement.transform.position;
    }
}
