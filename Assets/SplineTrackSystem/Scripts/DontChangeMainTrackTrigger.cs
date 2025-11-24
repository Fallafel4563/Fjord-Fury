using Unity.Cinemachine;
using UnityEngine;
// This script makes the boat not change main track when it's grounded and inside the trigger zone (box collider)
// Branching tracks have to placed under the main track (around 0.5 units) for it to also work when the boat is jumping
// If braching tracks are above the main track, then the boat will follow the branch track when landing from a jump (but only when inside the trigger)
// If the boat isn't inside the trigger and it lands on a place with overlapping tracks, then it will follow the last track it hits (usually the one at the bottom)
// So if the main track is above branching tracks then the boat will only follow the main track while it's inside the trigger, but if the boat is outside the
// trigger then the boat will follow the branch track.


[RequireComponent(typeof(BoxCollider), typeof(CinemachineSplineCart))]
public class DontChangeMainTrackTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.dontChangeMainTrack = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.dontChangeMainTrack = false;
        }
    }
}
