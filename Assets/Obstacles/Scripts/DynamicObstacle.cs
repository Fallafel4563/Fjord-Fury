using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public Transform[] points;
    
    private int currentPointIndex = 0;
    
    public float speed = 3f;  // Movement speed

    private float minDistance = 0.1f; // Minimum distance required between player and target point to move to next point in array

    private Vector3 target; 

    public bool closeLoop = false; 

    void Start()
    {
        if (points == null)
        {
            Debug.LogError("Please assign Point Transforms in the inspector.");
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
            if (currentPointIndex == points.Length - 1) //If at the end of list of points and close loop is checked; set target back to the start
            {
                if (closeLoop == true)
                {
                    currentPointIndex = points.Length;
                    target = points[0].position;
                }
                else
                {
                    currentPointIndex = 0;
                    transform.position = points[0].position;
                    target = points[currentPointIndex + 1].position;
                }
                
            }
            else
            {
                currentPointIndex += 1;
                target = points[currentPointIndex + 1].position;
            }
                
            
        }
    }
}