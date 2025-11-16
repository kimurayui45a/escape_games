using UnityEngine;

/// <summary>
/// クリックで反応させたい対象に付けるマーキング用。
/// 種類(enum)と表示名などを一元管理。
/// </summary>
[DisallowMultipleComponent]
public class ItemTypeController : MonoBehaviour
{
    // オブジェクトの種類
    [SerializeField] ItemType kind = ItemType.None;

    // UI に表示する名称
    [SerializeField] string displayName = "Unnamed";

    // オブジェクトの種別をクリック受付側が参照
    public ItemType Kind => kind;

    // UI に表示するための名称
    public string DisplayName => displayName;

    // ※必要になったら、詳細データ（ScriptableObjectやアイコン等）を追加して拡張可

}
