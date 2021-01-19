---
id: ConvertToKaramba
title: Convert to Karamba3D
---

STB のデータを使って Karamba3D のモデルを作成する方法を説明します。こちらの内容は Samples フォルダーの Convert_to_Karamba.gh のモデルを参照してください。

## データの変換

はじめに、[Show ST-Bridge model](./ShowSTBModel) で説明したように STB のデータを Grasshopper で読み込めるようにしてください。  
読み込んだ結果を以下のように Convert to Karamba コンポーネントに入力することでデータの変換が行われます。  
要素と断面が変換対象です。変換したものはそのまま Karamba の Assmeble Model(Karamba3D) コンポーネントの入力にすることができるため、後は Load と Support の設定を行うだけで Karamba で解析が実行可能です。  
材料(Material) の情報は CrossSection の出力に含まれているため、別途設定する必要はありません。


![](../../images/ConvertToKaramba/gh.png)

## 変換の仕様

以下の仕様により STB から Karamba3D にデータを変換しています。

### 変換対象

- 柱、梁、ブレースを変換し、床壁のような面材は変換しません

### 部材名称

- 簡便のために、STB で持っているIDを名前として変換しています。(例 "Id15" のような名前)
- そのため、部材符号は失われます。

### 断面形状

- 複数断面やテーパーをもつ部材でも中央断面のみを持つ部材としてで変換します。
- RC 柱
  - 矩形断面: Trapezoid として変換
  - 円形断面: Karamba3D に円形の中実断面はないため、〇-Section で軸断面積を合わせる形で変換
- RC 梁
  - 全て Trapezoid として変換
- S 部材
  - H 断面: I-Section として変換
  - T 断面: I-Section として変換
  - フラットバー: Trapezoid として変換
  - 箱型断面: []-Section として変換
  - L 断面, C断面, 丸鋼: Karamba3D に対応断面がないこと、かつ基本的にはブレースとして仕様される部材と思われるので、軸断面積を合わせる形で正方形の Trapezoid として変換
    - 背中合わせや2丁づかいには対応していないため、STB 上でそのように設定されていても単独の断面として変換されます。

### 材料
- RC
  - 断面(StbSections) の設定で StbSecColumn_RC, および StbSecBeam_RC で出力されているものを RC として変換しています。
  - 材料は STB で指定可能ですが、材料名の文字列であり、特定の形式で記述されているかわけではないためすべて FC21 相当の材料特性を持つものとしています。
    - ヤング率: 21860 N/mm2
    - ポアソン比: 0.2
    - 密度: 24 kN/m3
    - Karamba の fy: 14 N/mm2
- S
  - 上記以外の断面に対して、鉄の材料を与えています。
  - RC の材料指定と同じ理由で、すべて SN400 相当の材料特性を持つものとしています。
    - ヤング率: 205000 N/mm2
    - ポアソン比: 0.3
    - 密度: 78.5 kN/m3
    - Karamba の fy: 235 N/mm2

:::note

非対応機能も要望があれば適宜対応しますので、Contact よりご連絡ください。

:::