using UnityEngine;

public class BoatSailSound : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;

    public FMODUnity.EventReference fmodEvent;

    public PlayerMovement PM;
    public bool isAlien;


    private void Start()
    {
        Debug.Log("SAILSOUND START!");
        instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        //instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, PM.GetComponent<Rigidbody>()));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, gameObject, PM.GetComponent<Rigidbody>());

        if (isAlien)
        {
            instance.setParameterByName("Boat", 1);
        }
        else
        {
            instance.setParameterByName("Boat", 0);
        }
        instance.start();
    }
    public void Update()
    {
        instance.setParameterByName("SailRPM", PM.currentForwardSpeed *10);
        instance.setParameterByName("IsOutOfWater", PM.isGrounded ? 1 : 0);
    }

}
