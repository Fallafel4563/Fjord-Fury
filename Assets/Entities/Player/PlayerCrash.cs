using UnityEngine;
using System;
using Unity.Cinemachine;
using UnityEngine.Splines;
using UnityEngine.Events;
using System.Collections;

public class PlayerCrash : MonoBehaviour
{
    public float bumpForceMultiplier;
    public float bumpDistance;
    public float bumpHeight;
    public GameObject VFX;
    private float collisionForce;
    [HideInInspector] public PlayerMovement playerMovement;
    
    private bool wasGroundedLastFrame;
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    
    void Update()
    {
        wasGroundedLastFrame = playerMovement.isGrounded;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PlayerCrash otherBoat))
        {
            Debug.Log("Works");
            PlayerMovement otherPlayerMovement = otherBoat.playerMovement;
            if(playerMovement != null && wasGroundedLastFrame)
            {
                float forwardSpeed = playerMovement.currentForwardSpeed;
                Debug.Log("The current speed" + forwardSpeed);

                Vector3 HorizontalSpeed = playerMovement.HorizontalVelocity;
                Debug.Log("Horizontal speed" + HorizontalSpeed);

               Vector3 bumpVelocity = (otherPlayerMovement.HorizontalVelocity - HorizontalSpeed ) * bumpForceMultiplier;
               bumpVelocity[1] = 1f;  //bumpForceMultiplier;
               // Vector3 bumpVelocity = new Vector3(HorizontalSpeed.x * bumpForceMultiplier + 1, HorizontalSpeed.magnitude * bumpForceMultiplier, forwardSpeed);
               Vector3 direction = (otherPlayerMovement.transform.position - transform.position).normalized;
               //otherPlayerMovement.gameObject.GetComponent<Rigidbody>().AddForce(direction * bumpVelocity.magnitude, ForceMode.Impulse);
            
                otherPlayerMovement.DetachFromCart();
                otherPlayerMovement.airVelocity += direction * bumpVelocity.magnitude + ((Vector3.up * bumpHeight));

               // otherPlayerMovement.airVelocity = bumpVelocity;
                //StartCoroutine(SetBump(otherPlayerMovement, bumpVelocity));


                //HorizontalSpeed * bumpForceMultiplier + 1;
                float speed1 = forwardSpeed;
                float speed2 = playerMovement.currentForwardSpeed;
                float average = (speed1 + speed2) / 2f;
                //speed1 = forwardSpeed;
                //speed2 = playerMovement.currentForwardSpeed;
                //forwardSpeed - other.forwardSpeed / ((forwardSpeed - other.forwardSpeed / 2f)) * 100f;
                //Compare with forwardSpeed of other boat, find % difference in speed, forward speed multiplier
                float speedDifference = MathF.Abs(speed1 - speed2);
                float percetnageSpeedDifference = (speedDifference / average) * 100f;

            }


           

           // Instantiate(VFX); 
            //collisionForce = other.impulse.magnitude;
           // bumpDistance = bumpForceMultiplier; //* //collisionForce;
            //Vector3 direction = (other.transform.position-transform.position).normalized;
            //other.gameObject.GetComponent<Rigidbody>().AddForce(direction * bumpDistance, ForceMode.Impulse);
        }
    }

    public IEnumerator SetBump(PlayerMovement boat, Vector3 bump)
    {
        yield return new WaitForEndOfFrame();
        boat.DetachFromCart();
        boat.airVelocity = bump;
    }
}
