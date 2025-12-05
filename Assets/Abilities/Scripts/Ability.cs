using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Ability : MonoBehaviour
{
    public SplineTrack Track;
    [SerializeField] private CinemachineSplineCart _spline;
    [SerializeField] private GameObject _art;
    [SerializeField] bool _isConected = true;

    [SerializeField] float _speed;

    [SerializeField] float _temporarryDurationVariable;

    void Start()
    {
        Destroy(gameObject, _temporarryDurationVariable);
    }

    public void ConfigurateMyself(float position, float XPosition)
    {
        //_spline = GetComponent<CinemachineSplineCart>();
        _spline.Spline = Track.track;//GetComponent<SplineContainer>();
       // _spline.Spline = Track.GetComponent<SplineContainer>();
        _spline.SplinePosition = (position + 5f);
        _art.transform.localPosition = new Vector3(XPosition, 0f, 0f);
    }

    void Update()
    {
        if (_spline.SplinePosition > Track.track.Spline.GetLength()-1 && _isConected)
        {
            _isConected = false;
            //Detach function
            _spline.enabled = false;
        }

        if (!_isConected)
        {
            Debug.Log("It worked");
            transform.position += transform.forward * _speed * Time.deltaTime;
        }
    }
}
