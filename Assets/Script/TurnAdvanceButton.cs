using UnityEngine;

public class TurnAdvanceButton : MonoBehaviour
{
    [SerializeField] int step = 1; // 1ターン進める(変更可)

    public void Advance()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.AdvanceTurn(step);
        else
            Debug.LogWarning("TurnManager.Instance が見つかりません");
    }
}
