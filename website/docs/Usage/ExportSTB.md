---
id: ExportSTB
title: Export ST-Bridge file
---

Karamba3D のデータから ST-Bridge のデータを作成する方法を説明します。こちらの内容は Samples フォルダーの ExportSTB.gh のモデルを参照してください。  
ST-Bridge の version 2.0.3 の形式で出力を行います。

## Karamba3D モデルの変換

Karamba3D で作成したモデルから以下を作成します。

- 節点（StbNodes）
- 部材情報（StbMembers）
- 断面情報（StbSections）

以下のように Karamba3D の AssembleModel コンポーネントなどから出力される Model のデータを FrameBuilder by angle コンポーネントと NodeBuilder に入力することでデータを変換します。  
変換された部材が、柱・梁・ブレースのどれに変換されたかがテキストで Rhino のビューポートに表示されます。柱と梁は部材の角度で分類しているため、想定の区分でなかった場合は Angle の入力に適切な角度を入力してください。

![](../../images/ExportStb/FrameBuilder.png)

:::note
Karamba3D からの出力に際して、モデル化の注意点についてはこのページの下部の変換仕様を確認してください
:::

## 軸情報の作成

AxisBuilder コンポーネントで軸（StbAxes）のデータを作成します。  
基本的な挙動の考えとしては、Distance で指定した軸から Range の幅（Dist ± Range）にある節点を軸に属する節点として処理します。  

入力の仕様は以下です。
リストで入力し、同一のインデックスでの入力をマッチさせて変換します。

- Node: 節点情報です。NodeBuilder の出力の Node を入れてください
- Distance: 原点からの軸の距離を指定してください。
  - 軸の方向は全体座標系での X 軸または Y 軸に平行になります。
  - 指定した Distance が X 方向か Y 方向かについては Direction の入力で指定します
- Range: Distance で設定した軸に対して節点を所属させる幅を指定します。
  - 節点座標を浮動小数点で持っている関係上、たとえ全ての節点が軸上のある場合でも 0 よりも大きな値を設定することを推奨します。
- Name: 軸の名前になります。
- Direction: 軸の方向を指定します。
  - 0 は X 方向、1 は Y 方向の軸として処理します。

![](../../images/ExportStb/AxisBuilder.png)

## 階情報の作成

StoryBuilder コンポーネントで階（StbStories）のデータを作成します。  
基本的な挙動は AxisBuilder コンポーネントと同様です。  

- Node: 節点情報です。NodeBuilder の出力の Node を入れてください。
- Height: 階高の情報です。原点からの高さを入力してください。
- Range: Height で指定した階に対して節点を所属させる幅を指定します。
- Name: 階の名前になります。

![](../../images/ExportStb/StoryBuilder.png)

## データの出力

上記の 3 つで変換したデータを全て Export STB file コンポーネントに入力することでデータをまとめて 1 つの ST-Bridge ファイルを作成します。  
Path で指定したパスに ST-Bridge ファイルを出力します。指定しない場合、デスクトップに model.stb というファイルで出力されます。  
Out? の値を True にするとファイルが出力されます。

![](../../images/ExportStb/ExportStbfile.png)

---

## 変換の仕様

以下の仕様により ST-Bridge データの作成を行います。

### 対象

- ST-Bridge version 2.0.3 の形式で出力します。
- 柱、梁、ブレースを変換し、床壁のような面材は変換しません

:::note
ST-Bridge v2.0.3 の計算編の出力には非対応です。
:::

### 部材の判別

- Karamba3D のモデルでは、柱梁ブレースの区別がないため以下の仕様で判定しています
  - Karamba3D でトラス要素として扱っているものはブレースとして変換
  - 全体座標系の Z 軸に対しての FrameBuilder の Angle に入力された部材の角度未満である部材は柱、それ以上の場合は梁として変換
- 部材の判別は ST-Bridge における StbMember 内での各表現に対応します（例えば StbColumn など）
- ST-Bridge の梁部材には、部材が基礎部材かのフラグ（isFoundation）があるが全て False で出力します

### 材質の判別

- Karamba3D の 材料を作成する際に設定する Family の名前が "Steel" の場合は鉄骨部材、"Concrete" の場合は 鉄筋コンクリート部材とします
- 材質の判別は ST-Bridge における StbSections 内での各表現に対応します（例えば StbSecColumn_RC など）

:::important
材料の Family 名が上記以外の場合エラーになります
:::

### 部材名称

- Karamba3D 内での名称は使用しません
- Karamba3D が内部的に持っている部材の順番で、柱ならば C、梁ならば G、ブレースならば V と数字の組み合わせで名称を付けます。（C1, G1 など）

### 断面名称

- Karamba3D の Cross Section コンポーネントの Name で設定した名称で断面を作成します

:::important
Name の設定ごとに ST-Bridge ファイルに出力しているため、必ず Name を設定するようにしてください  
Name が重複している場合、HoaryFox のコンバーターでは同一断面として処理するため、1 つの断面しか出力されません。
:::

### 断面形状

- Karamba3D で設定した断面の形状に合わせて ST-Bridge に出力します
- 変換にエラーがある時は、10mm の角材として出力します
- RC 断面は Karamba3D では配筋情報を持たないため鉄筋情報はありません。
- S 断面の場合、材質はフランジ、ウェブともに SN400 として出力します

:::note
非対応機能も要望があれば適宜対応しますので、Contact よりご連絡ください。
:::
