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

    [Header("Bounce")]
    [SerializeField] private float BounceMultiplier = 2f;
    [SerializeField] private float shroomStrengthDivider;

    private float LeadSplinePosition;
    private float OwnSplinePosition;

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

    public void ConfigurateMyself(float position, float XPosition, Transform player, int shortBoost/*Longer*/, int mediumBoost/*Bigger*/, int longBoost/*Stronger*/, ForwardSpeedMultiplier forwardSpeedMultiplier, Ruber ruber)
    {
        owner = player;

        if (!GetComponentInChildren<BounceShroom>()) transform.SetParent(owner);
        _spline = GetComponent<CinemachineSplineCart>();
        _spline.Spline = Track.GetComponent<SplineContainer>();

        // Set strength through the ObstacleLifetimeScalingSystem
        OLSS.LifeTime = Equalizer(shortBoost * 2, ruber);
        OLSS.MaxSize = Equalizer(mediumBoost, ruber);

        if (GetComponentInChildren<BounceShroom>())
        {
            
            int strengthBoost = longBoost; // it has to be a float in the formula
            //GetComponentInChildren<BounceShroom>().BouncePower *= Equalizer(longBoost);
            GetComponentInChildren<BounceShroom>().Owner = owner;
            float BounceStrength = ruber.rubberbandBoost * (10f + (BounceMultiplier * (strengthBoost / (shroomStrengthDivider + (strengthBoost / 2)))));  
            Debug.Log("Bounce: " + BounceStrength);
            //GetComponentInChildren<BounceShroom>().BouncePower = BounceStrength;
        }

        if (GetComponent<RamAbility>())
        {
            GetComponent<RamAbility>().StartAbility(Equalizer(longBoost, ruber), forwardSpeedMultiplier);
        }

        if (_spline != null)
        {
            if (Track != null) _spline.Spline = Track.track;
            _spline.SplinePosition = (position + _spawnOffset);
            if (!GetComponentInChildren<RamAbility>()) _art.transform.localPosition = new Vector3(XPosition, 0f, 0f);
        }
    }

    void GetEqualizerValue()
    {
        LeadSplinePosition = 2f;
        OwnSplinePosition = 1f;
        EqualizerValue = LeadSplinePosition - OwnSplinePosition;
    }

    float Equalizer(int input, Ruber ruber)
    {
        float returnValue = input + 1;

        returnValue = ruber.rubberbandBoost * (1 + (input / strengthDivider));
        if (input == 0) returnValue = 1;
        if (returnValue == 0)
        {
            returnValue = 1;
            Debug.Log("Returned 0");
        }

        Debug.Log(ruber.rubberbandBoost);
        //returnValue /= ruber.rubberbandBoost;

        return returnValue;
    }

    void Update()
    {
        GetEqualizerValue();

        if (_spline.SplinePosition > Track.track.Spline.GetLength() - 1 && _isConected)
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
