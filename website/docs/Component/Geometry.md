---
id: Geometry
title: Geometry
---

読み込んだ STB データから部材を可視化、Bake するコンポーネントのカテゴリ

---

## Stb to Line

![](../../images/Component/StbToLine.png)

部材を Line で表示する

|入力|説明|
|---|:---:|
|Data|Load STB file コンポーネントの Data 出力を入力|
|Bake|各 Line を断面符号ごとにレイヤー分けして Bake する|

|出力|説明|
|---|:---:|
|Nodes| 節点の Point3d のリストを出力|
|Columns| 柱の Line のリストを出力|
|Girders| 大梁の Line のリストを出力|
|Posts| 間柱の Line のリストを出力|
|Beams| 小梁の Line のリストを出力|
|Braces| ブレースの Line のリストを出力|

---

## Stb to Brep

![](../../images/Component/StbToBrep.png)

部材を Brep で表示する

|入力|説明|
|---|:---:|
|Data|Load STB file コンポーネントの Data 出力を入力|
|Bake|各 Brep を断面符号ごとにレイヤー分けして Bake する|

|出力|説明|
|---|:---:|
|Columns| 柱形状を表す Brep のリストを出力|
|Girders| 大梁形状を表す Brep のリストを出力|
|Posts| 間柱形状を表す Brep のリストを出力|
|Beams| 小梁形状を表す Brep のリストを出力|
|Braces| ブレース形状を表す Brep のリストを出力|
|Slabs| スラブ形状を表す Brep のリストを出力|
|Walls| 壁形状を表す Brep のリストを出力|

### 表示仕様

- 壁は開口を含めて出力します
- スラブは凹形状の場合うまく出力されないことがあります
