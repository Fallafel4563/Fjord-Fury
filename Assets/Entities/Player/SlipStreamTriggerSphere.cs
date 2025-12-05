using UnityEngine;

public class SlipStreamTriggerSphere : MonoBehaviour
{
    public float Timer = 2;
    public GameObject Instantiator;
    
    void Start()
    {
        
    }

   
    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            Destroy (gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Instantiator)
        return;

        Debug.Log("Collided with Player");
        if (other.gameObject.TryGetComponent<SlipStream>( out SlipStream slipStream)) 
        {
            slipStream.slipStreamTimer = slipStream.slipStreamMaxTimer;
            Debug.Log("slipStreamTimer = slipStreamMaxTimer");
            
        }
    }
}
