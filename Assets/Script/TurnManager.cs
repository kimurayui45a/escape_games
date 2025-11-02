using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// シーンを跨いで1つだけ存在する「ターン管理」シングルトン。
/// 進行（AdvanceTurn）時にイベントを発火して、UIやAIに通知する。
/// ※ 具体クラス（TurnManager）が SingletonMonoBehaviour<TurnManager> を継承するのがポイント
/// </summary>
public sealed class TurnManager : SingletonMonoBehaviour<TurnManager>
{
    /// <summary>
    /// 現在のターン番号（0開始）。外部からは読み取り専用。
    /// 値はシーン切り替えでも保持される（インスタンスが生きている限り）。
    /// </summary>
    public int CurrentTurn { get; private set; } = 0;

    /// <summary>
    /// ターンが変化したときに通知するイベント。
    /// 引数は「更新後のターン番号」。
    /// UI・敵AI・制御スクリプトなどが購読して利用する。
    /// </summary>
    public event Action<int> OnTurnChanged;

    /// <summary>
    /// 進行中ガード（二重実行・連打による多重進行を防ぐ）
    /// </summary>
    private bool _isAdvancing;

    /// <summary>
    /// ターンを進めるための公開メソッド。
    /// 通常は step=1（+1）だが、負の値で巻き戻し等も可能。
    /// コルーチンで処理するため、演出待ち（yield）などを挟める。
    /// </summary>
    public void AdvanceTurn(int step = 1)
    {
        //すでに進行中なら無視（連打対策）
        if (_isAdvancing) return;

        // 実処理はコルーチンへ（演出やSE再生の待ちを挟みたいときに柔軟）
        StartCoroutine(AdvanceRoutine(step));
    }

    /// <summary>
    /// 実際のターン進行処理。
    /// 今は即時でカウント＋イベント発火だが、必要に応じて
    /// アニメーション終了待ち（yield return）などを入れられる。
    /// </summary>
    private IEnumerator AdvanceRoutine(int step)
    {
        _isAdvancing = true;

        CurrentTurn += step;

        Debug.Log(CurrentTurn);

        // 変更を購読者へ通知（UI・AIなどが反応）
        OnTurnChanged?.Invoke(CurrentTurn);

        _isAdvancing = false;
        yield break;
    }
}
