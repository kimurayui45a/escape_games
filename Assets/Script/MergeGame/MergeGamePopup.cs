using TMPro;
using UnityEngine;
using System.Linq;

public class MergeGamePopup : PopupViewScript
{

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private MergeGameStageSO mergeGameStageSO;

    // 表示するstage
    [SerializeField]
    private Transform stageObject;


    public override void Setup(string id)      // ← override & object 受け
    {
        if (!mergeGameStageSO || mergeGameStageSO.mergeGameSOList == null || mergeGameStageSO.mergeGameSOList.Length == 0)
        {
            Debug.LogWarning("[mergeGameSOList] MergeGameStageSO が未設定/空です。", this);
            return;
        }
        if (!stageObject)
        {
            Debug.LogWarning("[mergeGameSOList] stageObject(表示先) が未設定です。", this);
            return;
        }

        // 1) ID で該当データを検索（完全一致）
        var data = mergeGameStageSO.mergeGameSOList
            .FirstOrDefault(d => d != null && d.mergeGameStageNo == id);

        if (data == null)
        {
            Debug.LogWarning($"[DifferenceSpot] 指定IDに一致するステージが見つかりませんでした: \"{id}\"", this);
            // 既存表示は消しておく
            ClearChildren(stageObject);
            if (titleText) titleText.text = string.IsNullOrEmpty(id) ? "Stage" : id;
            return;
        }

        // 2) タイトル更新
        if (titleText)
        {
            // 名前があれば優先、無ければIDを出す
            titleText.text = string.IsNullOrEmpty(data.mergeGameStageName) ? data.mergeGameStageNo : data.mergeGameStageName;
        }

        // 3) 既存の子をクリアしてから、該当プレハブを生成
        ClearChildren(stageObject);

        if (data.mergeGameStagePrefab == null)
        {
            Debug.LogWarning($"[DifferenceSpot] プレハブ未設定: {data.mergeGameStageNo}", this);
            return;
        }

        var spawned = Instantiate(data.mergeGameStagePrefab, stageObject);

        // 任意：ローカル座標・スケール初期化（必要に応じて）
        var t = spawned.transform as RectTransform ?? spawned.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        // 注意：
        // もし生成した子の内部でも「クリックでログ」を出したい場合は、
        // 生成済みプレハブ側に Collider2D + クリック用スクリプトを付けておくこと。
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