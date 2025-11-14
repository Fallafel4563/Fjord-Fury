using UnityEngine;
using UnityEngine.Splines;

using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraSplineButtonController : MonoBehaviour
{
    [Header("References")]
    public CinemachineSplineCart splineCart;    // Drag your CinemachineSplineCart
    public SplineContainer splineContainer;     // Drag the SplineContainer here

    [Header("Settings")]
    public float moveSpeed = 5f;
    public bool loop = false;
    public int knotCount = 4;
    private int currentKnot = 0;
    private int targetKnot = 0;
    private float currentSpeed = 0;
    private bool moving = false;
    private List<float> knotDistances = new List<float>();

    void Start()
    {
        if (splineCart == null || splineContainer == null)
        {
            Debug.LogError("Please assign CinemachineSplineCart and SplineContainer!");
            enabled = false;
            return;
        }

        splineCart.Spline = splineContainer;
        //PrecomputeKnotDistances();

        currentKnot = 0;
        targetKnot = 0;
        splineCart.SplinePosition = 0; //starts at knot 0
    }

    void Update()
    {
        if (!moving) return;

        //checks distance to goal
        if (Mathf.Abs(splineCart.SplinePosition - targetKnot) < 0.05f)
        {
            splineCart.SplinePosition = targetKnot;
            currentKnot = targetKnot;
            moving = false;

            //stops moving
            if (splineCart.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
            {
                autoDolly.Speed = 0;
                currentSpeed = 0;
            }
        }
    }

    public void MoveRight()
    {
        if (moving) return;

        //starts moving right
        if (splineCart.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
        {
            autoDolly.Speed = moveSpeed;
            currentSpeed = moveSpeed;
        }

        if (currentKnot >= knotCount - 1)
        {
            if (loop) targetKnot = 0;
            else return;
        }
        else
        {
            targetKnot = currentKnot + 1;
        }

        moving = true;
    }

    public void MoveLeft()
    {
        if (moving) return;

        //starts moving left
        if (splineCart.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
        {
            autoDolly.Speed = -moveSpeed;
            currentSpeed = -moveSpeed;
        }


        if (currentKnot <= 0)
        {
            if (loop) targetKnot = knotCount - 1;
            else return;
        }
        else
        {
            targetKnot = currentKnot - 1;
        }

        moving = true;
    }
}
