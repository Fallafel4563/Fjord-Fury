using UnityEngine;
using Unity.Cinemachine;

public class TrickAbilitySystem : MonoBehaviour
{
    [SerializeField] private GameObject[] abilityPrefabs;
    [SerializeField] private GameObject[] abilityFailedPrefabs;
    [SerializeField] private Transform AbilitySpawnPoint;

    private bool abilityHasSpawned;
    private GameObject abilityBuffer;
    private PlayerMovement PM;

    [SerializeField] private float abilityDuration;
    private float abilityTimeLeft;

    [HideInInspector] public int FirstTrickIndex;
    [HideInInspector] public int ShortBoost;
    [HideInInspector] public int MediumBoost;
    [HideInInspector] public int LongBoost;

    private int combinedStrength;
    private ForwardSpeedMultiplier FSM;

    [SerializeField] private CinemachineSplineCart splineCart;

    [Header("Strength dividers")]
    [SerializeField] private float DurationDivider;
    [SerializeField] private float SizeDivider;
    [SerializeField] private float SpeedDivider;

    void Start()
    {
        PM = GetComponent<PlayerMovement>();
        FSM = GetComponent<ForwardSpeedMultiplier>();
    }

    public void SpawnAbility(int firstTrick, int shortBoost, int mediumBoost, int longBoost)
    {
        Debug.Log("SpawnAbility " + firstTrick);

        abilityHasSpawned = true;
        int comboCount = 0;
        combinedStrength = 0;

        combinedStrength += (shortBoost * 1);
        combinedStrength += (mediumBoost * 2);
        combinedStrength += (longBoost * 3);

        comboCount += shortBoost;
        comboCount += mediumBoost;
        comboCount += longBoost;

        abilityBuffer = Instantiate(abilityPrefabs[firstTrick - 1], AbilitySpawnPoint.position, AbilitySpawnPoint.rotation);

        float newDuration = combinedStrength / DurationDivider;
        float newSize = combinedStrength / SizeDivider;
        float newSpeed = combinedStrength / SpeedDivider;

        // Set up the abilityBuffer ref
        ConfigureAbility(abilityBuffer, comboCount, combinedStrength);
        abilityTimeLeft = abilityDuration / newDuration;
    }

    // Supply the ability with all data of where it's suppost to spawn
    void ConfigureAbility(GameObject buffer, int comboCount, float strength)
    {
        Ability a = abilityBuffer.GetComponent<Ability>();
        a.Track = PM.mainTrack;

        a.ConfigurateMyself(splineCart.SplinePosition, transform.localPosition.x, transform, GetComponent<ForwardSpeedMultiplier>(), comboCount, strength);
        abilityBuffer.GetComponentInChildren<Obstacle>().owner = this.transform;
    }

    public void SpawnAbilityFailed(int firstTrick)
    {
        abilityHasSpawned = true;
        abilityBuffer = Instantiate(abilityFailedPrefabs[firstTrick], AbilitySpawnPoint.position, AbilitySpawnPoint.rotation);
        abilityTimeLeft = abilityDuration;
    }

    void DespawnAbility()
    {
        Destroy(abilityBuffer);
        abilityHasSpawned = false;
        FirstTrickIndex = 0;
    }
}
