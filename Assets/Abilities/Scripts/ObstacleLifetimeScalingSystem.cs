using System.Linq;
using UnityEngine;

public class ObstacleLifetimeScalingSystem : MonoBehaviour
{
    public float MaxSize;
    public float LifeTime;

    [SerializeField] private GameObject Parent;
    [SerializeField] private AnimationCurve ScalingCurve;

    private float animationTime;


    public void SetMaxSize(float maxSize)
    {
        MaxSize = maxSize;
        //Debug.Log("Set MAx Size");
    }


    void LateUpdate()
    {
        Scaling();

        // Set the time based on the scalingCurve and the Lifetime
        animationTime += Time.deltaTime / LifeTime;

        //Destroy parent if the ScalingCurve has reached its end
        if (animationTime > ScalingCurve.keys.Last().time)
            Destroy(Parent);
    }


    //Set the new size of the art based on the ScalingCurve
    void Scaling()
    {
        float size = MaxSize * ScalingCurve.Evaluate(animationTime);
        transform.localScale = new Vector3(size, size, size);
    }
}
