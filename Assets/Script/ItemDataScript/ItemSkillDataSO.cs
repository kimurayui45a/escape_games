using UnityEngine;

[CreateAssetMenu(fileName = "ItemSkillDataSO", menuName = "Scriptable Objects/ItemSkillDataSO")]
public class ItemSkillDataSO : ScriptableObject
{
    public ItemBasicData itemBasicData;
    public ItemPurposeType itemPurposeType;
    public bool itemConsumableFlag;
    public int itemPrice;
    public bool itemFoodFlag;
}
