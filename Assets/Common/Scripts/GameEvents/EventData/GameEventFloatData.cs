using UnityEngine;
// This class can be used to make an asset that can be added to GameEvents and which allows the event to send data to another script.
// For this class only a single float can be sent with an event.

// This allows us to make a new instance of this class as an asset
[CreateAssetMenu(menuName = "Game Event Data/Float Event Data", fileName = "NewFloatEventData")]
public class GameEventFloatData : GameEventData
{
    public float data = 1.0f;
}
