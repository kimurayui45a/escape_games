using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ITEMS_TYPE
{
    さくらんぼ = 1,
    いちご,
    ぶどう,
    オレンジ,
    かき,
    りんご,
    なし,
    もも,
    パイナップル,
    メロン,
    すいか,
}

public class MergeGameItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("このフルーツの種類")]
    public ITEMS_TYPE fruitsType;

    [Header("合体後に生成する次段階のフルーツプレハブ")]
    [SerializeField] private MergeGameItem nextFruitsPrefab;

    [Header("合体とみなす距離（UI上の判定）")]
    [SerializeField] private float mergeDistance = 0.5f; // 今は width 判定で実質使っていないが一旦そのまま

    // ドラッグ開始時の位置（戻す用）
    private Vector3 startPosition;

    // キャッシュ用：ステージ
    private MergeGameStageScript stage;

    private void Awake()
    {
        // 自分より上の階層からステージスクリプトを探してキャッシュ
        stage = GetComponentInParent<MergeGameStageScript>();
        if (stage == null)
        {
            Debug.LogWarning($"[MergeGameItem] 親に MergeGameStageScript が見つかりません: {name}", this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        transform.SetAsLastSibling();  // UI 的に最前面へ
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MergeGameItem target = FindMergeTarget();

        if (target != null)
        {
            MergeWith(target);
        }
        else
        {
            transform.position = startPosition;
        }
    }

    private MergeGameItem FindMergeTarget()
    {
        MergeGameItem[] allFruits = FindObjectsByType<MergeGameItem>(FindObjectsSortMode.None);
        MergeGameItem best = null;
        float bestDist = float.MaxValue;

        foreach (var f in allFruits)
        {
            if (f == this) continue;
            if (f.fruitsType != fruitsType) continue;

            var rt1 = transform as RectTransform;
            var rt2 = f.transform as RectTransform;
            if (rt1 == null || rt2 == null) continue;

            float width1 = rt1.sizeDelta.x * 0.5f;
            float width2 = rt2.sizeDelta.x * 0.5f;

            float dist = Vector2.Distance(transform.position, f.transform.position);
            Debug.Log($"dist to {f.name} = {dist}");

            // 幅の合計以内なら「重なった」とみなす
            if (dist < (width1 + width2) && dist < bestDist)
            {
                bestDist = dist;
                best = f;
            }
        }

        return best;
    }

    /// <summary>
    /// 指定された MergeGameItem（other）とマージし、
    /// 次段階のアイテム（nextFruitsPrefab）を生成する。
    /// ・this … ドラッグしていた側（呼び出し元）
    /// ・other … ドロップされて「マージ対象」になった側
    /// </summary>
    private void MergeWith(MergeGameItem other)
    {
        // 次の段階のプレハブが設定されていない場合は、マージ処理ができないので
        // 警告を出してドラッグしていた自分（this）のみ削除して終了する
        if (nextFruitsPrefab == null)
        {
            Debug.LogWarning($"[MergeGameItem] nextFruitsPrefab が未設定のため、マージできません: {name}", this);
            Destroy(gameObject);
            return;
        }

        // 生成するオブジェクトの親（親Transform）を決める
        // 基本ルール：
        //   ・ステージ側(MergeGameStageScript)が ItemsRoot を持っていれば、それを親にする
        //   ・なければ、フォールバックとして「other と同じ親（同じスロット階層）」を使う
        Transform parent = null;
        if (stage != null && stage.ItemsRoot != null)
        {
            // ステージの ItemsRoot (= GridImage) を親にする
            parent = stage.ItemsRoot;
            Debug.Log($"[MergeGameItem] use stage.ItemsRoot: {stage.ItemsRoot.name}");
        }
        else
        {
            // ステージ情報が取れない場合の保険：マージ対象と同じ親を利用
            parent = other.transform.parent;
            Debug.Log($"[MergeGameItem] fallback to other.parent: {other.transform.parent.name}");
        }

        // マージ対象の RectTransform を取得（位置情報を引き継ぐため）
        var otherRect = other.transform as RectTransform;

        // ここで一旦、元の2つのアイテムを削除する
        // ・other … マージされる側（ドロップされた側）
        // ・this  … ドラッグしていた側
        Destroy(other.gameObject);   // された側
        Destroy(gameObject);         // 自分

        // 次の段階のアイテムを生成する
        // 親を指定して Instantiate することで、GridImage(ItemsRoot) の子として生成される
        var next = Instantiate(nextFruitsPrefab, parent);

        // 生成したオブジェクトの RectTransform を取得して、位置・スケールを調整する
        var nextRect = next.transform as RectTransform;
        if (otherRect != null && nextRect != null)
        {
            // まずアンカーとピボットを揃える
            nextRect.anchorMin = otherRect.anchorMin;
            nextRect.anchorMax = otherRect.anchorMax;
            nextRect.pivot = otherRect.pivot;

            // サイズや回転・スケールも必要に応じて揃える
            nextRect.sizeDelta = otherRect.sizeDelta;
            nextRect.localRotation = otherRect.localRotation;
            nextRect.localScale = otherRect.localScale;

            // 最後に anchoredPosition をコピー
            nextRect.anchoredPosition3D = otherRect.anchoredPosition3D;
        }
        // デバッグ用ログ：どの親の下に、どのオブジェクトとしてマージされたかを出力
        Debug.Log($"[MergeGameItem] Merged into {next.name} under parent {parent.name}");
    }
}
