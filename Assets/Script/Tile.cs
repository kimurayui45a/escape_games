using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// タイルの色と値を管理するデータクラス。
/// </summary>
[System.Serializable]
public class TileData
{
    public Color color;
    public int value;
}


/// <summary>
/// タイルの管理クラス。
/// </summary>
public class Tile : MonoBehaviour
{
    // 色と値のデータの配列。上で作ったTileDataのこと。Unityインスペクターで設定。
    public TileData[] tileData;

    // タイルの色
    public Image imageColor;

    //このタイルの値。上のTileDataとは別物だけど、リンクする。
    public int value;

    // タイルの値を表示する。
    public TMP_Text valueText;


    /// <summary>
    /// タイルの値を設定するメソッド(Game2048Managerから呼び出す)
    /// </summary>
    /// <param name="newValue">新規の値</param>
    public void SetValue(int newValue)
    {
        // 与えられた値に対応するデータを取得
        TileData data = GetTileData(newValue);

        // データが存在すれば、色や値を設定
        if (data != null)
        {
            imageColor.color = data.color; // 色を設定
            value = data.value; // 値を設定
            valueText.text = value.ToString(); // プレファブのテキストを設定
        }
    }


    /// <summary>
    /// 対象の値に対応するデータを取得するメソッド
    /// </summary>
    /// <param name="targetValue">対象の値</param>
    TileData GetTileData(int targetValue)
    {
        foreach (TileData data in tileData)
        {
            if (data.value == targetValue)

            {
                return data;
            }
        }
        // 対応するデータが見つからない場合
        return null;
    }

}