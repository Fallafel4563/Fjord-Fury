using UnityEngine;
using UnityEngine.Events;

public class GrindSound : MonoBehaviour
{
    public PlayerMovement movementScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isGrinding;
    public UnityEvent startGrind, EndGrind,woodgrind,metalgrind, stonegrind;

    // Update is called once per frame
    void Update()
    {
        if (isGrinding)
        {
            if(movementScript.isGrounded == false || !movementScript.currentTrack.isCircle)
            {
                EndGrind.Invoke();

                isGrinding = false;
                
            }
        }
        else
        {
            if(movementScript.isGrounded && movementScript.currentTrack.isCircle)
            {
                isGrinding = true;
                
                switch (movementScript.currentTrack.trailType)
                {
                    case trackType.woodGrind:
                        woodgrind.Invoke();
                        break;
                    case trackType.stoneGrind:
                        stonegrind.Invoke();
                        break;
                    case trackType.metalGrind:
                        metalgrind.Invoke();
                        break;
                    default:
                        metalgrind.Invoke();
                        break;

                }
                startGrind.Invoke();
            }
        }
    }

   
}
