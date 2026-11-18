using UnityEngine;

// 試作用プレイヤー：8方向歩行のみ（攻撃・ジャンプ等は入れない）
// 入力はキーボード（矢印/WASD）と VirtualJoystick の合算。
// 移動は XZ 平面。見た目（スプライト）は SpriteFrameAnimator 側が担当する。
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.2f;
    [SerializeField] VirtualJoystick joystick; // 任意。未設定ならキーボードのみ

    CharacterController cc;
    SpriteFrameAnimator anim;

    // 他のスクリプト（波紋など）が参照する現在の状態
    public Vector2 MoveInput { get; private set; }
    public bool IsMoving { get; private set; }

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<SpriteFrameAnimator>();
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (joystick != null && joystick.InputVector.sqrMagnitude > input.sqrMagnitude)
            input = joystick.InputVector;
        input = Vector2.ClampMagnitude(input, 1f);

        MoveInput = input;
        IsMoving = input.sqrMagnitude > 0.001f;

        if (IsMoving)
        {
            // カメラが固定角（斜め見下ろし）前提なので、入力をそのまま XZ に写す
            Vector3 move = new Vector3(input.x, 0f, input.y) * moveSpeed * Time.deltaTime;
            cc.Move(move);
        }

        if (anim != null)
            anim.SetMoveState(input, IsMoving);
    }
}
