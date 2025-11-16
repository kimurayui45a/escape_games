using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public List<ItemDataObject> itemDataObjectList = new List<ItemDataObject>();

#if UNITY_EDITOR
    void OnValidate()
    {
        if (itemDataObjectList == null) return;

        foreach (var r in itemDataObjectList)
        {
            if (r == null) continue;

            // 目的が Tap の場合はスキル参照を必ず null に正規化
            if (r.itemPurposeType == ItemPurposeType.Tap)
            {
                r.itemSkillData = null;
            }

            // 消費不可なら価格は常に 0（編集されても自動で戻す）
            if (!r.itemConsumableFlag)
            {
                r.itemPrice = 0;
            } 
                
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
