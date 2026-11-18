using UnityEngine;

// HD-2D 用固定角カメラ：斜め見下ろしの角度を保ったまま、
// 対象へ緩やかに追従する（即時追従にしない＝試作仕様 §2）。
public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float pitchDegrees = 38f;   // 見下ろし角
    [SerializeField] float distance = 11f;       // 対象からの距離
    [SerializeField] float followSharpness = 4f; // 追従の鋭さ（大きいほど速く寄る）
    [SerializeField] float lookHeight = 1.0f;    // 注視点の高さ

    void LateUpdate()
    {
        if (target == null) return;
        Quaternion rot = Quaternion.Euler(pitchDegrees, 0f, 0f);
        Vector3 focus = target.position + Vector3.up * lookHeight;
        Vector3 desired = focus - rot * Vector3.forward * distance;
        float k = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desired, k);
        transform.rotation = rot;
    }
}
