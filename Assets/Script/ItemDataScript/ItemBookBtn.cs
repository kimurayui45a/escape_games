using UnityEngine;
using UnityEngine.UI;

public class ItemBookBtn : MonoBehaviour
{
    [SerializeField]
    private Button btnRewardDetail;

    [SerializeField]
    private Image imgItem;

    // （任意）ボタン押下時の挙動を追加したい場合のために保持
    private ItemDataObject _data;



    public void SetUpItemDetail(ItemDataObject itemDataObject)
    {
        _data = itemDataObject;

        // nullセーフにスプライト設定
        var sprite = (itemDataObject != null && itemDataObject.itemBasicData != null)
            ? itemDataObject.itemBasicData.itemImage
            : null;

        if (imgItem) imgItem.sprite = sprite;

        // 押下時のイベントをここで張る場合（任意）
        // btnRewardDetail?.onClick.RemoveAllListeners();
        // btnRewardDetail?.onClick.AddListener(() => OnClick());
    }
}
