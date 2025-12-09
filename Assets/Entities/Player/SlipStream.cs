using UnityEngine;

public class SlipStream : MonoBehaviour
{
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;
    private bool slipStream;
    public float speedMultiplier = 0;
    public float speedIncrease = 0.001f;
    public float maxSpeed = 1.5F;
    public float baseSpeed = 1;
    public float slipStreamTimer;
    public float slipStreamMaxTimer = 3;
    public float startTimer = 0;
    public float startThreshold = 3;
    public GameObject TriggerSphere;
    public float spawnTime = 0;
    public float spawnThreshold = 1;
    
    void  Awake()
    {
        forwardSpeedMultiplier = GetComponent<ForwardSpeedMultiplier>();
    }

   
    void Update()
    {
        
        if (slipStreamTimer > 0)
        {
            slipStreamTimer -= Time.deltaTime;

            if (startTimer < startThreshold)
            {
                startTimer += Time.deltaTime;
            }
        }

        spawnTime += Time.deltaTime;
        if (spawnTime >= spawnThreshold)
        {
           GameObject slipStreamSphere = Instantiate(TriggerSphere, transform.position, Quaternion.identity);
           slipStreamSphere.GetComponent<SlipStreamTriggerSphere>().Instantiator = this.gameObject;
            spawnTime = 0;
            Debug.Log("Spawned Sphere");
        }
        
    }

    void FixedUpdate()
    {
        

        if (startTimer >= startThreshold && slipStreamTimer > 0)
        {
            slipStream = true;
        }
        else
        {
            if (slipStreamTimer <= 0)
            {
                slipStream = false;
                startTimer = 0;
            }
        }

        if (slipStream == true)
        {
            if (speedMultiplier + 1 < maxSpeed)
            {
                speedMultiplier += speedIncrease;
                forwardSpeedMultiplier.SetForwardSpeedMultiplier("SlipStream", speedMultiplier + 1);
                
            }
        }
        else
        {
            
            if (slipStream == false && speedMultiplier + 1 > baseSpeed)
            {
                speedMultiplier -= speedIncrease / 2;
                forwardSpeedMultiplier.SetForwardSpeedMultiplier("SlipStream", speedMultiplier + 1);
                
            }
        }
        
    }
}
