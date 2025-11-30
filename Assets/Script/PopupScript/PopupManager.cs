using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("Parents")]
    [SerializeField] private Transform popupContainer; // 生成先（未指定なら自分）

    [Header("Registry (Type -> Prefab)")]
    [SerializeField] private List<PopupData> entries = new(); // インスペクタで紐付け

    // 実行時キャッシュ
    private Dictionary<PopupType, PopupViewScript> map; // Type -> Prefab
    private PopupViewScript current;                    // 現在表示中（同時1個想定）

    /// <summary>いま何か表示しているか</summary>
    public bool IsOpen => current != null;

    private void Awake()
    {
        // Singleton（任意）
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // レジストリを辞書化（null を除去し、Type 重複は先勝ち）
        map = entries?
            .Where(e => e != null && e.prefab != null)
            .GroupBy(e => e.type)
            .ToDictionary(g => g.Key, g => g.First().prefab)
            ?? new Dictionary<PopupType, PopupViewScript>();

        // 親フォールバック
        if (!popupContainer) popupContainer = transform as RectTransform;
    }

    /// <summary>
    /// Type 指定でポップアップを開く。既存があれば破棄して差し替え。
    /// payload は派生 PopupViewScript の Setup で受け取る想定（任意）。
    /// </summary>
    public PopupViewScript Show(PopupType type, string payload = null)
    {
        if (!map.TryGetValue(type, out var prefab) || !prefab)
        {
            Debug.LogWarning($"[PopupManager] Prefab not registered for type: {type}");
            return null;
        }

        // 既存があれば先に閉じる（同時1個運用）
        Close();

        // 生成
        var parent = popupContainer ? popupContainer : transform;
        current = Instantiate(prefab, parent);
        current.name = $"{type} (Runtime)";

        // 初期化（必要なデータを渡す）
        current.Setup(payload); // PopupViewScript 側で virtual Setup(object) を実装しておく

        // もし基底に Close ボタンがあるならここでフック（任意）
        current.CloseButton?.onClick.AddListener(Close);

        // アニメがあるなら OpenRoutine を回す設計でも可
        // StartCoroutine(current.OpenRoutine());

        return current;
    }

    /// <summary>表示中を閉じる（演出なしの即破棄）</summary>
    public void Close()
    {
        if (current == null) return;
        Destroy(current.gameObject);
        current = null;
    }

    /// <summary>型を指定して取りたい場合（キャスト付き）</summary>
    public T Show<T>(PopupType type, string payload = null) where T : PopupViewScript
    {
        var v = Show(type, payload);
        return v as T;
    }
}
