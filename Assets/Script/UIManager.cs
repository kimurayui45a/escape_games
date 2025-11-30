using UnityEngine;


public class UIManager : MonoBehaviour
{

    public void ItemBook() {
        PopupManager.Instance?.Show(PopupType.ItemsBookPopup, "ItemsBook");
    }

}
