using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーが持つパラメータと、
/// 達成済みイベントIDのリストを管理するクラス。
/// ・ParamA（例：レベル、ポイント、進行度など）
/// ・完了したイベントIDの記録
/// などを提供し、イベント解放判定の土台となる。
/// </summary>
public class PlayerParam : MonoBehaviour
{
    private const string SaveKey = "player_param"; // ファイル名の元になるキー

    /// <summary>
    /// プレイヤーのパラメータA。
    /// EventData の ConditionDataParamA などで参照される想定の値。
    /// （何を意味するかはゲーム側の設計次第：例 レベル / スコア / 経験値 など）
    /// </summary>
    [SerializeField]
    int paramA;

    /// <summary>
    /// プレイヤーが「完了した」と判定されたイベントIDの一覧。
    /// EventId をキーとして管理し、二重登録は行わない。
    /// </summary>
    [SerializeField]
    List<string> completedEventId = new();

    /// <summary>
    /// プレイヤーの ParamA を外部から参照するためのプロパティ。
    /// 読み取り専用（変更はこのクラス内、またはインスペクタで行う想定）。
    /// </summary>
    public int ParamA => paramA;

    void Awake()
    {
        // ゲーム起動時（シーンロード時）に読み込み
        Load();
    }

    /// <summary>
    /// 指定したイベントIDを「完了済み」として登録する。
    /// すでに登録済みの場合は何もしない（重複を避ける）。
    /// </summary>
    /// <param name="eventId">完了したイベントのID</param>
    public void CompleteEvent(string eventId)
    {
        // まだ完了リストに含まれていないIDのみ追加
        if (!completedEventId.Contains(eventId))
        {
            completedEventId.Add(eventId);
        }
    }

    /// <summary>
    /// 指定したイベントIDが「完了済み」かどうかを判定する。
    /// </summary>
    /// <param name="eventId">チェックしたいイベントのID</param>
    /// <returns>true: 完了済み / false: 未完了</returns>
    public bool IsCompletedEvent(string eventId)
    {
        return completedEventId.Contains(eventId);
    }
    
    /// <summary>
    /// paramA と completedEventId をまとめて保存
    /// </summary>
    public void Save()
    {
        var data = new PlayerParamSaveData
        {
            paramA = paramA,
            completedEventId = new List<string>(completedEventId) // 念のためコピー
        };

        JsonFileHelper.SaveObjectData(SaveKey, data);
        Debug.Log("[PlayerParam] Saved.");
    }

    /// <summary>
    /// paramA と completedEventId をまとめてロード
    /// </summary>
    public void Load()
    {
        // ファイルが無い場合はインスペクタ初期値を維持したいので、Existsで分岐
        if (!JsonFileHelper.ExistsData(SaveKey))
        {
            Debug.Log("[PlayerParam] No save file. Use inspector defaults.");
            return;
        }

        var data = JsonFileHelper.LoadObjectData(SaveKey, new PlayerParamSaveData());

        paramA = data.paramA;
        completedEventId = data.completedEventId ?? new List<string>();

        Debug.Log("[PlayerParam] Loaded.");
    }
}
