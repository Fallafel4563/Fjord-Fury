using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Ability : MonoBehaviour
{
    public SplineTrack Track;
    [SerializeField] private CinemachineSplineCart _spline;
    [SerializeField] private GameObject _art;
    [SerializeField] private GameObject _artParticles;
    private bool _isConected = true;

    private float _offSplineSpeed = 140f;
    [SerializeField] private float _spawnOffset = 5f;
    [SerializeField] private float _temporarryDurationVariable;

    [SerializeField] private RamAbility RA;

    [SerializeField] private ObstacleLifetimeScalingSystem OLSS;
    [SerializeField] private Obstacle O;



    [Header("Ability implementaation")]
    [SerializeField] private float CrashSpeedMultiplier;
    [SerializeField] private AnimationCurve CrashSpeedMultiplierCurve;
    [SerializeField] private AudioSource CrashSound;
    [SerializeField] private bool CauseHarm;
    [SerializeField] private bool DestructOnCrash;
    [SerializeField] private GameObject DestructParticles;
    [SerializeField] private GameObject Instantiator;
    [SerializeField] private float BounceHeight;

    [SerializeField] private float MaxSize;
    [SerializeField] private float LifeSpan;
    [SerializeField] private AnimationCurve MultiplierCurve;

    ForwardSpeedMultiplier _forwardSpeedMultiplier;

    void Start()
    {
        OLSS = GetComponentInChildren<ObstacleLifetimeScalingSystem>();
        O = GetComponentInChildren<Obstacle>();
        RA = GetComponent<RamAbility>();
        Destroy(gameObject, _temporarryDurationVariable);
    }

    public void ConfigurateMyself(float position, float XPosition, Transform player, int shortBoost/*Longer*/, int mediumBoost/*Bigger*/, int longBoost/*Stronger*/)
    {
        _spline = GetComponent<CinemachineSplineCart>();
        _spline.Spline = Track.GetComponent<SplineContainer>();

        // Set strength through the ObstacleLifetimeScalingSystem
        OLSS.LifeTime = shortBoost + 2;
        OLSS.MaxSize = mediumBoost + 1;
        //O.bounceHeight = longBoost + 1;

        if (GetComponentInChildren<BounceShroom>()) GetComponentInChildren<BounceShroom>().BouncePower *= longBoost + 1;

        if (_spline != null)
        {
            if (Track != null) _spline.Spline = Track.track;
            _spline.SplinePosition = (position + _spawnOffset);
            _art.transform.localPosition = new Vector3(XPosition, 0f, 0f);
        }
    }

    void SetStrenght(float strength)
    {
        Debug.Log(strength);

        if (_artParticles != null) _artParticles.transform.localScale = new Vector3(strength, strength, strength);
        OLSS = GetComponentInChildren<ObstacleLifetimeScalingSystem>();
        OLSS.SetMaxSize(strength);
        if (_art.GetComponent<BounceShroom>()) _art.GetComponent<BounceShroom>().BouncePower *= 1.6f + (strength * 1.225f);

        if (RA != null) SetRamStrength(strength);

        if (_spline.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly)
            Debug.Log("Confermation");//autoDolly.Speed = 1f;// *= (strength / 2);
    }

    void SetRamStrength(float strength)
    {
        RA.StartAbility(strength, _forwardSpeedMultiplier);
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
            transform.position += transform.forward * _offSplineSpeed * Time.deltaTime;
        }
    }
}
