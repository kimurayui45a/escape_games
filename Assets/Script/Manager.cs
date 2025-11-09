using UnityEngine;

public class Manager : SingletonMonoBehaviour<Manager>
{
    [SerializeField]
    TurnManager turnManager;

    // 一意にしたいマネージャーを記述する
    public TurnManager TurnManager => turnManager;
}
