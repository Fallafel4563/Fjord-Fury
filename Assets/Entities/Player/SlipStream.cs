using UnityEngine;

public class SlipStream : MonoBehaviour
{
    [HideInInspector] public PlayerMovement playerMovement;
    private bool slipStream;
    public float speedMultiplier;
    public float speedIncrease;
    public float maxSpeed;
    public float baseSpeed;
    public float slipStreamTimer;
    public float slipStreamMaxTimer;
    public float startTime;
    public float startThreshold;
    public GameObject TriggerSphere;
    public float spawnTime;
    public float spawnThreshold;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
