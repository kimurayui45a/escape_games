using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectClickExample : MonoBehaviour, IPointerClickHandler
{
    // クリックされたときに呼び出されるメソッド
    public void OnPointerClick(PointerEventData eventData)
    {
        print($"オブジェクト {name} がクリックされたよ！");
    }
}