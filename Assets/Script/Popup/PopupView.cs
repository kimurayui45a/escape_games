using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ポップアップ「実体」。プレハブに付けるコンポーネント。
/// ・タイトル設定
/// ・クリック座標ロギング（任意）
/// ・閉じるボタンで自身をDestroy
/// </summary>
public class PopupView : MonoBehaviour, IPointerClickHandler
{
    
    [Header("UI")]
    [SerializeField] private RectTransform popup;      // 自身のRectTransform
    [SerializeField] private Button closeButton;

    // 共通しない下の要素は継承先に持たせる
    [SerializeField] private TMP_Text titleText;
    

    /// <summary>Canvas が Screen Space - Camera / World の場合に使うカメラ。Overlayなら null</summary>
    public Camera UiCamera { get; set; }

    private void Awake()
    {
        if (!popup) popup = transform as RectTransform;
        //if (closeButton) closeButton.onClick.AddListener(closeButton);
    }

    public void Setup(string title)
    {
        if (titleText) titleText.text = title ?? string.Empty;
    }

    public Button CloseButton => closeButton;

    /// <summary>
    /// ポップアップ内クリック位置（左上原点）をログに出すデバッグ用
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!popup) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                popup, eventData.position, UiCamera, out var local))
        {
            Vector2 size = popup.rect.size;
            Vector2 topLeft = new Vector2(-popup.pivot.x * size.x, (1f - popup.pivot.y) * size.y);
            Vector2 xy = new Vector2(local.x - topLeft.x, -(local.y - topLeft.y));

            if (xy.x < 0 || xy.y < 0 || xy.x > size.x || xy.y > size.y)
                Debug.Log($"[PopupClickLogger] outside: {xy}");
            else
                Debug.Log($"[PopupClickLogger] inside: ({xy.x:0.##}, {xy.y:0.##})");
        }
    }
}
