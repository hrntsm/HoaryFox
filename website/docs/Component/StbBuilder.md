---
id: StbBuilder
title: StbBuilder
---

STB 書き出しのための情報を作成するコンポーネントのカテゴリ

:::note
このコンポーネントの使用方法や注意点は [Export ST-Bridge file](../Usage/ExportSTB) にも詳しく書いていますので、そちらも参照してください。
:::

---

## Frame Builder using Karamba

![](../../images/Component/FrameBuilder.png)

架構情報を Karamba のモデルから作成する

|入力|説明|
|---|:---:|
|Model|Karamba の Model 出力を入力|

|出力|説明|
|---|:---:|
|Node|節点情報（StbNodes）|
|Member|部材情報（StbMembers）|
|Section|断面情報（StbSections）|

---

## Axis Builder

![](../../images/Component/AxisBuilder.png)

軸情報を作成する

|入力|説明|
|---|:---:|
|Node|節点情報、FrameBuilder の Node 出力を入力|
|Distance|原点からの軸の距離（mm）|
|Range|軸の幅（mm）|
|Name|軸の名前|
|Direcrion|軸の方向|

|出力|説明|
|---|:---:|
|Axis|軸情報（StbAxes）|

---

## Story Builder

![](../../images/Component/StoryBuilder.png)

階情報を作成する

|入力|説明|
|---|:---:|
|Node|節点情報、FrameBuilder の Node 出力を入力|
|Height|原点からの階の高さ（mm）|
|Range|階の幅（mm）|
|Name|階の名前|

|出力|説明|
|---|:---:|
|Story|階情報（StbStories）|
