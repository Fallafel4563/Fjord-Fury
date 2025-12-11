using UnityEngine;

public class ObstacleLifetimeScalingSystem : MonoBehaviour
{
    public float MaxSize;
    [SerializeField] private AnimationCurve ScalingCurve;

    float animationTime;

    public void SetMaxSize(float maxSize)
    {
        MaxSize = maxSize;
        Debug.Log("Set MAx Size");
    }

    void Update()
    {
        animationTime += Time.deltaTime;
        function();

        if (animationTime > ScalingCurve.length) Destroy(gameObject);
    }

    void function()
    {
        float size = MaxSize * ScalingCurve.Evaluate(animationTime);
        transform.localScale = new Vector3(size, size, size);
    }
}
