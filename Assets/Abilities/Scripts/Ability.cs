using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Ability : MonoBehaviour
{
    public SplineTrack Track;
    [SerializeField] private CinemachineSplineCart _spline;
    [SerializeField] private GameObject _art;
    [SerializeField] private bool _isConected = true;

    [SerializeField] private float _speed;

    [SerializeField] private float _temporarryDurationVariable;

    void Start()
    {
        Destroy(gameObject, _temporarryDurationVariable);
    }

    public void ConfigurateMyself(float position, float XPosition)
    {
        if (_spline != null)
        {
            _spline.Spline = Track.track;
            _spline.SplinePosition = (position + 5f);
            _art.transform.localPosition = new Vector3(XPosition, 0f, 0f);
        }
        //_spline = GetComponent<CinemachineSplineCart>();
        //GetComponent<SplineContainer>();
        // _spline.Spline = Track.GetComponent<SplineContainer>();
    }

    void Update()
    {
        if (_spline.SplinePosition > Track.track.Spline.GetLength()-1 && _isConected)
        {
            _isConected = false;
            _spline.enabled = false;
        }

        if (!_isConected)
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
        }
    }
}
