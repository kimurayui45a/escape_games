using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムの基本データ
/// </summary>
[System.Serializable]
public class ItemBasicData
{
    public string itemBookNo;
    public ItemNameType itemNameType;
    public string itemName;
    public Sprite itemImage;
    public string itemExplanation;
    public int itemLevel;
    public List<ItemNameType> relatedItemsTypeList = new();
    public string hintText;
    public Sprite noImg;
}
