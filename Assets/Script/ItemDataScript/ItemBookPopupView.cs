using System.Linq;
using UnityEngine;

public class ItemBookPopupView : PopupView
{
    public static ItemBookPopupView Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private ItemDataSO itemDataSO;          // ここにSOをアサイン
    [SerializeField] private ItemBookBtn itemBookBtnPrefab;  // ここにボタンのPrefabをアサイン
   //[SerializeField] private Transform contentParent;         // ここに配置先(Vertical/Horizontal/Grid)をアサイン

    public void Awake()
    {
        if (!itemDataSO || itemDataSO.itemDataObjectList == null)
        {
            Debug.LogWarning("[ItemBookPopupView] ItemDataSO 未設定または空です。", this);
            return;
        }
        if (!itemBookBtnPrefab)
        {
            Debug.LogWarning("[ItemBookPopupView] Prefab もしくは配置先が未設定です。", this);
            return;
        }

        // 既存の子要素をクリア
        //ClearChildren(contentParent);

        // itemBasicData があるものだけを対象にし、itemBookNo 昇順に並べる
        var ordered = itemDataSO.itemDataObjectList
            .Where(d => d != null && d.itemBasicData != null)
            .OrderBy(d => d.itemBasicData.itemBookNo);

        foreach (var data in ordered)
        {
            var btn = Instantiate(itemBookBtnPrefab);
            btn.SetUpItemDetail(data);
        }

        // レイアウトがある場合は、必要に応じて強制リビルド
        // var rect = contentParent as RectTransform;
        // if (rect) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
    }
}
