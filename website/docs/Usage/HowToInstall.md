---
id: HowToInstall
title: How to install
---

## インストール方法

HoaryFox のインストール方法について紹介します。

1. Food4Rhino の [HoaryFox](https://www.food4rhino.com/app/hoaryfox) のサイトからファイルをダウンロード
1. ダウンロードした zip ファイルを右クリックしプロパティから全般のタブの中にあるセキュリティの項目を「許可する」にし、zip を解凍する
1. Grasshopper を起動して File > Special Folders > Components folder を選択することで表示されるフォルダに、1. でダウンロードしたファイルの中にある 「HoaryFox」 という名前のフォルダを入れる
1. Rhino を再起動する

## Karamba3D との連携

Karamba3D との連携機能を使う場合は Karamba.gha を管理者権限のないフォルダに移動してください。Karamba3D のフォルダは通常 Program flies > Rhino > Plug-ins にあります。

:::note
連携が必要ない場合は、HoaryFoxのフォルダ内にある karambaConnect.gha ファイルを削除するとコンポーネントの読み込みエラーを回避できます。
:::

## 各ソフトとの連携の概要

HoaryFox は下図のように ST-Bridge を介して Rhinoceros/Grasshopper/Karamba3D と BIM ソフト、一貫構造計算との連携を行うことができます。

詳細は各コンポーネントの紹介ページを参照してください。

![](../../images/HowToInstall/relation.png)