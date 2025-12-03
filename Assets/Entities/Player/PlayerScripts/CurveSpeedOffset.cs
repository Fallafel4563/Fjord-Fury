using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class CurveSpeedOffset : MonoBehaviour
{
    public float distanceOffset = 2f;
    public CinemachineSplineCart splineCart;
    public ForwardSpeedMultiplier forwardSpeedMultiplier;
    public GameObject sphere;


    private Vector3 currentPoint;
    private Vector3 nextPoint;
    private Vector3 previousPoint;


    private List<GameObject> debugObjects = new();


    private void Start()
    {
        GameObject newGameObject = new();
        debugObjects.Add(Instantiate(newGameObject));
        debugObjects.Add(Instantiate(newGameObject));
        debugObjects.Add(Instantiate(newGameObject));
    }


    private void Update()
    {
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


        // Set the positions of the debug objects
        debugObjects[0].transform.position = currentPoint;
        debugObjects[1].transform.position = nextPoint;
        debugObjects[2].transform.position = previousPoint;
        Debug.LogFormat("Current {0}, Next {1}, Prev {2}", currentPoint, nextPoint, previousPoint);

        // Callculate circle center and radius form current, next and previous point
        Vector3 circleCenter = GetCircleCenterPos();

        float splineCartRadius = Vector3.Distance(splineCart.transform.position, circleCenter);
        float boatRadius = Vector3.Distance(transform.position, circleCenter);
        float speedOffsetMultiplier = splineCartRadius / boatRadius;

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
        Vector3 v1 = currentPoint - previousPoint;
        Vector3 v2 = nextPoint - previousPoint;

        float v1v1 = Vector3.Dot(v1, v1);
        float v2v2 = Vector3.Dot(v2, v2);
        float v1v2 = Vector3.Dot(v1, v2);

        float b = 0.5f / (v1v1 * v2v2 - v1v2 * v1v2);
        float k1 = b * v2v2 * (v1v1 - v1v2);
        float k2 = b * v1v1 * (v2v2 - v1v2);
        Vector3 circleCenter = previousPoint + v1 * k1 + v2 * k2;
        return circleCenter;
    }
}
