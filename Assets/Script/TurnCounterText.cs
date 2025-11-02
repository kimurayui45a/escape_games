using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ラベルをターン番号に合わせて表示する「ビュー」側コンポーネント。
/// - Manager（TurnManager）はデータとイベントのみを担当
/// - View（本クラス）は表示だけを担当（疎結合）
/// シーンごとに置き、Enable/Disableで購読開始/解除するのが安全。
/// </summary>
public class TurnCounterText : MonoBehaviour
{
    /// <summary>
    /// 表示先のラベル。
    /// </summary>
    [SerializeField] private Text label;

    /// <summary>
    /// オブジェクトが有効化されたときに呼ばれる。
    /// - 初期表示を即反映
    /// - イベント購読開始
    /// </summary>
    private void OnEnable()
    {
        Debug.Log(TurnManager.Instance.CurrentTurn);

        // TurnManager が存在していれば初期表示＆購読開始
        if (TurnManager.Instance != null)
        {
            // 現在値を初期表示（シーンを跨いだ直後も即時反映される）
            UpdateLabel(TurnManager.Instance.CurrentTurn);

            // 以降の変化を購読
            TurnManager.Instance.OnTurnChanged += UpdateLabel;
        }
        // もし null の場合は、後から生成される設計なら
        // シーンロード通知で再バインドするなどの工夫が必要（任意）。
    }

    /// <summary>
    /// 無効化時にイベント購読を解除。
    /// これをしないと、破棄済みオブジェクトにイベントが飛び、例外の原因になる。
    /// </summary>
    private void OnDisable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged -= UpdateLabel;
        }
    }

    /// <summary>
    /// 実際の表示更新処理。イベントから呼ばれる。
    /// </summary>
    private void UpdateLabel(int turn)
    {
        if (label != null)
        {
            label.text = $"Turn: {turn}";
        }
    }
}
