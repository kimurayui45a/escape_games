using UnityEngine;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(Collider2D))]
public class DifferenceSpot : MonoBehaviour, IPointerClickHandler
{
    [Header("クリックしたら出すログ文言")]
    [SerializeField] private string logText = "クリック！";

    [Header("クリック時に座標なども一緒に出すか")]
    [SerializeField] private bool includeContext = true;

    // どのポップアップに通知するか
    private DifferenceSpotPopup ownerPopup;

    /// <summary>
    /// Popup 側から呼んで、通知先をセットする
    /// </summary>
    public void Initialize(DifferenceSpotPopup popup)
    {
        ownerPopup = popup;
    }

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

        // 親ポップアップに「このオブジェクトがクリックされた」と通知
        if (ownerPopup != null)
        {
            ownerPopup.OnSpotClicked(gameObject);
        }
    }
}
