using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using Unity.Mathematics;
using System;

public class SplineTrackDistance : MonoBehaviour
{
    private SplineContainer[] splineContainer;
    public Transform Player;
    private float3 pointOnSpline;
    public float coordinateDelay;
   
    void Start()
    {
        StartCoroutine(searchForSpline());
        {
            SplineContainer trackerSpline = GameObject.FindWithTag("Distance Tracker").GetComponent<SplineContainer>();
            splineContainer = new SplineContainer[1];
            splineContainer[0] = trackerSpline;
        }

        StartCoroutine(timedCoordinates());
    }

    void Update()
    {
        if (splineContainer == null || Player == null)
        {
            Debug.Log("No Spline and/or Player!.");
            return;
        }

        SplineUtility.GetNearestPoint(splineContainer[0].Spline, Player.position-splineContainer[0].transform.position, out float3 nearestPointOnSpline, out float t);
        Vector3 offset = splineContainer[0].transform.position;
        pointOnSpline = nearestPointOnSpline + new float3(offset.x,offset.y,offset.z) ;
        Debug.DrawLine(Player.position, pointOnSpline, Color.red);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere((Vector3)pointOnSpline, 0.5f);
    }

    private IEnumerator searchForSpline()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Searching for Spline");
    }

    private IEnumerator timedCoordinates()
    {
        while (true) // Loop
        {
            yield return new WaitForSeconds(coordinateDelay);
            Debug.Log("Coordinates" + pointOnSpline);
        }
    }

}