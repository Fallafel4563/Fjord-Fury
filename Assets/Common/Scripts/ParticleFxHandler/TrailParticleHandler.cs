using UnityEngine;

public class TrailParticleHandler : MonoBehaviour
{

    public SplineBoat boatRef;

    public ParticleSystem waterTrail, rainbowTrail, grindTrail, pissTrail;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (boatRef.isGrounded == false)
        {
            waterTrail.gameObject.SetActive(false);
            rainbowTrail.gameObject.SetActive(false);
            grindTrail.gameObject.SetActive(false);
            pissTrail.gameObject.SetActive(false);
        }
        else
        {
            trackType currentTrack = boatRef.currentTrack.trailType;
            switch (currentTrack)
            {
                case trackType.water:
                    waterTrail.gameObject.SetActive(true);
                    break;
                case trackType.rainbow:
                    rainbowTrail.gameObject.SetActive(true);
                    break;
                case trackType.grind:
                    grindTrail.gameObject.SetActive(true);
                    break;
                case trackType.piss:
                    pissTrail.gameObject.SetActive(true);
                    break;
            }
        }
    }


}
public enum trackType
{
    water,
    rainbow,
    grind, 
    piss
}