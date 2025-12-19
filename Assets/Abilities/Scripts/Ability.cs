using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class Ability : MonoBehaviour
{
    public SplineTrack Track;
    [SerializeField] private CinemachineSplineCart _spline;
    [SerializeField] private ObstacleLifetimeScalingSystem obstacleLifetimeScaling;

    [Header("Art")]
    [SerializeField] private GameObject _art;

    [Header("Bounce")]

    [Header("Scaling")]
    [SerializeField] private float sizeBaseValue = 1f;
    [SerializeField] private float sizeDivider = 3f;
    [SerializeField] private float sizeMultiplier = 2f;
    [Space(10)]
    [SerializeField] private float lengthBaseValue = 1f;
    [SerializeField] private float lengthDivider = 3f;
    [SerializeField] private float lengthMultiplier = 2f;
    [Space(10)]
    [SerializeField] private float strengthBaseValue = 1f;
    [SerializeField] private float strengthDivider = 3f;
    [SerializeField] private float strengthMultiplier = 2f;

    private bool _isConected = true;

    [Header("Off spline")]
    [SerializeField] private float _offSplineSpeed = 140f;
    [SerializeField] private float _spawnOffset = 5f;


    private void Start()
    {
        Destroy(gameObject, 100f);
    }


    public void ConfigurateMyself(float position, float XPosition, Transform player, int lengthBoost, int sizeBoost, int strengthBoost, ForwardSpeedMultiplier forwardSpeedMultiplier, Ruber rubberband, PlayerObstacleCollisions playerObstacleCollisions)
    {
        float rubberbandBoost = rubberband.rubberbandBoost;
        float length = rubberbandBoost * (lengthBaseValue + (lengthMultiplier * (lengthBoost / (lengthDivider + (lengthBoost / 2f)))));
        float size = rubberbandBoost * (sizeBaseValue + (sizeMultiplier * (sizeBoost / (sizeDivider + (sizeBoost / 2f)))));
        float strength = rubberbandBoost * (strengthBaseValue + (strengthMultiplier * (strengthBoost / (strengthDivider + (strengthBoost / 2f)))));

        Debug.LogFormat(
            "Length mult {0}, Length input {1} \nSize mult {2}, Size input {3} \nStength mult {4}, Strength input {5} \n",
            length,
            lengthBoost,
            size,
            sizeBoost,
            strength,
            strengthBoost
        );

        // Set size and duration through the ObstacleLifetimeScalingSystem
        obstacleLifetimeScaling.LifeTime *= length;
        obstacleLifetimeScaling.MaxSize *= size;

        // This is also for the tornado
        if (GetComponentInChildren<BounceShroom>())
        {
            BounceShroom bounceShroom = GetComponentInChildren<BounceShroom>();
            bounceShroom.Owner = player;
            bounceShroom.BouncePower *= strength;
        }

        if (GetComponentInChildren<RamAbility>())
        {
            RamAbility ramAbility = GetComponentInChildren<RamAbility>();
            transform.SetParent(player);
            ramAbility.StartAbility(strength, forwardSpeedMultiplier, playerObstacleCollisions, obstacleLifetimeScaling.LifeTime, player);
        }


        _spline = GetComponent<CinemachineSplineCart>();
        _spline.Spline = Track.GetComponent<SplineContainer>();

        if (_spline != null)
        {
            if (Track != null) _spline.Spline = Track.track;
            _spline.SplinePosition = position + _spawnOffset;
            if (!GetComponentInChildren<RamAbility>()) _art.transform.localPosition = new Vector3(XPosition, 0f, 0f);
        }
    }


    void Update()
    {
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
