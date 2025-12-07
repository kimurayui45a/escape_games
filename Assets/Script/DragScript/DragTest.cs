using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragTest : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("ドラッグ中");

        // マウスに合わせてドラッグ(Canvasがスクリーンスペース-オーバーレイの場合)
        transform.position = eventData.position;
    }

    // Canvas上のPlayerタグを持つuGUIにあたると消える
    public void OnEndDrag(PointerEventData eventData)
    {
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (var hit in raycastResults)
        {
            if (hit.gameObject.CompareTag("Player"))
            {
                // 処理
                Destroy(gameObject);
            }
        }

    }
}