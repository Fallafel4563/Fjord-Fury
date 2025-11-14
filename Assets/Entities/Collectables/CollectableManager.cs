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
        //Pickups require Rigidbody for collision to work
        rb = GetComponent<Rigidbody>();

        //Sets the amount of coins/pickups collected to zero on initialazing
        coinCount = 0;
        coinsCollected.value = 0;
        collectableCount = 0;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickupCoin"))
        {
            other.gameObject.SetActive(false);

            //Play VFX

            //Play SFX

            coinCount = coinCount + 1;
            coinsCollected.value += 1;
            Debug.Log("Coin Picked up");

        }
        else if(other.gameObject.CompareTag("PickupCollectable"))
        {
            other.gameObject.SetActive(false);

            //Play VFX

            //Play SFX

            collectableCount = collectableCount + 1;
            Debug.Log("Collectable Picked up");
        }  
    }
}
