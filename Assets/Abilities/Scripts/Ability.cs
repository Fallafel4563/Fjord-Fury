using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Ability : MonoBehaviour
{
    public SplineTrack Track;
    [SerializeField] private CinemachineSplineCart _spline;
    [SerializeField] private RamAbility RA;
    [SerializeField] private ObstacleLifetimeScalingSystem OLSS;

    [Header("Art")]
    [SerializeField] private GameObject _art;
    [SerializeField] private GameObject _artParticles;

    [Header("Equalizer")]
    [SerializeField] private float EqualizerValue;
    [SerializeField] private AnimationCurve EqualizerCurve;
    [SerializeField] private float strengthDivider;

    private float LeadSplinePosition;
    private float OwnSplinePosition;

    // EqualizerCurve.Evaluate(EqualizerValue * (1 + (shortBoost / strengthDivider)));
    // EqualizerValue = LeadSplinePosition � OwnSplinePosition;

    private bool _isConected = true;

    private float _offSplineSpeed = 140f;
    [SerializeField] private float _spawnOffset = 5f;
    [SerializeField] private float _temporarryDurationVariable;

    Transform owner;

    void Start()
    {
        OLSS = GetComponentInChildren<ObstacleLifetimeScalingSystem>();
        RA = GetComponent<RamAbility>();
        Destroy(gameObject, _temporarryDurationVariable);
    }

    public void ConfigurateMyself(float position, float XPosition, Transform player, int shortBoost/*Longer*/, int mediumBoost/*Bigger*/, int longBoost/*Stronger*/)
    {
        owner = player;

        transform.SetParent(owner);
        _spline = GetComponent<CinemachineSplineCart>();
        _spline.Spline = Track.GetComponent<SplineContainer>();

        // Set strength through the ObstacleLifetimeScalingSystem
        OLSS.LifeTime = shortBoost + 2;
        OLSS.MaxSize = mediumBoost + 1;

        if (GetComponentInChildren<BounceShroom>())
        {
            GetComponentInChildren<BounceShroom>().BouncePower *= longBoost + 1;
            GetComponentInChildren<BounceShroom>().Owner = owner;
        }

        if (_spline != null)
        {
            if (Track != null) _spline.Spline = Track.track;
            _spline.SplinePosition = (position + _spawnOffset);
            if (!GetComponentInChildren<RamAbility>()) _art.transform.localPosition = new Vector3(XPosition, 0f, 0f);
        }
    }

    void Update()
    {
        if (_spline.SplinePosition > Track.track.Spline.GetLength()-1 && _isConected)
        {
            _isConected = false;
            _spline.enabled = false;
            transform.SetParent(null);
        }

        if (!_isConected)
        {
            transform.position += transform.forward * _offSplineSpeed * Time.deltaTime;
        }
    }
}
