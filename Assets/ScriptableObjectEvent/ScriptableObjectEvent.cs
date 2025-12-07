using UnityEngine;
using UnityEngine.UI;

public class ScriptableObjectEvent : MonoBehaviour
{
    [System.Serializable]
    public class ScreenComponent
    {
        public ScreenType ScreenType;
        public ScreenEvent ScreenEvent;
    }
    [SerializeField]
    EventData[] eventDataArray;
    [SerializeField]
    PlayerParam playerParam;
    [SerializeField]
    ScreenComponent[] screenComponents;
    [SerializeField]
    Button buttonOpen;

    void Start()
    {
        buttonOpen.onClick.AddListener(() =>
        {
            CheckEvent();
        });
    }

    void CheckEvent()
    {
        var isOpen = false;
        foreach (var eventData in eventDataArray)
        {
            if (playerParam.IsCompletedEvent(eventData.EventId))
            {
                // 既にクリア済みのイベントはスキップ
                continue;
            }
            if (eventData.UnlockConditions != null)
            {
                // アンロック条件がある場合、未クリアの条件があればスキップ
                var isLocked = false;
                foreach (var unlockCondition in eventData.UnlockConditions)
                {
                    if (!playerParam.IsCompletedEvent(unlockCondition.EventId))
                    {
                        isLocked = true;
                        break;
                    }
                }
                if (isLocked)
                {
                    continue;
                }
            }

            var conditionA = false;
            switch (eventData.ConditionDataParamA.ComparisonType)
            {
                case EventData.ComparisonType.Equal:
                    conditionA = playerParam.ParamA == eventData.ConditionDataParamA.Param;
                    break;
                case EventData.ComparisonType.Greater:
                    conditionA = playerParam.ParamA > eventData.ConditionDataParamA.Param;
                    break;
                case EventData.ComparisonType.GreaterEqual:
                    conditionA = playerParam.ParamA >= eventData.ConditionDataParamA.Param;
                    break;
            }

            if (conditionA)
            {
                foreach (var screenComponent in screenComponents)
                {
                    if (screenComponent.ScreenType == eventData.ScreenType)
                    {
                        isOpen = true;
                        screenComponent.ScreenEvent.Initialize(eventData, (result) =>
                        {
                            if (result)
                            {
                                // イベントクリア処理
                                playerParam.CompleteEvent(eventData.EventId);
                            }
                            CheckEvent();
                        });
                        break;
                    }
                }
            }
            if (isOpen)
            {
                break;
            }
        }
        if (!isOpen)
        {
            Debug.LogError("No Event Opened");
        }
    }

}
