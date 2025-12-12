using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerVibration : MonoBehaviour
{
    public float vibrationDuration;
    public float longVibrationDuration;
    public float softVibrationDuration;
    private int playerIndex;
    public PlayerInput playerInput;
    private Gamepad gamePad;
   // public UnityEvent controllerVibrationOn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerIndex = playerInput.user.index;
        gamePad = playerInput.GetDevice<Gamepad>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ControllerRumble()
    {
        gamePad.SetMotorSpeeds(1,1);
        StartCoroutine(ControllerRumbleDuration());
        //controllerVibrationOn.Invoke();
        
    }

    public void LongControllerRumble()
    {
        gamePad.SetMotorSpeeds(1,1); 
        StartCoroutine(LongControllerRumbleDuration());
    }

    public void SoftControllerRumble()
    {
        gamePad.SetMotorSpeeds(0.5f, 0.5f);
        StartCoroutine(SoftControllerRumbleDuration());
    }

    public void StopControllerRumble()
    {
        gamePad.SetMotorSpeeds(0, 0);
        
    }

    IEnumerator ControllerRumbleDuration()
    {
        yield return new WaitForSeconds(vibrationDuration);
        {
            StopControllerRumble();
        }
        
    } 

    IEnumerator LongControllerRumbleDuration()
    {
        yield return new WaitForSeconds(longVibrationDuration);
        {
            StopControllerRumble();
        }
        
    }

    IEnumerator SoftControllerRumbleDuration()
    {
        yield return new WaitForSeconds(softVibrationDuration);
        {
            StopControllerRumble();
        }
    }
}

