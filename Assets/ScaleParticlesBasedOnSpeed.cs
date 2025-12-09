using UnityEngine;

public class ScaleParticlesBasedOnSpeed : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public PlayerMovement playerMovement;
    public ParticleSystem particleSystem;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        particleSystem.startLifetime = playerMovement.currentForwardSpeed / 40f;
    }
}
