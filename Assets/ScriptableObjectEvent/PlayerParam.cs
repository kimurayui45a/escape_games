using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParam : MonoBehaviour
{
    [SerializeField]
    int paramA;
    [SerializeField]
    List<string> completedEventId = new();

    public int ParamA => paramA;


    public void CompleteEvent(string eventId)
    {
        if (!completedEventId.Contains(eventId))
        {
            completedEventId.Add(eventId);
        }
    }

    public bool IsCompletedEvent(string eventId)
    {
        return completedEventId.Contains(eventId);
    }

}
