using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    //public Transform pointA;  // Starting point
    //public Transform pointB;  // Ending point
    public float speed = 3f;  // Movement speed

    public Transform[] points;

    private Vector3 target;

    private int currentPointIndex = 0;

    private float minDistance = 0.1f;

    void Start()
    {
        if (points == null)
        {
            Debug.LogError("Please assign Point A and Point B Transforms in the inspector.");
            enabled = false;
            return;
        }
        
        transform.position = points[currentPointIndex].position;
        target = points[currentPointIndex + 1].position;
    }

    void Update()
    {
        // Move towards the target point
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // If close to the target, switch target
        if (Vector3.Distance(transform.position, target) < minDistance)
        {
            if (currentPointIndex == points.Length - 1)
            {
                currentPointIndex = 0;
            }
            else
            {
                currentPointIndex += 1;
                target = points[currentPointIndex + 1].position;
            }
                
            //target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }
}