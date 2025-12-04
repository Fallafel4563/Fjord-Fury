using UnityEngine;

public class SlipStreamTriggerSphere : MonoBehaviour
{
    public float Timer = 2;
    public GameObject Instantiator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
        if (other.gameObject.TryGetComponent<SlipStream>( out SlipStream slipStream)) //&& Instantiator == false)
        {
            slipStream.slipStreamTimer = slipStream.slipStreamMaxTimer;
            Debug.Log("slipStreamTimer = slipStreamMaxTimer");
            
        }
    }
}
