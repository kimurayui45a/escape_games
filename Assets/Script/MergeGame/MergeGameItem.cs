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

    [Header("合体とみなす距離（ワールド座標）")]
    [SerializeField] private float mergeDistance = 80f;

    // ドラッグ開始時の位置（戻す用）
    private Vector3 startPosition;

    /// <summary>
    /// ドラッグ開始時に呼ばれる
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 今の位置を保存しておく（合体しなかったら戻す用）
        startPosition = transform.position;
    }

    /// <summary>
    /// ドラッグ中に呼ばれる
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    /// <summary>
    /// ドラッグ終了時に呼ばれる
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // ドロップ位置付近に「同種のフルーツ」がいるか探す
        MergeGameItem target = FindMergeTarget();

        if (target != null)
        {
            // 見つかったら合体処理
            MergeWith(target);
        }
        else
        {
            // 見つからなければ元の位置に戻す
            transform.position = startPosition;
        }
    }

    /// <summary>
    /// 自分の近くに「同じ種類」のフルーツがいるか探す
    /// </summary>
    private MergeGameItem FindMergeTarget()
    {
        MergeGameItem[] allFruits = FindObjectsByType<MergeGameItem>(FindObjectsSortMode.None);
        MergeGameItem best = null;
        float bestDist = float.MaxValue;

        foreach (var f in allFruits)
        {
            if (f == this) continue;                         // 自分自身は無視
            if (f.fruitsType != fruitsType) continue;       // 種類が違うものは対象外

            float dist = Vector2.Distance(transform.position, f.transform.position);
            Debug.Log($"dist to {f.name} = {dist}");

            if (dist < mergeDistance && dist < bestDist)
            {
                bestDist = dist;
                best = f;
            }
        }

        return best;    // 条件を満たす最近傍が返る（いなければ null）
    }

    /// <summary>
    /// 指定のフルーツと合体して次の段階を生成
    /// </summary>
    private void MergeWith(MergeGameItem other)
    {
        // 上位フルーツが設定されていない場合は、
        // ドラッグしている自分だけ消して終了
        if (nextFruitsPrefab == null)
        {
            Destroy(gameObject);          // 自分だけ削除
            return;
        }

        // 生成位置は「された側（other）」の位置だけ使う
        Vector3 spawnPos = other.transform.position;

        // もとの2つを削除
        Destroy(other.gameObject);        // された側
        Destroy(gameObject);              // ドラッグしていた自分

        // 回転は一切付けない（Quaternion.identity）
        Instantiate(nextFruitsPrefab, spawnPos, Quaternion.identity);
    }
}
