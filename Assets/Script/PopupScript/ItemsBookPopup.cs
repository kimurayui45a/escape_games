using TMPro;
using UnityEngine;
using System.Linq;
using System.Globalization;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemsBookPopup : PopupViewScript
{

    // 共通しない下の要素は継承先に持たせる
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text itemNo;

    [SerializeField]
    private TMP_Text itemName;

    [SerializeField]
    private TMP_Text itemText;

    [SerializeField]
    private Image imgItems;

    [Header("Setup")]
    [SerializeField] private ItemDataSO itemDataSO;          // ここにSOをアサイン（図鑑の全データ）
    [SerializeField] private ItemsBookButton itemsBookBtnPrefab;  // ここにボタンのPrefabをアサイン
    [SerializeField] private Transform contentParent;         // スクロールのContentをアサイン

    // 所持No（とりあえず直書き）
    // 大文字小文字を吸収したいので HashSet は OrdinalIgnoreCase 指定
    private readonly HashSet<string> ownedNos = new HashSet<string>(
        new[] { "CO0001", "CO0003", "CO0004", "CO0005", "CO0006", "CO0008", "EX0003", "EX0004", "EX0006" },
        System.StringComparer.OrdinalIgnoreCase
    );

    public void Awake()
    {
        if (!itemDataSO || itemDataSO.itemSkillDataSOList == null)
        {
            Debug.LogWarning("[ItemBookPopupView] ItemDataSO 未設定または空です。", this);
            return;
        }
        if (!itemsBookBtnPrefab)
        {
            Debug.LogWarning("[ItemBookPopupView] Prefab もしくは配置先が未設定です。", this);
            return;
        }

        // 既存の子要素をクリア
        ClearChildren(contentParent);

        var ordered = itemDataSO.itemSkillDataSOList
            .Where(d => d?.itemBasicData != null && !string.IsNullOrEmpty(d.itemBasicData.itemBookNo))
            .OrderBy(d => PrefixRank(d.itemBasicData.itemBookNo))   // 0=CO, 1=EX, 2=その他
            .ThenBy(d => NumericKey(d.itemBasicData.itemBookNo));   // 数値昇順（パース不可は Int32.MaxValue）

        foreach (var data in ordered)
        {
            var btn = Instantiate(itemsBookBtnPrefab, contentParent);
            btn.SetupItemBookButton(this, data); // 親インスタンス(this)も渡す
        }

        // レイアウトがある場合は、必要に応じて強制リビルド
        //var rect = contentParent as RectTransform;
        //if (rect) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
    public override void Setup(string payload)
    {
        if (titleText) titleText.text = payload ?? string.Empty;
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

    int PrefixRank(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length < 2) return 2;
        var p = code.Substring(0, 2).ToUpperInvariant();
        if (p == "CO") return 0;
        if (p == "EX") return 1;
        return 2; // その他（最後に回す）
    }

    int NumericKey(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length < 3) return int.MaxValue;

        // 先頭2文字以降を数値として解釈（"0001" → 1）
        var numPart = code.Substring(2);
        return int.TryParse(numPart, NumberStyles.None, CultureInfo.InvariantCulture, out var n)
            ? n
            : int.MaxValue; // 数値化できなければ末尾送り
    }


    /// 判定API（ボタン側からも使う）
    public bool IsOwned(string bookNo)
        => !string.IsNullOrEmpty(bookNo) && ownedNos.Contains(bookNo);

    /// <summary>
    /// ボタン側から呼ばれる詳細表示の本体
    /// 所持していない場合は hintText と noImg を表示
    /// </summary>
    public void ShowItemDetail(ItemSkillDataSO itemSkillDataSO)
    {
        var basic = itemSkillDataSO?.itemBasicData;
        if (basic == null)
        {
            if (itemNo) itemNo.text = "";
            if (itemName) itemName.text = "";
            if (itemText) itemText.text = "";
            if (imgItems) imgItems.sprite = null;
            return;
        }

        bool owned = IsOwned(basic.itemBookNo);

        if (itemNo) itemNo.text = basic.itemBookNo ?? "";
        if (itemName) itemName.text = basic.itemName ?? "";

        if (itemText)
            itemText.text = owned ? (basic.itemExplanation ?? "") : (basic.hintText ?? "");

        if (imgItems)
            imgItems.sprite = owned ? basic.itemImage : basic.noImg;
    }
}