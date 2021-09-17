---
id: HowToInstall
title: How to install
---

## インストール方法

HoaryFox のインストール方法について紹介します。

1. Food4Rhino の [HoaryFox](https://www.food4rhino.com/app/hoaryfox) のサイトからファイルをダウンロード
    - HoaryFox のバージョン 2 以降は ST-Bridge のバージョン 2 のみの対応となります。
    - ST-Bridge のバージョン 1 のデータを扱う場合は HoaryFox のバージョン 1.3 を使用してください。
1. ダウンロードした zip ファイルを右クリックしプロパティから全般のタブの中にあるセキュリティの項目を「許可する」にし、zip を解凍する
1. Grasshopper を起動して File > Special Folders > Components folder を選択することで表示されるフォルダに、1. でダウンロードしたファイルの中にある 「HoaryFox」 という名前のフォルダを入れる
1. Rhino を再起動する

## 動作環境

対応OS
- Windows
- Mac 
  - Apple silicon での動作は未確認
  - Karamba3D 連携については、Mac 版の Karamba3D がないため動きません

対応バージョン
- Rhino6
- Rhino7

## Karamba3D との連携

Karamba3D との連携機能を使う場合は Karamba.gha と同じフォルダに HoaryFox のフォルダを移動してください。  
Karamba.gha はデフォルトでは C:\Program Files\Rhino xx\Plugins\Karamba にあります。  

:::note
連携が必要ない場合は、HoaryFoxのフォルダ内にある karambaConnect.gha ファイルを削除するとコンポーネントの読み込みエラーを回避できます。
:::

## 各ソフトとの連携の概要

HoaryFox は下図のように ST-Bridge を介して Rhinoceros/Grasshopper/Karamba3D と BIM ソフト、一貫構造計算との連携を行うことができます。

詳細は各コンポーネントの紹介ページを参照してください。

![](../../images/HowToInstall/relation.png)