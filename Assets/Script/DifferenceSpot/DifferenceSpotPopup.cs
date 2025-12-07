using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DifferenceSpotPopup : PopupViewScript
{

    [SerializeField]
    private TMP_Text titleText;

    // 残り differenceSpot を表示する Text
    [SerializeField]
    private TMP_Text differenceSpotText;

    // クリア時に「クリア」と表示する Text
    [SerializeField]
    private TMP_Text clearMessageText;

    // ここにSOをアサイン（間違い探しのstageデータ）
    [SerializeField]
    private DifferenceStageSO differenceStageSO;

    // 表示するstage
    [SerializeField]
    private Transform stageObject;


    // 現在の残りスポット数（SO の differenceSpot から初期化）
    private int remainingSpots;


    // クリック済みオブジェクト名リスト（同じ名前は1回だけカウント）
    private HashSet<string> clickedObjectNames = new HashSet<string>();

    public override void Setup(string id)      // ← override & object 受け
    {
        if (!differenceStageSO || differenceStageSO.differenceSpotSOList == null || differenceStageSO.differenceSpotSOList.Length == 0)
        {
            Debug.LogWarning("[differenceSpotSOList] DifferenceStageSO が未設定/空です。", this);
            return;
        }
        if (!stageObject)
        {
            Debug.LogWarning("[differenceSpotSOList] stageObject(表示先) が未設定です。", this);
            return;
        }

        // 1) ID で該当データを検索（完全一致）
        var data = differenceStageSO.differenceSpotSOList
            .FirstOrDefault(d => d != null && d.differenceStageNo == id);

        if (data == null)
        {
            Debug.LogWarning($"[DifferenceSpot] 指定IDに一致するステージが見つかりませんでした: \"{id}\"", this);
            ClearChildren(stageObject);
            if (titleText) titleText.text = string.IsNullOrEmpty(id) ? "Stage" : id;

            // カウンタとテキストもリセット
            remainingSpots = 0;
            clickedObjectNames.Clear();
            UpdateSpotTexts();
            return;
        }

        // 2) タイトル更新
        if (titleText)
        {
            titleText.text = string.IsNullOrEmpty(data.differenceStageName)
                ? data.differenceStageNo
                : data.differenceStageName;
        }

        // 3) 既存の子をクリアしてから、該当プレハブを生成
        ClearChildren(stageObject);

        if (data.differenceSpotPrefab == null)
        {
            Debug.LogWarning($"[DifferenceSpot] プレハブ未設定: {data.differenceStageNo}", this);
            remainingSpots = 0;
            clickedObjectNames.Clear();
            UpdateSpotTexts();
            return;
        }

        // ★ differenceSpot の初期値を ScriptableObject から取得
        remainingSpots = data.differenceSpot;
        clickedObjectNames.Clear();
        UpdateSpotTexts();   // 最初の表示

        // DifferenceSpotScript をルートに持つプレハブを生成
        var spawned = Instantiate(data.differenceSpotPrefab, stageObject);

        // 任意：ローカル座標・スケール初期化（必要に応じて）
        var t = spawned.transform as RectTransform ?? spawned.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        // ★ 生成したプレハブ内の DifferenceSpot に「このポップアップへの参照」を渡す
        var spots = spawned.GetComponentsInChildren<DifferenceSpot>(true);
        foreach (var spot in spots)
        {
            spot.Initialize(this);
        }
    }

    /// <summary>
    /// DifferenceSpot から「クリックされた」と通知されると呼ばれる
    /// </summary>
    public void OnSpotClicked(GameObject spotObject)
    {
        if (spotObject == null) return;

        string name = spotObject.name;

        // すでにクリック済みの名前なら何もしない
        if (clickedObjectNames.Contains(name))
        {
            return;
        }

        // 新規クリックなので名前を登録
        clickedObjectNames.Add(name);
        Debug.Log("clickedObjectNames = [" + string.Join(", ", clickedObjectNames) + "]");


        // 残りカウントを減らす（0より大きい場合のみ）
        if (remainingSpots > 0)
        {
            remainingSpots--;
            UpdateSpotTexts();
        }

        // 0になったらクリア表示
        if (remainingSpots <= 0)
        {
            ShowClearMessage();
        }
    }

    /// <summary>
    /// differenceSpot 数字の Text と クリア Text を更新
    /// </summary>
    private void UpdateSpotTexts()
    {
        // differenceSpot 数字の表示
        if (differenceSpotText)
        {
            // 常に数字は出しておくなら ToString() だけでもよい
            differenceSpotText.text = remainingSpots.ToString();
        }

        // クリアテキストは「残りがある間は何も出さない」
        if (clearMessageText)
        {
            clearMessageText.text = remainingSpots <= 0 ? "Clear!" : string.Empty;
        }
    }

    /// <summary>
    /// クリア時の表示（必要ならSE再生などもここに追加）
    /// </summary>
    private void ShowClearMessage()
    {
        UpdateSpotTexts();
        // ここでアニメーションやSE再生なども可能
        Debug.Log("[DifferenceSpot] クリア！");
    }



    /// <summary>
    /// 表示先の子をすべて削除（実行中は Destroy、エディタは DestroyImmediate）
    /// </summary>
    private void ClearChildren(Transform parent)
    {
        if (!parent) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(child.gameObject);
            else
#endif
                Destroy(child.gameObject);
        }
    }
}
