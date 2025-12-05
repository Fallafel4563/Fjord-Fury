using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Ability : MonoBehaviour
{
    public SplineTrack Track;
    [SerializeField] CinemachineSplineCart _spline;

    public void ConfigurateMyself(float position)
    {
        //_spline = GetComponent<CinemachineSplineCart>();
        _spline.Spline = Track.track;//GetComponent<SplineContainer>();
       // _spline.Spline = Track.GetComponent<SplineContainer>();
        _spline.SplinePosition = position;
    }
}
