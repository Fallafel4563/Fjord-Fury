using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class TrickSoundManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;

    public FMODUnity.EventReference fmodEvent;

    [Range(0, 10)]
    public int pitch;


    public void playTrickSound(int combo)
    {

        instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        instance.setParameterByName("TrickPitch", combo);
        instance.start();
    }


    /*
    [SerializeField] StudioParameterTrigger parameterChanger;
    public UnityEvent playsound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void playTrickSound(int combo)
    {
        // adjust pitch based on combo
        int clamp = Mathf.Clamp(combo, 0, 10);
        parameterChanger.Emitters[0].Params[0].Value = combo;
        playsound.Invoke();
    }
    */


}
