using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FRUITS_TYPE
{
    さくらんぼ = 1,
    いちご,
    ぶどう,
    オレンジ,
    かき,
    りんご,
    なし,
    もも,
    パイナップル,
    メロン,
    すいか,
}

public class Fruits : MonoBehaviour
{
    public FRUITS_TYPE fruitsType;

    // 生成順に振られる通し番号（衝突時に「どちらが処理役になるか」を決めるため）
    private static int fruits_serial = 0;

    // 対象の個体に割り振られた通し番号
    private int my_serial;

    // 既に破棄処理に入っているかどうかのガード
    // 同一衝突で双方の OnCollisionEnter2D が呼ばれた場合の二重実行を防ぐ
    public bool isDestroyed = false;

    // 合体後に生成する次段階のフルーツのプレハブ
    [SerializeField] private Fruits nextFruitsPrefab;

    private void Awake()
    {
        // 生成順にシリアルを付与（静的カウンタをインクリメント）
        my_serial = fruits_serial;
        fruits_serial++;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // 既に破棄処理中なら何もしない（多重呼び出し対策）
        if (isDestroyed)
        {
            return;
        }

        // Fruitsスクリプトを持っているかで判定
        if (other.gameObject.TryGetComponent(out Fruits otherFruits))
        {
            // 相手のフルーツタイプと自分のフルーツタイプが同じなら削除を行う
            if (otherFruits.fruitsType == fruitsType)
            {
                // どちらか一方だけが“処理役”になるように、通し番号の小さい方だけが実行
                // （もう片方は何もしない：二重 Destroy/生成の防止）
                if (my_serial < otherFruits.my_serial)
                {
                    // 両者に「破棄中」フラグを立てて再入を防ぐ
                    isDestroyed = true;
                    otherFruits.isDestroyed = true;

                    // まず現フルーツ2個を破棄
                    Destroy(gameObject);
                    Destroy(other.gameObject);

                    // 上位存在のプレハブがセットされていなかったら削除のみ行う
                    if (nextFruitsPrefab == null)
                    {
                        return;
                    }

                    // 2つの中間位置・中間回転で次段階のフルーツを生成
                    Vector3 center = (transform.position + other.transform.position) / 2;
                    Quaternion rotation = Quaternion.Lerp(transform.rotation, other.transform.rotation, 0.5f);

                    // アタッチされているプレハブを参照し、上位フルーツを表示
                    Fruits next = Instantiate(nextFruitsPrefab, center, rotation);

                    // ２つの速度と角速度の平均をとる
                    Rigidbody2D nextRb = next.GetComponent<Rigidbody2D>();
                    Vector3 velocity = (GetComponent<Rigidbody2D>().linearVelocity + other.gameObject.GetComponent<Rigidbody2D>().linearVelocity) / 2;
                    nextRb.linearVelocity = velocity;

                    float angularVelocity = (GetComponent<Rigidbody2D>().angularVelocity + other.gameObject.GetComponent<Rigidbody2D>().angularVelocity) / 2;
                    nextRb.angularVelocity = angularVelocity;
                }
            }
        }
    }
}