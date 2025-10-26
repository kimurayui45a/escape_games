using UnityEngine;

public class QuarterViewFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 12, -12); // 斜め上後方
    public float pitch = 30f;  // X
    public float yaw = 45f;    // Y
    public bool snapToWhole = true; // 位置の丸めで“ピタッ”

    void LateUpdate()
    {
        if (!target) return;

        // 方向固定（X=Pitch, Y=Yaw）
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // 望遠っぽくしたいなら FOV を 25-35 に
        var cam = GetComponent<Camera>();
        if (cam && !cam.orthographic) cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 25f, 35f);

        // 位置決め（ターゲット＋オフセットをワールドに回転して適用）
        Vector3 worldOffset = transform.rotation * offset;
        Vector3 pos = target.position + worldOffset;

        if (snapToWhole)
        {
            // 0.5単位や1.0単位に丸めるとタイル/ピクセル合わせが楽
            float step = 0.5f;
            pos.x = Mathf.Round(pos.x / step) * step;
            pos.y = Mathf.Round(pos.y / step) * step;
            pos.z = Mathf.Round(pos.z / step) * step;
        }
        transform.position = pos;
    }
}
