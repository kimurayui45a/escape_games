using System;
using System.Collections.Generic;

[Serializable]
public class PlayerParamSaveData
{
    public int paramA;
    public List<string> completedEventId = new();
}
