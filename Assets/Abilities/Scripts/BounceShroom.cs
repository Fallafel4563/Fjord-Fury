using UnityEngine;

public class BounceShroom : MonoBehaviour
{
    public float BouncePower;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<PlayerMovement>())
        {
            // Get a refferance to the PlayerMovement script
            PlayerMovement movRef = collider.GetComponent<PlayerMovement>();

            // Get the PlayerMovement ref and call the ShroomBounce function
            movRef.ShroomBounce(BouncePower);
        }
    }
}

/*
    public void ShroomBounce(float bouncePower)
    {
        // Detach the boat form the spline cart
        DetachFromCart();

        // Stop all upwards velocity
        float upwardsVel = Vector3.Dot(airVelocity, transform.up);
        airVelocity -= transform.up * upwardsVel;
        // Set the upwards air velocity to be the equal to jump power
        // Set the air velocity when jumping. Also set the velocity forwads to avoid having the boat stop for a breif moment when jumping
        airVelocity += transform.up * bouncePower;


        // Invoke events
        Jumped.Invoke();
    }
*/