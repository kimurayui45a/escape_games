using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DifferenceSpotSO", menuName = "Scriptable Objects/DifferenceSpotSO")]
public class DifferenceSpotSO : ScriptableObject
{
    public string differenceStageNo;
    public string differenceStageName;
    public DifferenceSpotScript differenceSpotPrefab;
    public int differenceSpot;
}
