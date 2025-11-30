using UnityEngine;
using UnityEngine.UI;

public class ItemsBookButton : MonoBehaviour
{
    [SerializeField] private Button btnItems;
    [SerializeField] private Image imgItems;

    private ItemsBookPopup owner;          // 呼び出し元のポップアップ
    private ItemSkillDataSO data;          // 自分が保持する1件分のデータ

    /// <summary>
    /// インスタンス生成時に親とデータを渡す
    /// </summary>
    public void SetupItemBookButton(ItemsBookPopup popup, ItemSkillDataSO itemSkillDataSO)
    {
        owner = popup;
        data = itemSkillDataSO;

        // サムネ画像
        var basic = data?.itemBasicData;
        // 所持していない場合は noImg、所持している場合は itemImage
        if (imgItems)
        {
            if (basic == null)
            {
                imgItems.sprite = null;
            }
            else
            {
                bool owned = owner != null && owner.IsOwned(basic.itemBookNo);
                imgItems.sprite = owned ? basic.itemImage : basic.noImg;
            }
        }

        // クリックで詳細表示
        if (btnItems)
        {
            btnItems.onClick.RemoveAllListeners();
            btnItems.onClick.AddListener(OnClickItemsDetail);
        }
    }

    void OnClickItemsDetail()
    {
        if (owner != null)
            owner.ShowItemDetail(data);
    }
}
