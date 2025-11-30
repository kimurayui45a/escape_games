using UnityEngine;

[CreateAssetMenu(fileName = "DifferenceStageSO", menuName = "Scriptable Objects/DifferenceStageSO")]
public class DifferenceStageSO : ScriptableObject
{
    [SerializeReference] public DifferenceSpotSO[] differenceSpotSOList;
}
