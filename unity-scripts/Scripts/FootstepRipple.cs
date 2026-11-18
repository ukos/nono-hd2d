using System.Collections.Generic;
using UnityEngine;

// 水際（浅瀬ゾーン）歩行中、足元に波紋リングを出す（試作仕様 §3 の演出）。
// 浅瀬ゾーンは Tag "ShallowWater" の Trigger コライダーで判定する。
// 波紋は水平クアッド＋ NONO/RippleRing シェーダーをプールで使い回す。
public class FootstepRipple : MonoBehaviour
{
    [SerializeField] Material rippleMaterial; // NONO/RippleRing を使ったマテリアル
    [SerializeField] float spawnInterval = 0.28f;
    [SerializeField] float lifeTime = 0.9f;
    [SerializeField] float maxRadius = 0.55f;
    [SerializeField] int poolSize = 12;

    PlayerController player;
    readonly List<Ripple> pool = new List<Ripple>();
    bool inShallow;
    float timer;

    class Ripple
    {
        public GameObject go;
        public Material mat;
        public float age = -1f; // 負＝未使用
    }

    void Awake()
    {
        player = GetComponent<PlayerController>();
        if (rippleMaterial == null) { enabled = false; return; }
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Mesh quadMesh = quad.GetComponent<MeshFilter>().sharedMesh;
        Destroy(quad);
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject("Ripple");
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 水平に寝かせる
            var mf = go.AddComponent<MeshFilter>(); mf.sharedMesh = quadMesh;
            var mr = go.AddComponent<MeshRenderer>();
            var m = new Material(rippleMaterial);
            mr.material = m;
            go.SetActive(false);
            pool.Add(new Ripple { go = go, mat = m });
        }
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("ShallowWater")) inShallow = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("ShallowWater")) inShallow = false; }

    void Update()
    {
        // 既存の波紋を進める
        foreach (var r in pool)
        {
            if (r.age < 0f) continue;
            r.age += Time.deltaTime;
            float p = r.age / lifeTime;
            if (p >= 1f) { r.age = -1f; r.go.SetActive(false); continue; }
            r.mat.SetFloat("_Progress", p);
            float s = Mathf.Lerp(0.1f, maxRadius * 2f, p);
            r.go.transform.localScale = new Vector3(s, s, 1f);
        }

        // 浅瀬を歩いている間だけ一定間隔で出す
        if (player == null || !inShallow || !player.IsMoving) return;
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;
        foreach (var r in pool)
        {
            if (r.age >= 0f) continue;
            r.age = 0f;
            Vector3 pos = transform.position; pos.y += 0.02f;
            r.go.transform.position = pos;
            r.go.SetActive(true);
            break;
        }
    }
}
