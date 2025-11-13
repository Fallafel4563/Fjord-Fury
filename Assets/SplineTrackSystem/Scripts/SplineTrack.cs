using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;


// Automatically adds the SplineExtrude and MeshCollider components to the game object when adding this component to said game object
[RequireComponent(typeof(SplineExtrude), typeof(MeshCollider))]
public class SplineTrack : MonoBehaviour
{
    [Header("Track settings")]
    public float width = 1f;
    [SerializeField] private float multiplier = 1f;
    // Sets how many segments per unit the spline extruder should have
    [SerializeField] private float segmentsPerUnit = 0.5f;
    [HideInInspector] public SplineContainer track;
    private SplineExtrude extruder;


    [Header("Grind Rail Settings")]
    public bool IsGrindRail;
    public float overrideSpeed;


    [Header("Spline Events")]
    // The GameObject referenecs should be set to the boat is entering/exiting/reaching the end of the track
    public UnityEvent<GameObject> OnBoatEnter;
    public UnityEvent<GameObject> OnBoatExit;
    public UnityEvent<GameObject> OnBoatReachedEnd;


    void Start()
    {
        //Get spline component and component to extrude spline mesh
        track = GetComponent<SplineContainer>();
        extruder = GetComponent<SplineExtrude>();
    }


    // This function is called when editing values in the inspector
    private void OnValidate()
    {
        // Get the spline extrude component
        extruder = GetComponent<SplineExtrude>();
        // Update the radius when editing the width or multiplier
        extruder.Radius = width * multiplier;
        extruder.SegmentsPerUnit = segmentsPerUnit;
        extruder.Rebuild();
    }


    public TrackDistanceInfo GetDistanceInfoFromPosition(Vector3 worldPos)
    {
        // Get the point on the track that is closes to worldPos
        SplineUtility.GetNearestPoint(track[0], worldPos - transform.position, out Unity.Mathematics.float3 nearestPos, out float distance);
        distance *= track[0].GetLength();

        TrackDistanceInfo trackDistanceInfo = new();
        trackDistanceInfo.distance = distance;
        trackDistanceInfo.nearestSplinePos = new Vector3(nearestPos.x, nearestPos.y, nearestPos.z) + transform.position;
        return trackDistanceInfo;
    }


}
    public struct TrackDistanceInfo
    {
        public float distance;
        public Vector3 nearestSplinePos;
    }
