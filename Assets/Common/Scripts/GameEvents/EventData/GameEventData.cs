using UnityEngine;
// Base class for data that can be sent with game events.
// This class won't be used directly, rather you have to make subclasses that have variables with the data they need.

// See GameEventFloatData to see a simple example of how subclasses should be setup

abstract public class GameEventData : ScriptableObject
{

}
