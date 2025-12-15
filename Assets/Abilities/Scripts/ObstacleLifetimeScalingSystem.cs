using UnityEngine;

public class ObstacleLifetimeScalingSystem : MonoBehaviour
{
    public float MaxSize;
    [SerializeField] private AnimationCurve ScalingCurve;
    [SerializeField] private GameObject Parent;
    float animationTime;

    public void SetMaxSize(float maxSize)
    {
        MaxSize = maxSize;
        Debug.Log("Set MAx Size");
    }

    void Update()
    {
        animationTime += Time.deltaTime;
        Scaling();

        //Destroy parent if the ScalingCurve has reached its end
        if (animationTime > ScalingCurve.length) Destroy(Parent);
    }

    //Set the new size of the art based on the ScalingCurve
    void Scaling()
    {
        float size = MaxSize * ScalingCurve.Evaluate(animationTime);
        transform.localScale = new Vector3(size, size, size);
    }
}
