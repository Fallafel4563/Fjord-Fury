using System.Collections.Generic;
using UnityEngine;
// A class that allows you to make game events assets, whihc will be observed by a GameEventsObserver.
// To send the data, responses from the GameEventsObserver has to be connected to functions that have a GameEventData as a parameter.

[CreateAssetMenu(menuName = "Game Event", fileName = "NewGameEvent")]
public class GameEvent : ScriptableObject
{
    // Data that this event will have
    public GameEventData eventData;
    // All observers that are observing this event
    private List<GameEventsObserver> observers = new();


    // Make an observer observe this event
    public void AddObserver(GameEventsObserver observer)
    {
        observers.Add(observer);
    }


    // Stop an observer from observing this event
    public void RemoveObserver(GameEventsObserver observer)
    {
        observers.Remove(observer);
    }


    public void TriggerEvent()
    {
        // Tell all observers that is observing this event to respond
        for (int i = 0; i < observers.Count; i++)
        {
            observers[i].RespondToEvent(name);
        }
    }
}
