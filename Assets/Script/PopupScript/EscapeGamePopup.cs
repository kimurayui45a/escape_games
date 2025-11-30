using TMPro;
using UnityEngine;

public class EscapeGamePopup : PopupViewScript
{

    // ‹¤’Ê‚µ‚È‚¢‰º‚Ì—v‘f‚ÍŒp³æ‚É‚½‚¹‚é
    [SerializeField] private TMP_Text titleText;

    public override void Setup(string id)      // © override & object ó‚¯
    {
        var title = id as string;              // Šú‘ÒŒ^‚ÉƒLƒƒƒXƒg
        if (titleText) titleText.text = title ?? string.Empty;
    }
}
