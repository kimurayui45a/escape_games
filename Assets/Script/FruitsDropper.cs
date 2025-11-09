using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsDropper : MonoBehaviour
{
    // ランダムにフルーツを選択するマネージャー
    [SerializeField] private RandomFruitsSelector randomFruitsSelector;

    // 水平移動スピード
    public float moveSpeed = 5f;

    [SerializeField] private float coolTime = 1f;
    private Fruits fruitsInstance;

    private void Start()
    {
        // ゲーム開始後一定時間フルーツがぶら下がっている
        StartCoroutine(HandleFruits(coolTime));
    }

    // ゲーム開始後一定時間フルーツがぶら下がっているメソッド
    private IEnumerator HandleFruits(float delay)
    {
        yield return new WaitForSeconds(delay);

        var fruitsPrefab = randomFruitsSelector.Pop();
        fruitsInstance = Instantiate(fruitsPrefab, transform.position, Quaternion.identity, transform);
        fruitsInstance.transform.SetParent(transform);

        var rb = fruitsInstance.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;  // ← 旧 isKinematic = true の置き換え
        rb.linearVelocity = Vector2.zero;         // 速度リセット
        rb.angularVelocity = 0f;                  // 角速度リセット
    }

    private void Update()
    {
        // Spaceキーでフルーツを生成（現在位置に、回転なし(Quaternion.identity)）
        if (Input.GetKeyDown(KeyCode.Space) && fruitsInstance != null)
        {
            var rb = fruitsInstance.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;   // 旧 isKinematic = false
            fruitsInstance.transform.SetParent(null);
            fruitsInstance = null;
            StartCoroutine(HandleFruits(coolTime));
        }

        // 水平入力の取得（-1,0,1：A/D, ←/→ など）。Time.deltaTimeで毎秒スピードへ換算
        float horizontal = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;

        // 現在のXに加算して、左右端[-2.5, 2.5]の範囲に制限（はみ出し防止）
        float x = Mathf.Clamp(transform.position.x + horizontal, -2.5f, 2.5f);

        // 位置を反映（Y/Zはそのまま）
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}