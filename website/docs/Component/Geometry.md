---
id: Geometry
title: Geometry
---

読み込んだ ST-Bridge データから部材を可視化、Bake するコンポーネントのカテゴリ

---

## Stb to Line

![](../../images/Component/StbToLine.png)

部材を Line で表示する

| 入力       |                        説明                        |
| ---------- | :------------------------------------------------: |
| Data       |   Load STB file コンポーネントの Data 出力を入力   |
| OffsetNode |          部材端部のオフセットの考慮の有無          |
| Bake       | 各 Line を断面符号ごとにレイヤー分けして Bake する |

| 出力    |              説明              |
| ------- | :----------------------------: |
| Nodes   | 節点の Point3d のリストを出力  |
| Columns |    柱の Line のツリーを出力    |
| Girders |   大梁の Line のツリーを出力   |
| Posts   |   間柱の Line のツリーを出力   |
| Beams   |   小梁の Line のツリーを出力   |
| Braces  | ブレースの Line のツリーを出力 |
| Piles   |    杭の Line のツリーを出力    |

### 備考

- 杭についてはオフセットの有無で杭長が変わらないように出力しているため、オフセットを False にした場合、杭先端がオフセット分だけ異なる位置に表示されます。

---

## Stb to Brep

![](../../images/Component/StbToBrep.png)

部材を Brep で表示する

| 入力 |                        説明                        |
| ---- | :------------------------------------------------: |
| Data |   Load STB file コンポーネントの Data 出力を入力   |
| Bake | 各 Brep を断面符号ごとにレイヤー分けして Bake する |

| 出力     |                   説明                   |
| -------- | :--------------------------------------: |
| Log      |            変換結果のログ出力            |
| Columns  |     柱形状を表す Brep のツリーを出力     |
| Girders  |    大梁形状を表す Brep のツリーを出力    |
| Posts    |    間柱形状を表す Brep のツリーを出力    |
| Beams    |    小梁形状を表す Brep のツリーを出力    |
| Braces   |  ブレース形状を表す Brep のツリーを出力  |
| Slabs    |   スラブ形状を表す Brep のツリーを出力   |
| Walls    |     壁形状を表す Brep のツリーを出力     |
| Piles    |     杭形状を表す Brep のツリーを出力     |
| Footings | フーチング形状を表す Brep のツリーを出力 |

### 表示仕様

- Log は GH コンポーネントと同じフォルダに変換ごとにファイルとして保存されます。
- 床の開口は反映されません。
- 平面でないスラブでは近似した面を張るため処理が重い場合があります。
- SRC,CFT は S と RC を一体化した一つの Brep として作成されます。
- C 断面の背中合わせは H 型、顔合わせはボックス型として出力されます。
- L 断面の背中合わせは T 型、顔合わせは C 型として出力されます。

---

## Axis

![](../../images/Component/Axis.png)

階と軸をラインで表示する

| 入力   |                      説明                      |
| ------ | :--------------------------------------------: |
| Data   | Load STB file コンポーネントの Data 出力を入力 |
| Factor |                軸線の長さの比率                |
| Size   |   軸名、階名の文字のサイズ。デフォルトは 12    |

| 出力 |           説明           |
| ---- | :----------------------: |
| Axis | 軸と階を示す Line の出力 |
