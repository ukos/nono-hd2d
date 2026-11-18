# HD-2D lite 地方データ設計（2026-11-18）

16地方を「1地方ずつの手書きコード」ではなく「データの追加」で増やすための設計。
`lite/index.html` の `REGIONS` が地方定義、`buildRegion()` がデータから地方を構築する。

## 地方定義（REGIONS の1エントリ）

| キー | 内容 |
|---|---|
| label | 地方名 |
| centerX | カメラのパンの基準になる地方の中心X |
| bounds | 歩行可能範囲 {minX, maxX, minZ, maxZ} |
| fog | {color, near, far} |
| bg | 背景色 |
| sun | {i: 強さ, pos, tgt} 太陽光の位置と向き |
| sub | HUD の案内文 |
| spawn | （開始地方のみ）プレイヤーの初期位置 {x, z, row} |
| wadeZ | この Z より北（小さい側）で波紋を出す。浅瀬のない地方は null |
| grounds | 地面プレーンの列（下記） |
| props | 小物の明示配置の列（下記） |
| scatter | 小物の散布ルール（下記） |
| npcs | NPC の列 {sheet, h, x, z, name, lines} |
| fireflies | {n, seed, box:{x:[min,max], y:[...], z:[...]}} |
| exits | 出口の列 {edge:'east'/'west'/'north'/'south', at, to, spawn:{x, z, row, trail}} |
| custom | 専用景観のビルダー名（'lake' 水面・夕日・山ボード／'forest' 奥の樹林ボード）。通常の地方は不要 |

## grounds

- `{tex: 'grass', repeat: [14, 4], w, d, x, y, z}` — tex は `GROUND_TEX` のキー（Canvas生成テクスチャ）
- `{color: 0x1d2b33, w, d, x, y, z}` — 単色プレーン
- 新しい地面の質感が要るときは `GROUND_TEX` に {base, dots, patch} を1つ足す

## props（t の種類と当たり判定）

- pine（松・円錐3段）r=0.34*s+0.08／rock（岩）r=0.5*s+0.12／driftwood（流木・2円）r=0.3
- stump（切り株）r=0.38／signpost（道標）r=0.25／tuft（草むら・ビルボード、判定なし）
- 共通フィールド： {t, x, z, s?, rot?}

## scatter（散布）

`{t:'pine', n, seed, x:[min,max], zCenter, offMin, offMax, s:[min,max]}`
小道（zCenter の直線）の両側へ交互に、offMin〜offMax 離して配置する。seed 固定で再現可能。

## 地方を1つ足す手順

1. REGIONS にエントリを追加（grounds/props/scatter/npcs/fireflies/exits）
2. 隣の地方の exits と対になるよう、双方向の出口を設定
3. NPC が要るならシート画像を lite/assets/ に置く（3方向×3コマ、176×224 セル想定）
4. 専用アート（背景ボード等）が要る地方だけ custom ビルダーを追加

## 決めごと・制約

- 当たり判定と NPC は地方ごとに保持し、表示中の地方のものだけが効く
- 地方の描画物は地方グループにまとめ、非表示地方は描画しない
- 乱数は地方ごとのシード（mulberry32）で再現可能にする
- 太陽の影カメラ範囲は現状 ±18m。大きな地方では影の範囲が足りなくなる場合があり、その地方の大きさに合わせて見直す
- ファイルは単一 index.html のまま（地方が増えて重くなったら分割を検討）
