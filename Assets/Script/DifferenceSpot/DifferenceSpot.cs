using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DifferenceSpot : MonoBehaviour, IPointerClickHandler
{
    [Header("クリックしたら出すログ文言")]
    [SerializeField] private string logText = "クリック！";

    [Header("クリック時に座標なども一緒に出すか")]
    [SerializeField] private bool includeContext = true;

    /// <summary>
    /// EventSystem + Physics2DRaycaster 経由のクリックコールバック
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        if (includeContext)
        {
            Debug.Log($"[DifferenceSpot] {logText} | obj='{name}' | worldPos={transform.position} | btn={eventData.button}");
        }
        else
        {
            Debug.Log(logText);
        }
    }
}
