using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectableManager : MonoBehaviour
{

    public ScriptableObjectFloat coinsCollected;
    private int coinCount;
    private int collectableCount;
    private Rigidbody rb;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coinCount = 0;
        collectableCount = 0;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickupCoin"))
        {
            other.gameObject.SetActive(false);
            coinCount = coinCount + 1;
            coinsCollected.value += 1;
            Debug.Log("Coin Picked up");
        }
        else if(other.gameObject.CompareTag("PickupCollectable"))
        {
            other.gameObject.SetActive(false);
            collectableCount = collectableCount + 1;
            Debug.Log("Collectable Picked up");
        }  
    }
}
