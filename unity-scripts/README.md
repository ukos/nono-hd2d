# HD-2D試作「湖畔」スクリプト一式 ― 組み立て手順

対象：Unity（LTS）＋ URP の新規プロジェクト。仕様書 `hd2d-waterside-prototype-spec-2026-11-17.md` と検証リスト `hd2d-prototype-checklist-2026-11-17.md` に対応する実装部品です。
nono は Unity を実行できないため、**組み立てとビルドは PC 側（Claude Code 等）で行ってください**。このフォルダのファイルはすべてテキストなので、プロジェクトへコピーするだけで使えます。

## ファイル構成

```
Scripts/
  PlayerController.cs      8方向歩行（キーボード＋仮想スティック）
  SpriteFrameAnimator.cs   既存シートのフレーム送り（方向別）
  CylindricalBillboard.cs  スプライトを Y 軸だけカメラへ向ける
  SpriteShadowCaster.cs    スプライトの形の影を落とすプロキシ
  CameraFollow.cs          固定角・緩やか追従カメラ
  VirtualJoystick.cs       タッチ用仮想スティック（Canvas UI）
  FootstepRipple.cs        浅瀬歩行時の足元波紋（プール方式）
Shaders/
  LakeWater.shader         軽量な湖面（反射なし、ゆらぎ＋フレネル＋グリント）
  RippleRing.shader        波紋リング
  ShadowOnly.shader        影だけ描く専用シェーダー
Editor/
  BuildWebGL.cs            WebGL ビルド（Decompression Fallback 自動設定）
  SpriteImportSettings.cs  Art/Sprites への自動取り込み設定
```

配置先：`Scripts/` と `Shaders/` は `Assets/` 直下（または任意のフォルダ）、`Editor/` の2ファイルは必ず **`Assets/Editor/`** に置くこと。

## 1. プロジェクト作成

1. Unity Hub で新規プロジェクト（3D URP テンプレート、LTS版）
2. この一式を上記の配置でコピー
3. URP アセット設定で **Depth Texture は任意**（本シェーダー群は未使用）、**Shadow の距離**はシーンが収まる範囲（例：30）に設定

## 2. 素材の取り込み

1. 既存スプライトシートを `Assets/Art/Sprites/` に置く（`SpriteImportSettings` が Point フィルタ・無圧縮を自動適用）
2. Sprite Editor でフレームを切り分け、各フレームの **Pivot を下中央（0.5, 0）** にする（影プロキシと位置を合わせるため）
3. 歩行シートから方向別にフレームを控える（下／上／左／右。各列の先頭＝待機コマ）

## 3. シーン「Lakeside」の組み立て

1. **地面**：岸のジオラマを Cube や ProBuilder で作る（歩ける平地＋水際の段差）。歩行範囲の外周に透明な壁コライダーを置く
2. **水面**：Plane を水位の高さに置き、`NONO/LakeWater` のマテリアルを適用。色は夕景に合わせて調整
3. **浅瀬ゾーン**：水際の帯に Box Collider（Is Trigger）を作り、Tag を `ShallowWater` に設定（なければ Tag を追加）
4. **遠景**：山並みシルエットの板ポリや既存背景を奥に配置。手前には草・小石の小物を配置（ぼかし確認用）
5. **光**：Directional Light を夕日の色・低い角度にし、Shadow を有効化。地面と小物は影を受ける設定に
6. **ポストプロセス**：Global Volume を作り、初期値は仕様書 §4（Bloom 弱め／Depth of Field は焦点をキャラ列に／Tonemapping = ACES）
7. **キャラクター（nono）**：
   - 空オブジェクト `Player`（CharacterController、PlayerController、FootstepRipple）
   - 子に `Visual`（CylindricalBillboard、SpriteRenderer、SpriteFrameAnimator）
   - さらに子に `Shadow`（MeshFilter、MeshRenderer、SpriteShadowCaster）※メッシュは実行時に自動生成
   - SpriteFrameAnimator の各方向にフレーム配列を設定
   - FootstepRipple の rippleMaterial に `NONO/RippleRing` のマテリアルを設定
8. **ライバル1体**：同じ構造で PlayerController だけ外し、静止配置（影と並びの確認用）
9. **カメラ**：Main Camera に CameraFollow を付け、target に Player、pitch 38° / distance 11 を初期値に
10. **タッチ UI**：Canvas にスティックの外枠 Image（VirtualJoystick）と内側ノブ Image を左下に配置し、PlayerController の joystick に外枠を割り当て

## 4. ビルドと公開

1. メニュー **NONO > Build WebGL**（またはバッチモードで `BuildWebGL.Run`）
   - Decompression Fallback と Brotli 圧縮が自動で設定される
2. 出力（`Build/WebGL`）を `nono-playground/hd2d-test/` にコピーして push
3. iPhone Safari で開き、検証リスト §2〜§3（ロード時間、fps、タブ落ち、発熱）を実測して記録

## 既知の割り切り（試作用）

- 入力は旧 Input Manager 前提（Input System パッケージは使わない）
- 水面はリアルタイム反射なし。物足りなければ仕様書 §3 の段階を上げる
- 影プロキシのバイアスは URP の標準設定に依存する。影が浮く／埋まる場合は Light の Shadow Bias を調整
- キャラクターの接地は CharacterController の高さで合わせる（足元＝ルート位置）
