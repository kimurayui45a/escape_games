using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurnCounterText : MonoBehaviour
{
    [SerializeField] Text targetText;
    [SerializeField] string format = "Turn: {0}";

    Coroutine _waitBind;

    void OnEnable()
    {
        if (!targetText) targetText = GetComponent<Text>();
        targetText.text = string.Format(format, 0);

        TryBind(); // まず即時トライ
        if (TurnManager.Instance == null)
            _waitBind = StartCoroutine(WaitAndBind()); // 後から生成される場合に備える
    }

    void OnDisable()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTurnChanged -= OnTurnChanged;

        if (_waitBind != null)
            StopCoroutine(_waitBind);
    }

    void TryBind()
    {
        if (TurnManager.Instance == null) return;
        TurnManager.Instance.OnTurnChanged -= OnTurnChanged; // 二重防止
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;

        // 現在値で即更新
        targetText.text = string.Format(format, TurnManager.Instance.CurrentTurn);
    }

    IEnumerator WaitAndBind()
    {
        yield return new WaitUntil(() => TurnManager.Instance != null);
        TryBind();
    }

    void OnTurnChanged(int turn)
    {
        targetText.text = string.Format(format, turn);
    }
}
