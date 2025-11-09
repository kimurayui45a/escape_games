using UnityEngine;

/// <summary>
/// ポップアップの生成・破棄を行うマネージャ（シングルトン）。
/// ・Show: 必要なときにプレハブから Instantiate
/// ・Close/HideImmediate: 実体を Destroy
/// </summary>
public class Popup : MonoBehaviour
{
    /// <summary>シーンに1つだけ存在させるための簡易シングルトン参照</summary>
    public static Popup Instance { get; private set; }

    [Header("Spawn Settings")]
    [Tooltip("表示に使うポップアップのプレハブ（PopupView が付いていること）")]
    [SerializeField] private PopupView popupPrefab;

    [Tooltip("生成先の親Transform。未指定ならこのGameObject配下に生成")]
    [SerializeField] private Transform parentForUI;

    [Tooltip("Canvas が Screen Space - Camera / World の場合に設定。Overlayなら nullでOK")]
    [SerializeField] private Camera uiCamera;

    /// <summary>現在ポップアップ表示中か（インスタンスの有無で判定）</summary>
    public bool IsOpen => current != null;

    private PopupView current;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// プレハブから新規にポップアップを生成して表示する。
    /// 既存があれば一旦破棄して作り直す（同時1個想定）。
    /// </summary>
    public void Show(ItemTypeController target, string extraMessage = null)
    {
        if (!popupPrefab)
        {
            Debug.LogWarning("[Popup] popupPrefab が未設定です。");
            return;
        }

        // 既存があれば破棄（同時に1つ想定のため）
        if (current)
        {
            Destroy(current.gameObject);
            current = null;
        }

        // 生成
        var parent = parentForUI ? parentForUI : transform;
        current = Instantiate(popupPrefab, parent);
        current.name = "PopupView (Runtime)";
        current.UiCamera = uiCamera;

        // 表示内容の設定
        string title = target != null ? target.DisplayName : string.Empty;
        current.Setup(title);

        if (current.CloseButton)
        {
            current.CloseButton.onClick.RemoveAllListeners();
            current.CloseButton.onClick.AddListener(Close);
        }

        // extraMessage を本文に出したい場合は PopupView に本文Textを足して受け取る実装に拡張してください
        // 例: current.Setup(title, extraMessage);

        // ここで配置/アニメなどがあれば呼ぶ
        // 例: current.ShowAnimation();
    }

    /// <summary>現在のポップアップを閉じる（Destroy）</summary>
    public void Close()
    {
        if (!current) return;

        Destroy(current.gameObject);
        current = null;
    }

    /// <summary>強制的に即時破棄（演出なしリセット用）</summary>
    public void HideImmediate()
    {
        if (!current) return;

        Destroy(current.gameObject);
        current = null;
    }
}
