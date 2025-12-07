using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MergeGameSO", menuName = "Scriptable Objects/MergeGameSO")]
public class MergeGameSO : ScriptableObject
{
    public string mergeGameStageNo;
    public string mergeGameStageName;
    public MergeGameStageScript mergeGameStagePrefab;
    public MergeGameItem[] mergeGameItemList;
}