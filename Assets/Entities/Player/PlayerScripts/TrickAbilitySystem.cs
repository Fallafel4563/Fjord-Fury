using UnityEngine;
using Unity.Cinemachine;

public class TrickAbilitySystem : MonoBehaviour
{
    [SerializeField] private GameObject[] abilityPrefabs;
    [SerializeField] private GameObject[] abilityFailedPrefabs;
    [SerializeField] private Transform AbilitySpawnPoint;
    [SerializeField] private Transform ramSpawnPoint;
    [SerializeField] private CinemachineSplineCart splineCart;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Ruber rubberbandBoost;
    [SerializeField] private ForwardSpeedMultiplier forwardSpeedMultiplier;
    [SerializeField] private PlayerObstacleCollisions playerObstacleCollisions;

    private GameObject abilityBuffer;


    public void SpawnAbility(int firstTrick, int lengthBoost, int sizeBoost, int strengthoost)
    {
        // Spawn the shroom and tornado above the player so that they don't collide with the palyer during the first frame they spawn
        Transform spawnTransform = firstTrick == 2 ? ramSpawnPoint : AbilitySpawnPoint;

        abilityBuffer = Instantiate(abilityPrefabs[firstTrick], spawnTransform.position, spawnTransform.rotation);

        Ability ability = abilityBuffer.GetComponent<Ability>();
        ability.Track = playerMovement.mainTrack;
        ability.ConfigurateMyself(splineCart.SplinePosition, transform.localPosition.x, transform, lengthBoost, sizeBoost, strengthoost, forwardSpeedMultiplier, rubberbandBoost, playerObstacleCollisions);
    }


    public void SpawnAbilityFailed(int firstTrick)
    {
        Transform spawnTransform = firstTrick == 2 ? ramSpawnPoint : AbilitySpawnPoint;

        abilityBuffer = Instantiate(abilityFailedPrefabs[firstTrick], spawnTransform.position, spawnTransform.rotation);
    }
}
