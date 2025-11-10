using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineExtrude))]
public class SplineTrack : MonoBehaviour
{
        [Header("Grind Rail Settings")]
    public bool IsGrindRail;
    public float overrideSpeed;

        [Header("Spline Events")]
    public UnityEvent OnBoatEnter;
    public UnityEvent OnBoatExit;

    [NonSerialized] public SplineContainer track;
    [SerializeField] private SplineExtrude extruder;

        [Header("idk")]
    public bool WidthGizmo = true;
    public float width, multiplier = 1.0f;
    Vector3 lastSplinePos = Vector3.zero, lastPlayerPos = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get spline component and component to extrude spline mesh
        track = GetComponent<SplineContainer>();
        extruder = GetComponent<SplineExtrude>();
    }

    // Update is called once per frame
    void Update()
    {
        //extruder.Radius = width* multiplier;
        //extruder.Rebuild();
    }


    private void OnValidate()
    {
        if (extruder != null)
        {
            extruder.Radius = width * multiplier;
            extruder.Rebuild();
        }
    }
    
            //Debug Gizoms
    /*
    private void OnDrawGizmosSelected()
    {
        if(WidthGizmo)
        {
            Vector3 start= extruder.Spline.EvaluatePosition(0);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(start+transform.position, width);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastSplinePos, 1);
        Gizmos.color= Color.blue;
        Gizmos.DrawSphere(lastPlayerPos, 10);
        Gizmos.color=(Color.white);
        Gizmos.DrawLine(lastSplinePos, lastPlayerPos);
    }
    */

    //Snap player to closest spline 
    public EvalInfo EvaluateBasedOnWorldPosition(Vector3 WorldPos)
    {
        lastPlayerPos = WorldPos;
        
        SplineUtility.GetNearestPoint(Track[0], WorldPos - transform.position, out Unity.Mathematics.float3 nearest, out float distance);
        print("T: " + distance.ToString());
        distance *= Track[0].GetLength();

        EvalInfo EvalInfo = new EvalInfo();
        EvalInfo.distance = distance;
        EvalInfo.SplinePos = new Vector3(nearest.x, nearest.y, nearest.z) + transform.position;
        lastSplinePos = EvalInfo.SplinePos;
        print("LAST SPLINE POS: " + lastSplinePos.ToString());
        return EvalInfo;
    }


}
    public struct EvalInfo
    {
        public float distance;
        public Vector3 SplinePos;
    }
