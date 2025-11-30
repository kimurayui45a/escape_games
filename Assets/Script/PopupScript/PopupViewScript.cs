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
public class PopupViewScript : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private RectTransform popup;      // 自身のRectTransform
    [SerializeField] private Button closeButton;

    //// 共通しない下の要素は継承先に持たせる
    //[SerializeField] private TMP_Text titleText;


    /// <summary>Canvas が Screen Space - Camera / World の場合に使うカメラ。Overlayなら null</summary>
    public Camera UiCamera { get; set; }

    private void Awake()
    {
        if (!popup) popup = transform as RectTransform;
    }

    public Button CloseButton => closeButton;

    public virtual void Setup(string id) { }

}