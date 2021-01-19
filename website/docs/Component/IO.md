---
id: IO
title: IO
---

データの入力出力を行うコンポーネントのカテゴリ

---

## Load STB file

![](../../images/Component/LoadStbFile.png)

STB データの読み込み

|入力|説明|
|---|:---:|
|Path|ST-Bridge ファイルのパス|

|出力|説明|
|---|:---:|
|Data|読み取った ST-Bridge ファイルのデータ|

---

## Export STB file

![](../../images/Component/ExportStbFile.png)

STB データの書き出し

|入力|説明|
|---|:---:|
|Node|節点情報, FrameBuilder コンポーネントの Node 出力を入力|
|Axis|軸情報, AxisBuilder コンポーネントの Axis 出力を入力|
|Story|階情報, StoryBuilder コンポーネントの Story 出力を入力|
|Member|部材情報, FrameBuilder コンポーネントの Member 出力を入力|
|Section|断面情報, FrameBuilder コンポーネントの Section 出力を入力|
|Path|作成した ST-Bridge データの出力先を指定。デフォルト値はデスクトップです|
|Out?|出力を行うかのブール値。True にすると出力します|

|出力|説明|
|---|:---:|
|Stb|作成した ST-Bridge データ|

こちらのコンポーネントの細かいの使い方・変換仕様は、Usage の [Export ST-Bridge file](../Usage/ExportSTB) の記事も参照してください。

---

## Convert to karamba

![](../../images/Component/ConvertToKaramba.png)

読み込んだ STB データを 構造解析を行うコンポーネント [Karamba3D](https://www.karamba3d.com/) のデータに変換

|入力|説明|
|---|:---:|
|Data|Load STB file コンポーネントの Data 出力を入力|

|出力|説明|
|---|:---:|
|ElementBeam|Karamba3D の梁要素|
|CrossSection|Karamba3D の断面情報|

こちらのコンポーネントの細かいの使い方・変換仕様は、Usage の [Convert to Karamba3D](../Usage/ConvertToKaramba) の記事も参照してください。