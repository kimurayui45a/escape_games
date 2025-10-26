using UnityEngine;

[ExecuteAlways]
public class IsometricCameraSetup : MonoBehaviour
{
    [Header("Grid/Scene fit")]
    public bool fitArea;
    public Vector2 areaSize = new Vector2(20, 12); // 見せたい範囲(幅x高さ, ワールド単位)
    Camera cam;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        // 等角：X=atan(√2) ≒ 35.264389°, Y=45°
        transform.rotation = Quaternion.Euler(35.264389f, 45f, 0f);

        if (fitArea) FitOrthographicSize(areaSize);
    }

    void FitOrthographicSize(Vector2 sizeWH)
    {
        // OrthographicSize は 画面の「縦」半分（ワールド単位）。
        // 幅に合わせる時は アスペクト比で割る。
        float sizeByHeight = sizeWH.y * 0.5f;
        float sizeByWidth = (sizeWH.x * 0.5f) / cam.aspect;
        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
    }
}