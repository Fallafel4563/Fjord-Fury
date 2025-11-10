using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;


public class SplineTrack : MonoBehaviour
{
        [Header("Grind Rail Settings")]
    public float overrideSpeed;
    public bool IsGrindRail;

        [Header("Spline Events")]
    public UnityEvent OnBoatEnter;
    public UnityEvent OnBoatExit;

    [NonSerialized] public SplineContainer Track;
    private SplineExtrude extruder;

        [Header("idk")]
    public bool WidthGizmo = true;
    public float width, multiplier;
    Vector3 lastSplinePos = Vector3.zero, lastPlayerPos = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get spline component and component to extrude spline mesh
        Track = GetComponent<SplineContainer>();
        extruder = GetComponent<SplineExtrude>();
    }

    // Update is called once per frame
    void Update()
    {
        extruder.Radius = width* multiplier;
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
        
        SplineUtility.GetNearestPoint(Track[0], WorldPos - transform.position, out Unity.Mathematics.float3 nearest, out float t);
        print("T: " + t.ToString());
        t *= Track[0].GetLength();

        EvalInfo EvalInfo = new EvalInfo();
        EvalInfo.t = t;
        EvalInfo.SplinePos = new Vector3(nearest.x, nearest.y, nearest.z) + transform.position;
        lastSplinePos = EvalInfo.SplinePos;
        print("LAST SPLINE POS: " + lastSplinePos.ToString());
        return EvalInfo;
    }


}
    public struct EvalInfo
    {
        public float t;
        public Vector3 SplinePos;
    }
