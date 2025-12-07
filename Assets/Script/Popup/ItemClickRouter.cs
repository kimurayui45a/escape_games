using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// クリックされたら、最も近い Interactable を探して処理をルーティング。
/// ポップアップ表示中は無視して多重発火を防ぐ。
/// </summary>
[RequireComponent(typeof(Collider))] // 2Dなら Collider2D 版を別ファイルで用意してもOK
public class ItemClickRouter : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // 多重発火防止
        if (Popup.Instance && Popup.Instance.IsOpen)
            return;

        // 自身 or 親に ItemTypeControllerスクリプト があるか
        var itemType = GetComponent<ItemTypeController>();
        if (!itemType)
            itemType = GetComponentInParent<ItemTypeController>();

        if (!itemType)
            return;

        // 種別に応じた処理（必要に応じてここを差し替え or Registry化も可能）
        switch (itemType.Kind)
        {
            case ItemType.None:
                Popup.Instance?.Show(itemType);
                break;

            case ItemType.Tree:
                //Popup.Instance?.Show(itemType, extraMessage: "木");
                PopupManager.Instance?.Show(PopupType.MergeGamePopup, "MG001");
                break;

            case ItemType.Pond:
                //Popup.Instance?.Show(itemType, extraMessage: "滝");
                PopupManager.Instance?.Show(PopupType.DifferenceSpotScript, "DF0001");
                break;

            case ItemType.Wall:
                //Popup.Instance?.Show(itemType, extraMessage: "壁");
                break;

            default:
                // 既定動作：名前だけ出す簡易ポップアップ
                Popup.Instance?.Show(itemType);
                break;
        }
    }
}
