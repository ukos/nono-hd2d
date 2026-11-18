# HD-2D 本格版（Unity）フェーズ1 キックオフ指示書（2026-11-18）

ユーザー判断（2026-11-18）：lite 試作の歩行チェックを経て、本格版（Unity）へ進むことが決まった。
この文書は、ユーザーの PC 上で動くエージェント（Claude Code または VS Code + Cline）への作業指示。
nono（Muse）側には Unity 環境がないため、コンパイルとビルドは PC 側で行う。

## ゴール（フェーズ1の範囲）

lite 試作と同じ「湖畔・夕景」1シーンを Unity で再現する。範囲はここまで：

- タイトル表示＋フィールド1画面分
- nono が8方向に歩ける（タッチ仮想スティック＋キーボード両対応）
- ライバル（クロガネ）1体を静止配置
- ゲームロジックは作らない

## 判定項目（フェーズ2の Go/No-Go に使う）

1. 見た目：スクリーンショット1枚で「HD-2D らしい」と言えるか
2. ビルドと Pages の制約：Decompression Fallback 有効化、ビルド総量50MB目安、1ファイル100MB未満、初回ロード時間の実測
3. 性能：iPhone Safari で30fps安定、タブ落ちなし、発熱が許容範囲
4. 操作：タッチとキーボードの両対応
5. 素材流用コスト：既存スプライト取り込み＋アニメ再現の工数を実測

## 準備済みの素材・スクリプト

- Unity スクリプト一式：`~/workspace/your_files/hd2d-unity-scripts/`（nono のワークスペース側）
  - Scripts 7本（PlayerController / SpriteFrameAnimator / CylindricalBillboard / CameraFollow / VirtualJoystick / FootstepRipple / SpriteShadowCaster）
  - Shaders 3本（LakeWater / RippleRing / ShadowOnly）
  - Editor 2本（BuildWebGL / SpriteImportSettings）
  - README.md（組み立て手順）、asset-instructions.md（背景素材の生成指示文）
  - **注意：Unity の無い環境で書いたためコンパイル未検証。エラーが出たら PC 側エージェントが直すか、エラー文を nono に送れば nono が直す**
- 背景素材：lite 用に生成済みの6枚（遠景の山並み・対岸の木々・立ち木・水際の草・小物・ホタルの光点）が流用できる。リポジトリ ukos/nono-playground の `hd2d-test/assets/` にある（透過済み PNG）
- キャラシート：
  - nono：**8方向アトラス `nono_sheet8c.png`**（528×1792、3列×8行）を使う。同じく `hd2d-test/assets/` にある
    - 行の対応：0=南（正面）／1=南東／2=東／3=北東／4=北（背面）／5=北西／6=西／7=南西。各行3コマ（歩行・中間・歩行）
    - 旧3方向シート（nono_sheet.png）は使わない
  - クロガネ：`kurogane_sheet.png`（3方向×3コマ、待機アニメのみでよい）

## lite で調整済みの移動パラメータ（そのまま採用する）

lite 試作で実機チェックを経て決まった値。これが「動く仕様書」としての lite の成果物：

- 移動速度：**2.5 m/s**（3.1 から落として決定）
- 加減速：速度を目標速度へ指数追随（係数は lite の `accel = 1 - pow(0.0025, dt)` 相当）
- 歩行アニメ：時間ではなく**進んだ距離**でコマを進める。1コマあたりの距離は方向別：
  - STEP_LENS（行順 0〜7）= [0.42, 0.40, 0.42, 0.38, 0.42, 0.38, 0.42, 0.40] m
  - コマ順は [0, 1, 2, 1] のループ
- 上下動（ボブ）：方向別の振幅 BOB_AMPS = [0.035, 0.035, 0.035, 0.04, 0.06, 0.04, 0.035, 0.035] m
  - 背面（行4）は足の振りが見えない絵柄のため、上下動を大きくして歩行感を出している
- 向き判定：速度ベクトルの角度（atan2）を45°刻みで8方向に量子化。反転スケールは使わない（行に焼き込み済み）
- 浅瀬（水際）では足元に波紋

## 手順

1. Unity プロジェクトを新規作成（URP テンプレート、WebGL モジュール入り）。バージョンは手元の LTS（2022 LTS 以降）でよい
2. スクリプト一式を Assets 配下へ配置（README の構成どおり）
3. 素材を取り込む：背景6枚＋キャラシート2枚。スプライトのインポート設定は Editor/SpriteImportSettings.cs を使う。キャラシートは「Multiple」分割（3列×8行／クロガネは3×3）
4. README の手順でシーンを組み立てる（地面・水面シェーダー・遠景ボード・ビルボード小物・ホタル・ブルーム＋ACES）
   - 構図の正解は lite 版：https://ukos.github.io/nono-playground/hd2d-test/ を見ながら合わせる
5. PlayerController に8方向対応と上記パラメータを反映する（既存スクリプトが3方向前提なら書き換える）
6. まずエディタ上で「見た目」のスクリーンショットを撮り、ユーザーに見せる（判定項目1）
7. WebGL ビルド（Decompression Fallback 有効）→ 総量と初回ロード時間を実測（判定項目2）
8. 公開して iPhone Safari で fps と発熱を確認（判定項目3・4）。公開先は当面 nono-playground の `hd2d-unity/` を使う（リポジトリ分離はフェーズ3の話）

## 報告してほしいこと

- コンパイルエラーの有無と、直した内容
- 見た目のスクリーンショット
- ビルド総量・初回ロード時間・iPhone Safari の fps
- スプライト1枚の取り込み＋アニメ再現にかかった実工数（判定項目5）
