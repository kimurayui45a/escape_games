using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;


/// <summary>
/// 2048ゲームのマネージャー
/// </summary>
public class Game2048Manager : MonoBehaviour
{
    // マス目オブジェクトを認識しておく。タイルの移動でマス目の座標を使う。
    //マス目オブジェクト配列。Unityで16個アサインする。
    [SerializeField] private GameObject[] squares;

    // マス目を二次元配列として認識する。[ , ]ですので注意。
    private GameObject[,] squareArray = new GameObject[4, 4];

    // 生成するタイル関係
    public GameObject tilePrefab;


    // [スワイプ関係]
    private Vector2 touchStartPos;
    // スワイプを検出するための閾値。これより指の動きが小さければ無反応。ご自由に調整してください
    private float swipeThreshold = 80f;
    private bool canPlay = true;
    private bool tapped = false;


    void Start()
    {
        // 4*4のマス配列を作る
        SetSquareArray();

        //２回呼ぶ
        SpawnTile(); 
        SpawnTile();
    }


    void Update()
    {
        // マウスのクリックポジションの取得
        if (Input.GetMouseButtonDown(0) && canPlay)
        {
            touchStartPos = Input.mousePosition;
            tapped = true;
        }
        else if (Input.GetMouseButton(0) && canPlay && tapped)
        {
            Vector2 touchEndPos = Input.mousePosition;
            float swipeX = touchEndPos.x - touchStartPos.x;
            float swipeY = touchEndPos.y - touchStartPos.y;

            Debug.Log(swipeX);
            Debug.Log(swipeY);

            // スワイプの方向を判定。
            // 横方向
            // 左右どちらの動きが大きいかを判定
            //Mathf.Absは絶対値を計算します。
            if (Mathf.Abs(swipeX) > Mathf.Abs(swipeY))
            {
                // 右方向のスワイプ
                //閾値を超えている場合はスワイプとみなす。
                if (swipeX > swipeThreshold)
                {
                    tapped = false;

                    // 右方向のスワイプメソッド
                    RightSwipe();

                }
                // 左方向のスワイプ
                else if (swipeX < -swipeThreshold)
                {
                    tapped = false;

                    // 左方向のスワイプメソッド
                    LeftSwipe();

                }
            }
            // 縦方向
            else
            {
                // 上方向のスワイプ
                if (swipeY > swipeThreshold)
                {

                    tapped = false;

                    // 上方向のスワイプメソッド
                    UpSwipe();

                }
                // 下方向のスワイプ
                else if (swipeY < -swipeThreshold)
                {

                    tapped = false;

                    // 下方向のスワイプメソッド
                    DownSwipe();

                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && tapped)
        {

            tapped = false;

        }
    }


    // 4*4のマス配列を作る
    void SetSquareArray()　
    {
        // 上の行からアサインしていく。０行目が一番上のマス目たち。
        int count = 0;

        for (int i = 0; i < 4; i++) // i行目
        {
            for (int j = 0; j < 4; j++) // ｊ列目
            {
                squareArray[i, j] = squares[count];

                //とりあえず０にする
                squareArray[i, j].GetComponent<SquareScript>().tileValue = 0;

                count++;
            }
        }
    }

    // タイルのセットアップ
    void SpawnTile()
    {
        // 空っぽのマスの座標を取得し、emptyCellに入れる
        var emptyCell = FindEmptySquare();

        // 空のマスにタイルを生成する。タイルのプレファブの位置に生成する
        GameObject newTile = Instantiate(tilePrefab, emptyCell.transform);

        // [SetParent]
        // Unity の Transform クラスのメソッド（UnityEngine.Transform.SetParent）
        // Transform の親子関係を変更するメソッド
        // false：グリッドのセルやUIのコンテナに子したい、親基準で配置し直したい時に使用
        // true：見た目の場所を動かさずに親だけ差し替えたい

        // newTile の親を emptyCell に変更して、ヒエラルキーで emptyCell の子にしている
        newTile.transform.SetParent(emptyCell.transform, false);

        // タイルのサイズや位置に変更を加える
        // newTile（GameObject）から UI 用の RectTransform コンポーネントを取得
        RectTransform newTileRect = newTile.GetComponent<RectTransform>();

        // 新しいタイルのサイズを設定（tilePrefabのサイズをそのままコピー）
        newTileRect.sizeDelta = tilePrefab.GetComponent<RectTransform>().sizeDelta;

        // 新しいタイルを空っぽのマスの上に配置する
        newTileRect.position = emptyCell.GetComponent<RectTransform>().position;

        // タイルの値をランダム生成
        int newValue = Random.Range(1, 3) * 2; // 2 or 4

        //　その数値でもって、タイルの色や値を指定。これは見た目の設定。
        //newTile.GetComponent<Tile>().SetValue(newValue);

        // DOTween で スケールを 0.1 秒かけて 1 にするTweenを作り、newTileにリンク
        newTileRect.DOScale(Vector3.one, 0.1f).SetLink(newTile);

        //　対象のタイルをマス目に認識させる。これは中身の管理。
        emptyCell.GetComponent<SquareScript>().tileValue = newTile.GetComponent<Tile>().value;
        emptyCell.GetComponent<SquareScript>().TileOnMe = newTile;
    }


    // 空のマスのリストを作成する
    GameObject FindEmptySquare()
    {
        List<GameObject> emptySquares = new List<GameObject>();

        // マス配列の中から、valueが０のもの。つまり空っぽのものを探す。
        foreach (var item in squareArray)
        {
            if (item.GetComponent<SquareScript>().tileValue == 0)
            {
                // 空っぽのマスのリストを作る。
                emptySquares.Add(item);
            }
        }

        if (emptySquares.Count > 0)
        {
            // tileValue=0であるitemの中からランダムで1つ選ぶ
            int randomIndex = Random.Range(0, emptySquares.Count);
            GameObject selectedSquare = emptySquares[randomIndex];

            return selectedSquare;
        }

        return null;
    }


    // 元居たマスを空にするメソッド
    void EmptyMySquare(GameObject mySquare)
    {
        //元のマスのバリューを０に。
        mySquare.GetComponent<SquareScript>().tileValue = 0;

        //元いたマスのタイルをnullに。
        mySquare.GetComponent<SquareScript>().TileOnMe = null;
    }



    //タイルの移動。
    void TileMoveAnimation(GameObject temp, GameObject targetSquare, bool Merging)
    {
        RectTransform tempRect = temp.GetComponent<RectTransform>();

        tempRect.SetParent(targetSquare.transform); // Squareの子要素に設定

        // 移動先にあったタイル
        GameObject nextTile = targetSquare.GetComponent<SquareScript>().TileOnMe;

        // Dotween 
        Vector2 endPos = targetSquare.GetComponent<RectTransform>().position;

        bool MergeAnime = false; //※マージする場合用のフラグ
        if (Merging)
        {
            MergeAnime = true;
        }

        tempRect.DOAnchorPos(endPos, 0.1f).SetEase(Ease.Linear).SetLink(temp).OnComplete(() =>
        {
            if (MergeAnime) // 外見に関することを記述。
            {
                int newValue = temp.GetComponent<Tile>().value;

                temp.GetComponent<Tile>().SetValue(newValue * 2);//タイルデータを更新。

                //隣にいたタイルはDestroy。
                Destroy(nextTile);

                //Destroyして空っぽになっているところにtempを入れる。
                targetSquare.GetComponent<SquareScript>().TileOnMe = temp;

                MergeAnime = false;
            }
        });
    }


    // [以下、スワイプメソッド]
    // 右方向のスワイプメソッド
    void RightSwipe()
    {
        bool canMove = false;

        // マージしたかどうかのフラグ
        bool Merging = false;

        // squareArrayを右列から見ていき、タイルが入っているマスがあれば、それを右に移動する。
        // これを端まで繰り返す。
        for (int row = 0; row < 4; row++) // 上の行から
        {
            //一番右の３列目から見て、左の０列まで
            for (int column = 3; column >= 0; column--)
            {
                int myNumber;

                // 対象の列にタイルがあれば。
                if (squareArray[row, column].GetComponent<SquareScript>().tileValue != 0)
                {

                    GameObject temp = squareArray[row, column].GetComponent<SquareScript>().TileOnMe;
                    myNumber = squareArray[row, column].GetComponent<SquareScript>().tileValue;

                    int x = column;

                    //自分より右が空である限り。一番右は見なくてよい。
                    while (x < 3 && squareArray[row, x + 1].GetComponent<SquareScript>().tileValue == 0)
                    {
                        canMove = true;
                        x++;
                    }

                    //  マージできるかどうか。隣が同じ数字、かつ今回のスワイプでマージされたタイルではない。
                    if (x < 3 && squareArray[row, x + 1].GetComponent<SquareScript>().tileValue == myNumber
                        && !squareArray[row, x + 1].GetComponent<SquareScript>().mergedThisSwipe)
                    {

                        Merging = true;
                        canMove = true;

                        x++;　//※

                    }
                    else if (!canMove)　// 隣が空白マスでもなく、マージもできない。
                    {
                        continue;
                    }

                    // いったん置き換えて。
                    GameObject targetSquare = squareArray[row, x];

                    //もと居たマスから移動先のマスへバリューを代入。
                    targetSquare.GetComponent<SquareScript>().tileValue = myNumber;

                    //もと居たマスを空っぽに。
                    EmptyMySquare(squareArray[row, column]);

                    // アニメーション
                    TileMoveAnimation(temp, targetSquare, Merging);

                    //　アニメーションやってる間にこっちが動きますので内部的に必要なことを記述。
                    if (Merging)
                    {
                        targetSquare.GetComponent<SquareScript>().mergedThisSwipe = true;

                        targetSquare.GetComponent<SquareScript>().tileValue = myNumber * 2;

                        Merging = false;
                    }
                    else
                    {
                        targetSquare.GetComponent<SquareScript>().TileOnMe = temp;
                    }

                    // 移動先のマスに、タイルを認識させる。
                    targetSquare.GetComponent<SquareScript>().TileOnMe = temp;

                }
            }
        }

        // タイルが移動した場合は、新しいタイルを生成。
        if (canMove)
        {
            canPlay = false;

            //タイルの移動に0.1fかけるので、それが終わった後で新しいタイルを生成。
            Invoke("SpawnTile", 0.2f);
        }

        // マージ中フラグをリセット
        foreach (var square in squareArray)
        {
            square.GetComponent<SquareScript>().ResetMergeState();

        }

    }


    // 左方向のスワイプメソッド
    void LeftSwipe()
    {
        bool canMove = false;

        // マージしたかどうかのフラグ
        bool Merging = false;

        // squareArrayを右列から見ていき、タイルが入っているマスがあれば、それを右に移動する。
        // これを端まで繰り返す。
        //squareArrayを左列から見ていき、タイルが入っているマスがあれば、それを左に移動する。これを端まで繰り返す。

        for (int row = 0; row < 4; row++) // 上の行から
        {
            for (int column = 0; column < 4; column++) //０列目から3列目まで
            {
                int myNumber;

                if (squareArray[row, column].GetComponent<SquareScript>().tileValue != 0)
                {
                    GameObject temp = squareArray[row, column].GetComponent<SquareScript>().TileOnMe;
                    myNumber = squareArray[row, column].GetComponent<SquareScript>().tileValue;
                    int x = column;

                    while (x > 0 && squareArray[row, x - 1].GetComponent<SquareScript>().tileValue == 0) //自分より左が空である限り。左隣はx-1
                    {
                        canMove = true;

                        x--; // マイナス。
                    }

                    //マージできるかどうか。
                    if (x > 0 && squareArray[row, x - 1].GetComponent<SquareScript>().tileValue == squareArray[row, column].GetComponent<SquareScript>().tileValue
                         && !squareArray[row, x - 1].GetComponent<SquareScript>().mergedThisSwipe)
                    {
                        Merging = true;
                        canMove = true;
                        x--;
                    }

                    else if (!canMove)
                    {
                        continue;
                    }

                    // いったん置き換えて。
                    GameObject targetSquare = squareArray[row, x];

                    //もと居たマスから移動先のマスへバリューを代入。
                    targetSquare.GetComponent<SquareScript>().tileValue = myNumber;

                    //もと居たマスを空っぽに。
                    EmptyMySquare(squareArray[row, column]);

                    // アニメーション
                    TileMoveAnimation(temp, targetSquare, Merging);

                    //　アニメーションやってる間にこっちが動きますので内部的に必要なことを記述。
                    if (Merging)
                    {
                        targetSquare.GetComponent<SquareScript>().mergedThisSwipe = true;

                        targetSquare.GetComponent<SquareScript>().tileValue = myNumber * 2;

                        Merging = false;
                    }
                    else
                    {
                        targetSquare.GetComponent<SquareScript>().TileOnMe = temp;
                    }

                    // 移動先のマスに、タイルを認識させる。
                    targetSquare.GetComponent<SquareScript>().TileOnMe = temp;

                }
            }
        }

        // タイルが移動した場合は、新しいタイルを生成。
        if (canMove)
        {
            canPlay = false;

            //タイルの移動に0.1fかけるので、それが終わった後で新しいタイルを生成。
            Invoke("SpawnTile", 0.2f);
        }

        // マージ中フラグをリセット
        foreach (var square in squareArray)
        {
            square.GetComponent<SquareScript>().ResetMergeState();
        }
    }


    // 上方向のスワイプメソッド
    void UpSwipe()
    {
        bool canMove = false;

        // マージしたかどうかのフラグ
        bool Merging = false;

        // squareArrayを右列から見ていき、タイルが入っているマスがあれば、それを右に移動する。
        // これを端まで繰り返す。

        //squareArrayを左列から見ていき、タイルが入っているマスがあれば、それを左に移動する。これを端まで繰り返す。
        for (int column = 0; column < 4; column++) // 左の列から
        {
            for (int row = 0; row < 4; row++)　//上から下へ順番に。（ちなみに上の行が０）
            {
                int myNumber;
                if (squareArray[row, column].GetComponent<SquareScript>().tileValue != 0) // 何かタイルがあれば。
                {
                    GameObject temp = squareArray[row, column].GetComponent<SquareScript>().TileOnMe;
                    myNumber = squareArray[row, column].GetComponent<SquareScript>().tileValue;
                    int y = row;

                    //自分より上のマスが空である限り
                    while (y > 0 && squareArray[y - 1, column].GetComponent<SquareScript>().tileValue == 0)
                    {
                        canMove = true;

                        y--; // マイナス。
                    }

                    //マージできるかどうか。
                    if (y > 0 && squareArray[y - 1, column].GetComponent<SquareScript>().tileValue == squareArray[row, column].GetComponent<SquareScript>().tileValue
                        && !squareArray[y - 1, column].GetComponent<SquareScript>().mergedThisSwipe)//上が同じ数字ならマージします。
                    {
                        Merging = true;
                        canMove = true;
                        y--;
                    }
                    else if (!canMove)
                    {
                        continue;
                    }

                    // いったん置き換えて。
                    GameObject targetSquare = squareArray[row, y];

                    //もと居たマスから移動先のマスへバリューを代入。
                    targetSquare.GetComponent<SquareScript>().tileValue = myNumber;

                    //もと居たマスを空っぽに。
                    EmptyMySquare(squareArray[row, column]);

                    // アニメーション
                    TileMoveAnimation(temp, targetSquare, Merging);

                    // アニメーションやってる間にこっちが動きますので内部的に必要なことを記述。
                    if (Merging)
                    {
                        targetSquare.GetComponent<SquareScript>().mergedThisSwipe = true;
                        targetSquare.GetComponent<SquareScript>().tileValue = myNumber * 2;
                        Merging = false;

                    }
                    else
                    {
                        targetSquare.GetComponent<SquareScript>().TileOnMe = temp;
                    }

                    // 移動先のマスに、タイルを認識させる。
                    targetSquare.GetComponent<SquareScript>().TileOnMe = temp;

                }
            }
        }

        // タイルが移動した場合は、新しいタイルを生成。
        if (canMove)
        {
            canPlay = false;

            //タイルの移動に0.1fかけるので、それが終わった後で新しいタイルを生成。
            Invoke("SpawnTile", 0.2f);
        }

        // マージ中フラグをリセット
        foreach (var square in squareArray)
        {
            square.GetComponent<SquareScript>().ResetMergeState();
        }

    }


    // 下方向のスワイプメソッド
    void DownSwipe()
    {
        bool canMove = false;

        // マージしたかどうかのフラグ
        bool Merging = false;

        // squareArrayを右列から見ていき、タイルが入っているマスがあれば、それを右に移動する。
        // これを端まで繰り返す。

        //squareArrayを下行から見ていき、タイルが入っているマスがあれば、それを下に移動する。これを端まで繰り返す。
        for (int column = 0; column < 4; column++) // 左の列から
        {
            for (int row = 3; row >= 0; row--) //下から上へ順番に。（ちなみに上の行が０）
            {
                int myNumber;

                if (squareArray[row, column].GetComponent<SquareScript>().tileValue != 0) // 何かタイルがあれば。
                {
                    // いったんタイルを保持して。
                    GameObject temp = squareArray[row, column].GetComponent<SquareScript>().TileOnMe;
                    myNumber = squareArray[row, column].GetComponent<SquareScript>().tileValue;
                    int y = row;

                    //自分より下のマスが空である限り。y+1が自分より下の行。
                    while (y < 3 && squareArray[y + 1, column].GetComponent<SquareScript>().tileValue == 0)
                    {
                        canMove = true;

                        y++;
                    }

                    //マージできるかどうか
                    if (y < 3 && squareArray[y + 1, column].GetComponent<SquareScript>().tileValue == squareArray[row, column].GetComponent<SquareScript>().tileValue
                        && !squareArray[y + 1, column].GetComponent<SquareScript>().mergedThisSwipe)
                    {
                        Merging = true;
                        canMove = true;
                        y++;
                    }
                    else if (!canMove)
                    {
                        continue;
                    }

                    // いったん置き換えて。
                    GameObject targetSquare = squareArray[row, y];

                    //もと居たマスから移動先のマスへバリューを代入。
                    targetSquare.GetComponent<SquareScript>().tileValue = myNumber;

                    //もと居たマスを空っぽに。
                    EmptyMySquare(squareArray[row, column]);

                    // アニメーション
                    TileMoveAnimation(temp, targetSquare, Merging);

                    // アニメーションやってる間にこっちが動きますので内部的に必要なことを記述。
                    if (Merging)
                    {
                        targetSquare.GetComponent<SquareScript>().mergedThisSwipe = true;
                        targetSquare.GetComponent<SquareScript>().tileValue = myNumber * 2;
                        Merging = false;
                    }
                    else
                    {
                        targetSquare.GetComponent<SquareScript>().TileOnMe = temp;
                    }

                    // 移動先のマスに、タイルを認識させる。
                    targetSquare.GetComponent<SquareScript>().TileOnMe = temp;

                }
            }
        }

        // タイルが移動した場合は、新しいタイルを生成。
        if (canMove)
        {
            canPlay = false;

            //タイルの移動に0.1fかけるので、それが終わった後で新しいタイルを生成。
            Invoke("SpawnTile", 0.2f);
        }

        // マージ中フラグをリセット
        foreach (var square in squareArray)
        {
            square.GetComponent<SquareScript>().ResetMergeState();
        }

    }

}