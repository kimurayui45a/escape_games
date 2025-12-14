using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ScriptableObject(EventData) を使って、
/// ・プレイヤーのパラメータ / 達成済みイベント
/// ・イベントのアンロック条件
/// ・画面(ScreenType)ごとの表示コンポーネント
/// をまとめてチェックし、条件を満たしたイベントを順に開いていくクラス。
/// </summary>
public class ScriptableObjectEvent : MonoBehaviour
{
    /// <summary>
    /// 画面種別ごとに、
    /// ・ScreenType（どの画面か）
    /// ・対応する ScreenEvent（実際にダイアログを表示するコンポーネント）
    /// を紐づけるための構造体。
    /// </summary>
    [System.Serializable]
    public class ScreenComponent
    {
        /// <summary>
        /// このコンポーネントが担当する画面種別。
        /// EventData.ScreenType と対応付けされる。
        /// </summary>
        public ScreenType ScreenType;

        /// <summary>
        /// 該当画面上で表示するイベントダイアログ。
        /// ScreenEvent.Initialize を呼び出して表示する。
        /// </summary>
        public ScreenEvent ScreenEvent;
    }

    /// <summary>
    /// 全イベント定義の配列。
    /// インスペクタで ScriptableObject(EventData) を複数アサインしておく。
    /// </summary>
    [SerializeField]
    EventData[] eventDataArray;

    /// <summary>
    /// プレイヤーのパラメータ・クリア済みイベント情報を持つコンポーネント。
    /// イベント発生条件判定やクリア状態更新に使用する。
    /// </summary>
    [SerializeField]
    PlayerParam playerParam;

    /// <summary>
    /// 画面種別ごとに対応する ScreenEvent を登録しておくための配列。
    /// EventData.ScreenType に応じて、どの ScreenEvent を使うかを判断する。
    /// </summary>
    [SerializeField]
    ScreenComponent[] screenComponents;

    /// <summary>
    /// イベントチェックを開始するトリガーとなるボタン。
    /// （例：ボタンを押すと現在発生可能なイベントを順番に開く）
    /// </summary>
    [SerializeField]
    Button buttonOpen;

    // 追加：保存ボタン
    [SerializeField] Button buttonSave;

    /// <summary>
    /// 初期化。イベントチェック処理をボタンに登録する。
    /// </summary>
    void Start()
    {
        // buttonOpen がクリックされたらイベントチェックを行う
        buttonOpen.onClick.AddListener(() =>
        {
            CheckEvent();
            Debug.Log(Application.persistentDataPath);
        });

        // 追加：保存
        if (buttonSave != null)
        {
            buttonSave.onClick.AddListener(() =>
            {
                playerParam.Save();
            });
        }
    }

    /// <summary>
    /// 発生条件を満たしたイベントがないか順番にチェックし、
    /// 条件を満たすものがあれば対応する ScreenEvent を開く。
    ///
    /// 流れ：
    /// 1. eventDataArray を先頭から順に見る
    /// 2. すでにクリア済みイベントはスキップ
    /// 3. UnlockConditions（他イベントの完了条件）を満たしていないものはスキップ
    /// 4. ParamA 条件（ComparisonType）を満たしているか判定
    /// 5. 条件を満たしたら ScreenType に対応する ScreenEvent を呼び出し、ダイアログを開く
    /// 6. YES/NO の結果に応じてイベントをクリア登録してから、続きのイベントを再チェック
    /// </summary>
    void CheckEvent()
    {
        // 今回の呼び出しで何かしらイベントを開けたかどうか
        var isOpen = false;

        // すべてのイベント定義を順番にチェック
        foreach (var eventData in eventDataArray)
        {
            // 既にクリア済みのイベントは対象外
            if (playerParam.IsCompletedEvent(eventData.EventId))
            {
                // 既にクリア済みのイベントはスキップ
                continue;
            }

            // アンロック条件が設定されている場合のチェック
            if (eventData.UnlockConditions != null)
            {
                // アンロック条件が 1つでも未クリアならロック扱いとする
                var isLocked = false;
                foreach (var unlockCondition in eventData.UnlockConditions)
                {
                    // 条件として登録されたイベントが未完了ならロック
                    if (!playerParam.IsCompletedEvent(unlockCondition.EventId))
                    {
                        isLocked = true;
                        break;
                    }
                }

                // ロック中ならこのイベントはスキップ
                if (isLocked)
                {
                    continue;
                }
            }

            // ParamA に対する条件チェック用フラグ
            var conditionA = false;

            // EventData 側に設定された ComparisonType に応じて、
            // PlayerParam.ParamA と EventData.ConditionDataParamA.Param を比較する
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

            // ParamA の条件を満たしている場合のみ、画面表示処理へ進む
            if (conditionA)
            {
                // screenComponents から、イベントの ScreenType に対応する ScreenEvent を探す
                foreach (var screenComponent in screenComponents)
                {
                    if (screenComponent.ScreenType == eventData.ScreenType)
                    {
                        isOpen = true;

                        // 見つかった ScreenEvent に対して Initialize を呼び出し、
                        // ダイアログを表示する
                        // onClick(bool result) には YES/NO の結果が渡される想定
                        screenComponent.ScreenEvent.Initialize(eventData, (result) =>
                        {
                            if (result)
                            {
                                // YES が押された場合、イベントを「完了済み」として登録
                                playerParam.CompleteEvent(eventData.EventId);
                            }

                            // このイベント処理が終わったので、次に発生可能なイベントがないか再チェック
                            // （連続でイベントが開くことを想定）
                            CheckEvent();
                        });

                        // 対応する ScreenEvent を見つけたので、この内側のループは抜ける
                        break;
                    }
                }
            }

            // 何か1つでもイベントを開いた場合は、
            // この時点で外側のループも抜ける（同時に複数を開かないようにする）
            if (isOpen)
            {
                break;
            }
        }

        // 1件もオープンできるイベントがなかった場合はログを出す
        if (!isOpen)
        {
            Debug.LogError("No Event Opened");
        }
    }
}
