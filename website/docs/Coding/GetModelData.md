---
id: GetModelData
title: Get model data & draw
---

ここでは節点と部材の接続情報を取得して、Rhino 上にラインを書き出す方法について紹介します。

## ST-Bridge のデータの構造について

初めに、どこに情報があるか探しやすくするために、ST-Bridge のデータ構造について簡単に説明します。
基本的なイメージとしては一貫構造計算ソフトに近いものと考えていただくのが早いと思います。

モデルの位置、断面情報は、`StbModel` に含まれ、以下のような構成になっています。
部材の情報を取得したければ StbMember、断面の情報を取得したければ StbSections を使用する形になります。

```
ST_BRIDGE
└───StbModel
    ├───StbAxes
    ├───StbJoints
    ├───StbMembers
    ├───StbNodes
    ├───StbSections
    └───StbStories
```

各データ間の参照は id によって行っています。  
例えば`柱 A` は`節点 1-2` 間を繋ぎ、`断面番号 5` の断面を持つといった形で StbMember はデータを保持しています。
ですので、柱 A の節点座標を知りたければ、 StbNodes の id が 1 と 2 のものを探すことになります。

## 節点情報の取得

部材を構成する最も基本的な要素である節点の一覧を取得します。
節点情報は以下で取得できます。

```cs
// model は v202.ST_BRIDGE 型のインスタンス
StbNode[] nodes = model.StbModel.StbNodes;
```

配列で [StbNode](https://hiron.dev/STBDotNet/docs/STBDotNet.v202.StbNode.html) が取得できます。

StbNode は節点の id や座標情報を持つクラスになっています。
仕様上は配列のインデックスと StbNode がもつ id は一致することを強制していないので、部材の節点を参照する際は必ず id を使って参照してください。
id が "1" の節点情報を取得する際は例えば Linq を使うと以下のように書くことができます。

```cs {1,4}
// using System.Linq をあらかじめしてください。
// model は v202.ST_BRIDGE 型のインスタンス
StbNode[] nodes = model.StbModel.StbNodes;
StbNode node1 = nodes.FirstOrDefault(n => n.id == "1");
```

id は数字で与えられている場合が多いですが、データの形式は文字列になっていますので注意してください。

### Point3d で図化

Rhino の Point3d として可視化したい場合は以下のように取得した節点情報から Point3d を作成することで可能です。

```cs {4}
// using System.Linq をあらかじめしてください。
// model は v202.ST_BRIDGE 型のインスタンス
StbNode[] nodes = model.StbModel.StbNodes;
Point3d[] pts = nodes.Select(n => new Point3d(n.x, n.y, n.z)).ToArray();
```

節点の一覧が取得できたので、次に部材情報を取得します。

## 部材情報の取得

### Line での図化
