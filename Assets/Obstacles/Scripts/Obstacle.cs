using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool causeHarm = false;
    public bool destructOnCrash = false;
    public float bounceHeight = 0f;
    public float crashSpeedMultiplier = 0.5f;
    public SpeedMultiplierCurve crashSpeedMultiplierCurve;
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
