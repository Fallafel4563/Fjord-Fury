using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Ability : MonoBehaviour
{
    [HideInInspector] public SplineTrack Track;
    [SerializeField] private CinemachineSplineCart _spline;
    [SerializeField] private GameObject _art;
    private bool _isConected = true;

    private float _offSplineSpeed = 140f;
    [SerializeField] private float _spawnOffset = 5f;
    [SerializeField] private float _temporarryDurationVariable;

    [SerializeField] private RamAbility RA;

    [SerializeField] int trickNumberRecuired;

    [SerializeField] private ObstacleLifetimeScalingSystem OLSS;



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
        RA = GetComponent<RamAbility>();
        Destroy(gameObject, _temporarryDurationVariable);
    }

    public void ConfigurateMyself(float position, float XPosition, Transform player, ForwardSpeedMultiplier forwardSpeedMultiplier, int comboCount, float strength)//, float speed)
    {
        _forwardSpeedMultiplier = forwardSpeedMultiplier;

        if (comboCount < trickNumberRecuired) return;

        if (_spline != null)
        {
            _spline.Spline = Track.track;
            _spline.SplinePosition = (position + _spawnOffset);
            _art.transform.localPosition = new Vector3(XPosition, 0f, 0f);
        }

        if (RA != null)
        {
            _spline.enabled = false;
            transform.rotation = player.rotation;
            transform.position = player.position;
            transform.SetParent(player);
            _art.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        //_spline = GetComponent<CinemachineSplineCart>();
        //GetComponent<SplineContainer>();
        // _spline.Spline = Track.GetComponent<SplineContainer>();

        SetStrenght(strength);
    }

    void SetStrenght(float strength)
    {
        Debug.Log(strength);

        //if (RA == null) _art.transform.localScale = new Vector3(strength, strength, strength);
        OLSS = GetComponentInChildren<ObstacleLifetimeScalingSystem>();
        OLSS.SetMaxSize(strength);
        _art.GetComponent<BounceShroom>().BouncePower *= strength;

        if (RA != null) SetRamStrength(strength);

        if (_spline.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed autoDolly) Debug.Log("Confermation");//autoDolly.Speed = 1f;// *= (strength / 2);
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
        else
        {
        }

        if (!_isConected)
        {
            transform.position += transform.forward * _offSplineSpeed * Time.deltaTime;
        }
    }
}
