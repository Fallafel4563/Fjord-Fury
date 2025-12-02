using UnityEngine;
using System;
using Unity.Cinemachine;
using UnityEngine.Splines;
using UnityEngine.Events;

public class PlayerCrash : MonoBehaviour
{
    public float bumpForceMultiplier;
    public float bumpDistance;
    public GameObject VFX;
    private float collisionForce;
    
    
   
    void Start()
    {
        
    }

    
    void Update()
    {
       
    }

    //void OnTriggerEnter(Collider other)
    //{
       /* if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Works");
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if(playerMovement != null)
            {
                float forwardSpeed = playerMovement.currentForwardSpeed;
                Debug.Log("The current speed" + forwardSpeed);

                Vector3 HorizontalSpeed = playerMovement.HorizontalVelocity;
                Debug.Log("Horizontal speed" + HorizontalSpeed);

                CinemachineSplineCart splineCartVelocity = playerMovement.splineCart;
                Debug.Log("Spline Cart speed" + splineCartVelocity);



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


           

            Instantiate(VFX); 
            //collisionForce = other.impulse.magnitude;
           // bumpDistance = bumpForceMultiplier; //* //collisionForce;
            //Vector3 direction = (other.transform.position-transform.position).normalized;
            //other.gameObject.GetComponent<Rigidbody>().AddForce(direction * bumpDistance, ForceMode.Impulse);
        }
    }*/
}
