using Unity.Cinemachine;
using UnityEngine;

public class CurveSpeedOffset : MonoBehaviour
{
    public float distanceOffset = 2f;
    public CinemachineSplineCart splineCart;
    public PlayerMovement playerMovement;
    public ForwardSpeedMultiplier forwardSpeedMultiplier;
    public GameObject sphere;


    private Vector3 currentPoint;
    private Vector3 nextPoint;
    private Vector3 previousPoint;


    private void Update()
    {
        // Don't calculate stuff when the player is in the air or when the track is a circle track
        if (!playerMovement.isGrounded || playerMovement.currentTrack.isCircle)
            return;
        
        SplineTrack currentTrack = splineCart.Spline.GetComponent<SplineTrack>();
        TrackDistanceInfo currentDistance = currentTrack.GetDistanceInfoFromPosition(splineCart.transform.position);

        // Get current point
        currentPoint = currentDistance.nearestSplinePos;

        // Get next point
        Vector3 posInFront = splineCart.transform.position + splineCart.transform.forward * distanceOffset;
        nextPoint = currentTrack.GetDistanceInfoFromPosition(posInFront).nearestSplinePos;

        // Get previous point
        Vector3 posBehind = splineCart.transform.position + splineCart.transform.forward * -distanceOffset;
        previousPoint = currentTrack.GetDistanceInfoFromPosition(posBehind).nearestSplinePos;

        // Callculate circle center and radius form current, next and previous point
        Vector3 circleCenter = GetCircleCenterPos();
        // Set the forward speed multiplier to be 1f when the value form the circle calculations are invalid
        if (circleCenter == Vector3.zero)
        {
            forwardSpeedMultiplier.SetForwardSpeedMultiplier("Track Position Multiplier", 1f);
            return;
        }

        float splineCartRadius = Vector3.Distance(splineCart.transform.position, circleCenter);
        float boatRadius = Vector3.Distance(transform.position, circleCenter);
        float speedOffsetMultiplier = splineCartRadius / boatRadius;

        Debug.Log(speedOffsetMultiplier);

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Track Position Multiplier", speedOffsetMultiplier);

        // Set position and scale of debug sphere
        if (sphere)
        {
            sphere.transform.position = circleCenter;
            sphere.transform.localScale = Vector3.one * splineCartRadius * 2f;
        }
    }


    private Vector3 GetCircleCenterPos()
    {
        // CREDITS: NWin - https://discussions.unity.com/t/how-to-make-a-circle-with-three-known-points/226343/3
        // I have no idea how this works (Treike)
        Vector3 v1 = currentPoint - previousPoint;
        Vector3 v2 = nextPoint - previousPoint;

        Debug.LogFormat("v1 {0}, v2 {1}", v1, v2);

        float v1v1 = Vector3.Dot(v1, v1);
        float v2v2 = Vector3.Dot(v2, v2);
        float v1v2 = Vector3.Dot(v1, v2);

        Debug.LogFormat("v1v1 {0}, v2v2 {1}, v1v2 {2}", v1v1, v2v2, v1v2);

        float b = 0.5f / (v1v1 * v2v2 - v1v2 * v1v2);
        float k1 = b * v2v2 * (v1v1 - v1v2);
        float k2 = b * v1v1 * (v2v2 - v1v2);

        // Exit early if the values can't be used because they're NaN or when they're too large
        if (float.IsNaN(b) || float.IsInfinity(b) || Mathf.Abs(b) > 1000f)
            return Vector3.zero;

        Debug.LogFormat("b {0}, k1 {1}, k2 {2}", b, k1, k2);

        Vector3 circleCenter = previousPoint + v1 * k1 + v2 * k2;
        return circleCenter;
    }
}
