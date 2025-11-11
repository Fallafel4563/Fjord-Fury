using UnityEngine;

public class StarCollectable : MonoBehaviour
{
   public GameEvent starCollectEvent;

    void OnTriggerEnter (Collider other)
    {
        if(other.CompareTag("Player"))
        {
            starCollectEvent.TriggerEvent();
            gameObject.SetActive(false);
        }
    }
}


