using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シーンを跨いで1つだけ存在するターン管理クラス。
/// 「AdvanceTurn() を呼ぶ＝ターンが進む」を全体に通知する。
/// </summary>
public class TurnManager : SingletonMonoBehaviour<TurnManager>
{
    /// <summary>グローバルアクセス用のシングルトン参照</summary>
   // public static TurnManager Instance { get; private set; }

    /// <summary>現在のターン番号（0開始）。外部からは読み取り専用</summary>
    public int CurrentTurn { get; private set; } = 0;

    [SerializeField] private Text turnText;
    [SerializeField] string format = "Turn: {0}";

    /// <summary>
    /// ターンが変化したときに発火するイベント。
    /// 引数は更新後のターン番号。UIやAIが購読して表示更新や行動トリガに使う。
    /// </summary>
    public event Action<int> OnTurnChanged;

    /// <summary>
    /// ターン進行中フラグ。
    /// 多重で AdvanceTurn が呼ばれてダブルカウントするのを防止するために使用。
    /// </summary>
    bool _isAdvancing = false;

    //void Awake()
    //{
    //    // シングルトン化：既に別インスタンスがあれば自分を破棄
    //    if (Instance && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;

    //    // シーン切り替えでも生存させる（タイトル→ゲーム本編などで共通管理したい場合）
    //    DontDestroyOnLoad(gameObject);
    //}

    /// <summary>
    /// ターンを進める（デフォルトは +1）。
    /// 連打・多重呼び出し対策で、進行中は無視する。
    /// </summary>
    /// <param name="step">増分（マイナス指定で巻き戻しも可能）</param>
    public void AdvanceTurn(int step = 1)
    {
        if (_isAdvancing) return;                // 進行中なら弾く
        StartCoroutine(AdvanceRoutine(step));    // コルーチンで進行（演出待ちを挟めるように）
        Debug.Log(CurrentTurn);
    }

    void Start() => RefreshUI();

    /// <summary>
    /// 実際の進行処理。
    /// 必要ならここで「演出が終わるまで待つ」を入れてからカウントを進める。
    /// </summary>
    IEnumerator AdvanceRoutine(int step)
    {
        _isAdvancing = true;

        // カウントを進めて購読者に通知（UI更新・敵AI処理などがここから発火）
        CurrentTurn += step;
        RefreshUI();
        OnTurnChanged?.Invoke(CurrentTurn);

        _isAdvancing = false;
        yield break;
    }

    void RefreshUI()
    {
        if (turnText) turnText.text = string.Format(format, CurrentTurn);
        // else Debug.LogWarning("turnText が未割り当てです");
    }
}
