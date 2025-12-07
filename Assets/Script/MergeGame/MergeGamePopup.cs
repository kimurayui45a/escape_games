using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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


        // 1) 生成したステージの中から GridLayoutGroup を探す
        var grid = spawned.GetComponentInChildren<GridLayoutGroup>(true);
        if (!grid)
        {
            Debug.LogWarning("[MergeGame] ステージ内に GridLayoutGroup が見つかりませんでした。", spawned);
            return;
        }

        Transform itemsRoot = grid.transform;

        // 2) 念のため、Grid の子を全削除（デフォルトで何か入っているなら消して差し替える運用）
        ClearChildren(itemsRoot);

        // 3) ScriptableObject に登録されているアイテムプレハブを並べる
        if (data.mergeGameItemList != null && data.mergeGameItemList.Length > 0)
        {
            foreach (var itemPrefab in data.mergeGameItemList)
            {
                if (!itemPrefab) continue;

                // GridLayoutGroup の子として生成
                var item = Instantiate(itemPrefab, itemsRoot);

                // UI なので localScale / anchoredPosition を初期化しておくと安全
                var itemRect = item.transform as RectTransform;
                if (itemRect != null)
                {
                    itemRect.localScale = Vector3.one;
                    itemRect.anchoredPosition3D = Vector3.zero; // 位置は Grid が制御するので気にしなくてOK
                }
                else
                {
                    item.transform.localScale = Vector3.one;
                    item.transform.localPosition = Vector3.zero;
                }
            }
        }
        else
        {
            Debug.LogWarning($"[MergeGame] mergeGameItemList が空です: {data.mergeGameStageNo}", this);
        }
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