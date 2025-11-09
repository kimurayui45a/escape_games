using UnityEngine;


/// <summary>
/// 2048ゲームのゲームボードを管理するクラス
/// </summary>
public class SquareScript : MonoBehaviour
{
    // このマス目の上にあるタイルを入れる
    public GameObject TileOnMe;

    // 上に乗っているタイルの数字を入れる
    public int tileValue;

    // 今回のスワイプでマージされたときtrue
    public bool mergedThisSwipe = false;

    //フラグをオフにする関数。
    public void ResetMergeState() // publicで。

    {
        mergedThisSwipe = false;
    }
}