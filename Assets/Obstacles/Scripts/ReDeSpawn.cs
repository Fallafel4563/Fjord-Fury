using UnityEngine;

public class ReDeSpawn : MonoBehaviour
{
    public Transform pointA;   // Starting point
    public Transform pointB;   // Ending point
    public float speed = 3f;   // Movement speed
    public float respawnDelay = 1f; // Delay before respawn
    public float timeOffset = 0f;

    private Vector3 target;
    private bool isMoving = true;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Please assign Point A and Point B Transforms in the inspector.");
            enabled = false;
            return;
        }

        Invoke(nameof(Respawn), timeOffset);
    }

    void Update()
    {
        if (!isMoving) return;

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if reached Point B
        if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
        {
            // Stop moving and start respawn coroutine
            isMoving = false;
            StartCoroutine(RespawnAfterDelay());
        }
    }

    void Respawn()
    {
        transform.position = pointA.position;
        target = pointB.position;
        isMoving = true;
    }

    System.Collections.IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }
}