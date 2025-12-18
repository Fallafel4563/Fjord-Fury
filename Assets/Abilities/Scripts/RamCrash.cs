using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class RamCrash : MonoBehaviour
{
    public float bumpForceMultiplier;
    public float bumpDistance;
    public float bumpHeight;
    public GameObject VFX;
    public UnityEvent OnCrash;

    public Transform owner;
    public PlayerMovement playerMovement;
    public bool allowCrash = false;


    public void Enable(Transform ownerR, float strength)
    {
        allowCrash = true;
        owner = ownerR;
        playerMovement = owner.GetComponent<PlayerMovement>();
        bumpForceMultiplier *= strength;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!allowCrash)
            return;
        

        if(other.gameObject.TryGetComponent(out PlayerCrash otherBoat) && other.transform != owner)
        {
            Debug.Log("Ram Crash");
            PlayerMovement otherPlayerMovement = otherBoat.playerMovement;

            float forwardSpeed = playerMovement.currentForwardSpeed;

            Vector3 HorizontalSpeed = playerMovement.HorizontalVelocity;

            Vector3 bumpVelocity = (otherPlayerMovement.HorizontalVelocity - HorizontalSpeed ) * bumpForceMultiplier + Vector3.forward * (otherPlayerMovement.currentForwardSpeed - forwardSpeed);
            bumpVelocity[1] = 1f;  //bumpForceMultiplier;
            // Vector3 bumpVelocity = new Vector3(HorizontalSpeed.x * bumpForceMultiplier + 1, HorizontalSpeed.magnitude * bumpForceMultiplier, forwardSpeed);
            Vector3 direction = (otherPlayerMovement.transform.position - transform.position).normalized;
            //otherPlayerMovement.gameObject.GetComponent<Rigidbody>().AddForce(direction * bumpVelocity.magnitude, ForceMode.Impulse);
            
            otherPlayerMovement.DetachFromCart();
            otherPlayerMovement.airVelocity += direction * bumpVelocity.magnitude + (Vector3.up * bumpHeight * forwardSpeed / 30);

            OnCrash.Invoke();

            GameObject particle = Instantiate(VFX, transform.position+direction,Quaternion.identity); 
            particle.transform.localScale = Vector3.one * bumpVelocity.magnitude;
            Destroy(particle, 2f);
        }
    }

    public IEnumerator SetBump(PlayerMovement boat, Vector3 bump)
    {
        yield return new WaitForEndOfFrame();
        boat.DetachFromCart();
        boat.airVelocity = bump;
    }
}
