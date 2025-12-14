using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// イベントの定義データを保持する ScriptableObject。
/// ・どの画面で発生するか（ScreenType）
/// ・イベントID / タイトル / メッセージ
/// ・発生条件（ConditionData）
/// ・アンロック条件となる他イベントの一覧（UnlockConditions）
/// などをまとめて管理する。
/// </summary>
[CreateAssetMenu(
    fileName = "ItemParam",                   // 新規作成時のデフォルトファイル名
    menuName = "ScriptableObjectsEvent/EventData" // Create メニューに表示されるパス
)]
public class EventData : ScriptableObject
{
    /// <summary>
    /// 条件判定に使用する比較タイプ。
    /// ・Equal        : 等しい
    /// ・Greater      : より大きい
    /// ・GreaterEqual : 以上
    /// </summary>
    public enum ComparisonType
    {
        Equal,
        Greater,
        GreaterEqual,
    }

    /// <summary>
    /// 単一の条件を表すデータ構造。
    /// 「どの比較方法で」「どの値と比較するか」を持つ。
    /// 例: ParamA >= 10 などの判定に使用することを想定。
    /// </summary>
    [System.Serializable]
    public struct ConditionData
    {
        /// <summary>
        /// 比較方法（== / > / >= など）
        /// </summary>
        public ComparisonType ComparisonType;

        /// <summary>
        /// 比較対象となるパラメータ値。
        /// 例: プレイヤーレベル、スコア、ステージ番号など。
        /// </summary>
        public int Param;
    }

    /// <summary>
    /// このイベントが属する画面種別。
    /// どの Screen でチェック・発生させるかの振り分け用。
    /// （ScreenType は別途定義されている enum を想定）
    /// </summary>
    public ScreenType ScreenType;

    /// <summary>
    /// イベントを一意に識別するためのID。
    /// セーブデータやフラグ管理などで参照する。
    /// </summary>
    public string EventId;

    /// <summary>
    /// イベントのタイトル（UI表示用）。
    /// ポップアップやログ表示などに利用する想定。
    /// </summary>
    public string Title;

    /// <summary>
    /// イベント本文となるメッセージテキスト。
    /// ダイアログやログ表示などにそのまま利用できる。
    /// </summary>
    public string Message;

    /// <summary>
    /// 条件パラメータ A に対する判定条件。
    /// 例: ParamA >= 10 でイベント解放、など。
    /// （パラメータAの中身・参照元は、ゲーム側のロジックで対応）
    /// </summary>
    public ConditionData ConditionDataParamA;

    /// <summary>
    /// このイベントがアンロックされるために必要な
    /// 他の EventData のリスト。
    /// ・全てのイベントが満たされている場合に解放、などの判定用。
    /// ・null / 長さ0 の場合は「アンロック条件なし」とみなす想定。
    /// </summary>
    public EventData[] UnlockConditions;
}
