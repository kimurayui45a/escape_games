using UnityEngine;

[CreateAssetMenu(fileName = "MergeGameStageSO", menuName = "Scriptable Objects/MergeGameStageSO")]
public class MergeGameStageSO : ScriptableObject
{
    [SerializeReference] public MergeGameSO[] mergeGameSOList;
}

