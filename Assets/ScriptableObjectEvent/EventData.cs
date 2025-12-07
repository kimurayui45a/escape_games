using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemParam", menuName = "ScriptableObjectsEvent/EventData")]
public class EventData : ScriptableObject
{
    public enum ComparisonType
    {
        Equal,
        Greater,
        GreaterEqual,
    }

    [System.Serializable]
    public struct ConditionData
    {
        public ComparisonType ComparisonType;
        public int Param;

    }
    public ScreenType ScreenType;
    public string EventId;
    public string Title;
    public string Message;
    public ConditionData ConditionDataParamA;
    public EventData[] UnlockConditions;

}
