using UnityEngine;
using UnityEngine.EventSystems;

// 画面左下に置く簡易バーチャルスティック（試作用の仮でよい）。
// Canvas 上に「外枠 Image（このコンポーネント）」と「内側ノブ Image」を配置して使う。
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] RectTransform knob;
    [SerializeField] float radius = 60f;

    public Vector2 InputVector { get; private set; }
    RectTransform rect;

    void Awake() { rect = (RectTransform)transform; }

    public void OnPointerDown(PointerEventData e) { OnDrag(e); }

    public void OnDrag(PointerEventData e)
    {
        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, e.position, e.pressEventCamera, out local))
            return;
        Vector2 v = Vector2.ClampMagnitude(local, radius);
        if (knob != null) knob.anchoredPosition = v;
        InputVector = v / radius;
    }

    public void OnPointerUp(PointerEventData e)
    {
        InputVector = Vector2.zero;
        if (knob != null) knob.anchoredPosition = Vector2.zero;
    }
}
