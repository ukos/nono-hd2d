using UnityEngine;

// SpriteRenderer は影を落とせないため、同じフレーム画像を持った
// クアッドを重ね、専用シェーダー（NONO/ShadowOnly）で影だけを落とす。
// 配置：スプライトと同じ GameObject の子に、このコンポーネント付きの
// 空オブジェクトを置く。スプライトの Pivot は下中央（0.5, 0）に統一すること。
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SpriteShadowCaster : MonoBehaviour
{
    MeshRenderer mr;
    Material mat;
    static Mesh unitQuad;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        if (unitQuad == null) unitQuad = BuildUnitQuad();
        GetComponent<MeshFilter>().sharedMesh = unitQuad;

        Shader sh = Shader.Find("NONO/ShadowOnly");
        if (sh == null)
        {
            Debug.LogError("NONO/ShadowOnly シェーダーが見つかりません。Shaders/ShadowOnly.shader をプロジェクトに入れてください。");
            enabled = false;
            return;
        }
        mat = new Material(sh);
        mr.material = mat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        mr.receiveShadow = false;
    }

    // フレームが変わるたびに SpriteFrameAnimator から呼ばれる
    public void SetFrame(Sprite sprite, bool flipX)
    {
        if (sprite == null || mat == null) return;
        mat.SetTexture("_MainTex", sprite.texture);

        Rect r = sprite.textureRect;
        Texture t = sprite.texture;
        Vector2 scale = new Vector2(r.width / t.width, r.height / t.height);
        Vector2 offset = new Vector2(r.xMin / t.width, r.yMin / t.height);
        if (flipX) { offset.x += scale.x; scale.x = -scale.x; }
        mat.SetTextureScale("_MainTex", scale);
        mat.SetTextureOffset("_MainTex", offset);

        float w = r.width / sprite.pixelsPerUnit;
        float h = r.height / sprite.pixelsPerUnit;
        transform.localScale = new Vector3(w, h, 1f);
    }

    static Mesh BuildUnitQuad()
    {
        // 原点＝下中央、Y が 0→1 に伸びるクアッド（足元基準）
        var mesh = new Mesh { name = "ShadowQuad" };
        mesh.vertices = new[]
        {
            new Vector3(-0.5f, 0f, 0f), new Vector3(0.5f, 0f, 0f),
            new Vector3(-0.5f, 1f, 0f), new Vector3(0.5f, 1f, 0f),
        };
        mesh.uv = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
        mesh.RecalculateNormals();
        return mesh;
    }
}
