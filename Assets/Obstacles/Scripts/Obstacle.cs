using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool causeHarm = true;
    public bool destructOnCrash = false;
    public float bounceHeight = 0f;
    public float crashSpeedMultiplier = 0.5f;
    public SpeedMultiplierCurve crashSpeedMultiplierCurve;
    
    public GameObject explosion;
    // TODO: Crash sound
    // TODO: Crash particles
    [HideInInspector] public Transform owner;


    public void OnPlayerCrashed()
    {
        // TODO: Play sound
        if (destructOnCrash)
        {
            Debug.Log("ASD");

            Destroy(this.gameObject);
            // TODO: Play crash particles
        }
    }
}
