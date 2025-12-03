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

    void Start()
    {
        PM = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!abilityHasSpawned && PM.isGrounded) Debug.Log("ability");
        if (!abilityHasSpawned && FirstTrickIndex != 0 && PM.isGrounded) SpawnAbility();
    }

    public void SpawnAbility()
    {
        abilityHasSpawned = true;
        abilityBuffer = Instantiate(abilityPrefabs[FirstTrickIndex], AbilitySpawnPoint.position, AbilitySpawnPoint.rotation);
        abilityTimeLeft = abilityDuration;

        SetAbilityStrength();
    }

    public void SpawnAbilityFailed()
    {
        abilityHasSpawned = true;
        abilityBuffer = Instantiate(abilityFailedPrefabs[FirstTrickIndex], AbilitySpawnPoint.position, AbilitySpawnPoint.rotation);
        abilityTimeLeft = abilityDuration;
    }

    void SetAbilityStrength()
    {
        int combinedStrength = 0;

        combinedStrength += (ShortBoost * 1);
        combinedStrength += (MediumBoost * 2);
        combinedStrength += (LongBoost * 3);

        switch (FirstTrickIndex)
        {
            case 1:
                // Fireball speed boost
                break;

            case 2:
                // Tornado
                break;

            case 3:
                // Bounce shroom
                break;
        }
    }

    void DespawnAbility()
    {
        Destroy(abilityBuffer);
        abilityHasSpawned = false;
        FirstTrickIndex = 0;
    }
}
