# nono-hd2d

「NONO: Blade of the Dawn」の HD-2D 版プロジェクト置き場。
現行の Web 版（nono-playground）とは別枠の実験プロジェクトです。

## lite 試作（ブラウザ版）

Three.js（WebGL）で HD-2D の見た目を再現した試作。Unity 本格版の「動く仕様書」として育てています。

- プレイ: https://ukos.github.io/nono-hd2d/lite/
- コード: `lite/index.html`（単一ファイル）＋ `lite/assets/`
- 内容: 湖畔・夕景と森の小道の2エリア、8方向歩行（nono／ユイ／レイ＋クロガネ）、仲間の追従、立体小物と当たり判定、会話

## Unity 本格版

- スクリプト一式: `unity-scripts/`（C# 7本、シェーダー3本、Editor 補助2本、組み立て手順 README）
- フェーズ1（湖畔1シーン試作）のキックオフ指示書: `docs/hd2d-unity-kickoff-2026-11-18.md`
- スクリプトは Unity の無い環境で作成したためコンパイル未検証です

## ドキュメント

- `docs/hd2d-roadmap-2026-11-18.md` — 全体ロードマップと用語整理
- `docs/hd2d-prototype-checklist-2026-11-17.md` — 試作の検証リスト
- `docs/hd2d-waterside-prototype-spec-2026-11-17.md` — 湖畔試作の仕様書
- その他、8方向アトラスの生成指示文など

## 経緯

2026-11-18 に nono-playground の `hd2d-test/` から移管しました（旧 URL は新しい場所へ転送されます）。
