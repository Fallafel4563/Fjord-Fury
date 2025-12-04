using UnityEngine;

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

    [Header("Strength dividers")]
    [SerializeField] float DurationDivider;
    [SerializeField] float SizeDivider;
    [SerializeField] float SpeedDivider;

    void Start()
    {
        PM = GetComponent<PlayerMovement>();
        FSM = GetComponent<ForwardSpeedMultiplier>();
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown("x"))
        {
            SpawnAbility();
            Debug.Log("ability");
        }
        */
    }

    public void OnPlayerLand()
    {
        if (!abilityHasSpawned && FirstTrickIndex != 0)
        {
            SpawnAbility();
        }
    }

    public void SpawnAbility()
    {
        abilityHasSpawned = true;
        abilityBuffer = Instantiate(abilityPrefabs[FirstTrickIndex], AbilitySpawnPoint.position, AbilitySpawnPoint.rotation);

        combinedStrength = 0;

        combinedStrength += (ShortBoost * 1);
        combinedStrength += (MediumBoost * 2);
        combinedStrength += (LongBoost * 3);

        float newDuration = combinedStrength / DurationDivider;
        float newSize = combinedStrength / SizeDivider;
        float newSpeed = combinedStrength / SpeedDivider;

        switch (FirstTrickIndex)
        {
            case 1:
                // Fireball speed boost
                /// FSM.SetForwardSpeedMultiplier("name", newSpeed, new SpeedMultiplierCurve());
                /// abilityBuffer.transform.localScale = new Vector3(transform.localScale.x * newSize, transform.localScale.y * newSize, transform.localScale.z * newSize);
                /// abilityBuffer.GetComponent<BounceShroom>();
                break;

            case 2:
                // Tornado
                break;

            case 3:
                // Bounce shroom
                break;
        }

        abilityTimeLeft = abilityDuration / newDuration;
    }

    public void SpawnAbilityFailed()
    {
        abilityHasSpawned = true;
        abilityBuffer = Instantiate(abilityFailedPrefabs[FirstTrickIndex], AbilitySpawnPoint.position, AbilitySpawnPoint.rotation);
        abilityTimeLeft = abilityDuration;
    }

    void DespawnAbility()
    {
        Destroy(abilityBuffer);
        abilityHasSpawned = false;
        FirstTrickIndex = 0;
    }
}
