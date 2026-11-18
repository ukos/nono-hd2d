using UnityEngine;

// 既存ドット絵シートのフレームを方向別に切り替える軽量アニメーター。
// Animator/AnimationClip を作らず、Sprite 配列を時間で送るだけにして
// 「素材流用の工数」を測りやすくする（試作仕様 §5）。
// 向きは4方向（下=手前 / 上=奥 / 左 / 右）。左右は反転で兼用も可。
public class SpriteFrameAnimator : MonoBehaviour
{
    [Header("フレーム（各方向の歩行コマ列。先頭を待機コマとして使う）")]
    [SerializeField] Sprite[] down;
    [SerializeField] Sprite[] up;
    [SerializeField] Sprite[] left;
    [SerializeField] Sprite[] right; // 空なら left を左右反転して使う
    [SerializeField] float framesPerSecond = 8f;

    SpriteRenderer sr;
    SpriteShadowCaster shadowCaster;
    Sprite[] currentSet;
    float timer;
    int frameIndex;
    bool moving;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        shadowCaster = GetComponentInChildren<SpriteShadowCaster>();
        currentSet = down;
        ApplyFrame(0);
    }

    public void SetMoveState(Vector2 input, bool isMoving)
    {
        moving = isMoving;
        if (isMoving)
        {
            Sprite[] next = PickSet(input);
            if (next != null && next != currentSet)
            {
                currentSet = next;
                frameIndex = 0;
                timer = 0f;
            }
        }
    }

    Sprite[] PickSet(Vector2 input)
    {
        bool hasRight = right != null && right.Length > 0;
        bool hasLeft = left != null && left.Length > 0;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (input.x > 0f) // 右向き
            {
                if (hasRight) { sr.flipX = false; return right; }
                sr.flipX = true; return left; // 左コマを反転して代用
            }
            // 左向き
            if (hasLeft) { sr.flipX = false; return left; }
            sr.flipX = true; return right; // 右コマを反転して代用
        }
        sr.flipX = false;
        return input.y > 0f ? up : down;
    }

    void Update()
    {
        if (currentSet == null || currentSet.Length == 0) return;
        if (!moving)
        {
            ApplyFrame(0); // 待機は先頭コマ
            return;
        }
        timer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, framesPerSecond);
        if (timer >= interval)
        {
            timer -= interval;
            frameIndex = (frameIndex + 1) % currentSet.Length;
            ApplyFrame(frameIndex);
        }
    }

    void ApplyFrame(int i)
    {
        if (currentSet == null || i >= currentSet.Length) return;
        Sprite s = currentSet[i];
        if (s == null) return;
        sr.sprite = s;
        if (shadowCaster != null) shadowCaster.SetFrame(s, sr.flipX);
    }
}
