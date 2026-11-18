using UnityEngine;

// Y軸だけカメラに向ける（円柱ビルボード）。
// HD-2D の作法：スプライトは地面に垂直に立て、水平方向だけカメラへ向ける。
// カメラが固定角の試作でも、追従で角度が微妙に変わるため入れておく。
public class CylindricalBillboard : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        if (Camera.main != null) cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;
        Vector3 toCam = cam.position - transform.position;
        toCam.y = 0f;
        if (toCam.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
    }
}
