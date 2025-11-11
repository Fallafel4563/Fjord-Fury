using UnityEngine;

public class ObstacleMoveAB : MonoBehaviour
{
    public Transform pointA;  // Starting point
    public Transform pointB;  // Ending point
    public float speed = 3f;  // Movement speed

    private Vector3 target;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Please assign Point A and Point B Transforms in the inspector.");
            enabled = false;
            return;
        }

        transform.position = pointA.position;
        target = pointB.position;
    }

    void Update()
    {
        // Move towards the target point
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // If close to the target, switch target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }
}