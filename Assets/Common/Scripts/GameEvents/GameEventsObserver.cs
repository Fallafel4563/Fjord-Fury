using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// This class can observe many different GameEvents and will respond with an UnityEvent when the events are triggered.

public class GameEventsObserver : MonoBehaviour
{
    // All the events this observer is observing
    [SerializeField] private List<GameEventResponse> events = new();

    // This event will be called when any event is invoked.
    [SerializeField] private UnityEvent anyResponse;


    // Add this component to the list of observers that events are "talking" to
    private void OnEnable()
    {
        for (int i = 0; i < events.Count; i++)
        {
            GameEventResponse eventResponse = events[i];
            eventResponse.gameEvent.AddObserver(this);
        }
    }


    // Remove this component from the list of observers that events are "talking" to
    private void OnDisable()
    {
        for (int i = 0; i < events.Count; i++)
        {
            GameEventResponse eventResponse = events[i];
            eventResponse.gameEvent.RemoveObserver(this);
        }
    }


    private void OnValidate()
    {
        // Set the name of each element in the events list to be the name of the event
        for (int i = 0; i < events.Count; i++)
        {
            GameEventResponse eventResponse = events[i];
            // Only change the name if the event resposne has an event
            if (eventResponse.gameEvent != null)
                eventResponse.title = eventResponse.gameEvent.name;
        }
    }


    public void RespondToEvent(string eventName)
    {
        // Go through every event and only respond to the event that was triggered
        for (int i = 0; i < events.Count; i++)
        {
            GameEventResponse eventResponse = events[i];
            if (eventResponse.gameEvent.name == eventName)
                eventResponse.response?.Invoke(eventResponse.gameEvent.eventData);
        }

        // Invoke the AnyResponse event when any event has been triggered
        anyResponse?.Invoke();
    }
}


[System.Serializable]
public class GameEventResponse
{
    // Name of the event. Is only used to set the name of the elements in the events list
    [HideInInspector] public string title;
    // The GameEvent this response is observing
    public GameEvent gameEvent;
    // This event will be called when the GameEvent is triggered
    public UnityEvent<GameEventData> response;

}