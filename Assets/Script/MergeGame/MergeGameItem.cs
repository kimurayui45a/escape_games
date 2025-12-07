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

    private void MergeWith(MergeGameItem other)
    {
        if (nextFruitsPrefab == null)
        {
            Debug.LogWarning($"[MergeGameItem] nextFruitsPrefab が未設定のため、マージできません: {name}", this);
            Destroy(gameObject);
            return;
        }

        // 親は基本的に「ステージの ItemsRoot (= GridImage)」
        Transform parent = null;
        if (stage != null && stage.ItemsRoot != null)
        {
            parent = stage.ItemsRoot;
        }
        else
        {
            // 念のためフォールバック：された側と同じ親
            parent = other.transform.parent;
        }

        var otherRect = other.transform as RectTransform;

        // 元2つを削除
        Destroy(other.gameObject);   // された側
        Destroy(gameObject);         // 自分

        // 親を指定して生成（GridImage の子として出る）
        var next = Instantiate(nextFruitsPrefab, parent);

        var nextRect = next.transform as RectTransform;
        if (otherRect != null && nextRect != null)
        {
            // GridImage の座標系は同じなので、そのスロット位置をそのまま引き継ぐ
            nextRect.anchoredPosition = otherRect.anchoredPosition;
            nextRect.localScale = otherRect.localScale;
        }

        Debug.Log($"[MergeGameItem] Merged into {next.name} under parent {parent.name}");
    }
}
