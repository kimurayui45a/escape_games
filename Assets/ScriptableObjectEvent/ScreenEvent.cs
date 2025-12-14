using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面上にイベント用のダイアログ（タイトル＋メッセージ＋YES/NO）を表示し、
/// プレイヤーの選択結果（YES/NO）をコールバックで呼び出すクラス。
/// EventData と紐づけて使う前提。
/// </summary>
public class ScreenEvent : MonoBehaviour
{
    /// <summary>
    /// ダイアログ全体のルート GameObject。
    /// 表示/非表示の切り替えに使用する。
    /// （Canvas 配下の Panel などを想定）
    /// </summary>
    [SerializeField]
    GameObject root;

    /// <summary>
    /// イベントタイトル表示用の Text。
    /// EventData.Title をここに反映する。
    /// </summary>
    [SerializeField]
    Text textTitle;

    /// <summary>
    /// イベント本文（メッセージ）表示用の Text。
    /// EventData.Message をここに反映する。
    /// </summary>
    [SerializeField]
    Text textMessage;

    /// <summary>
    /// 「YES」ボタン。
    /// クリック時に true を返す想定。
    /// </summary>
    [SerializeField]
    Button buttonYes;

    /// <summary>
    /// 「NO」ボタン。
    /// クリック時に false を返す想定。
    /// </summary>
    [SerializeField]
    Button buttonNo;

    /// <summary>
    /// イベント内容を受け取ってダイアログを初期化し、表示する。
    /// YES/NO が押されたら root を非表示にし、結果をコールバックで通知する。
    /// </summary>
    /// <param name="eventData">表示したいイベントのデータ（タイトル・メッセージ等）</param>
    /// <param name="onClick">
    /// YES/NO ボタンが押されたときのコールバック。
    /// ・YES → onClick(true)
    /// ・NO  → onClick(false)
    /// </param>
    public void Initialize(EventData eventData, Action<bool> onClick)
    {
        // ダイアログを表示状態にする
        root.SetActive(true);

        // EventData からタイトルとメッセージを反映
        textTitle.text = eventData.Title;
        textMessage.text = eventData.Message;

        // YES クリック時の処理を登録
        buttonYes.onClick.AddListener(() =>
        {
            // ダイアログを閉じる
            root.SetActive(false);
            // 呼び出し元に「YES が選ばれた」と通知（true）
            onClick?.Invoke(true);
        });

        // NO クリック時の処理を登録
        buttonNo.onClick.AddListener(() =>
        {
            // ダイアログを閉じる
            root.SetActive(false);
            // 呼び出し元に「NO が選ばれた」と通知（false）
            onClick?.Invoke(false);
        });
    }
}
