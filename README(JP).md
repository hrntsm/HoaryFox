# HoaryFox

[![Release](https://img.shields.io/github/v/release/hrntsm/HoaryFox)](https://github.com/hrntsm/HoaryFox/releases)
[![License](https://img.shields.io/github/license/hrntsm/HoaryFox)](https://github.com/hrntsm/HoaryFox/blob/master/LICENSE)
[![download](https://img.shields.io/badge/download-Food4Rhino-lightgray)](https://www.food4rhino.com/app/hoaryfox)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/c0a462728dce4983802d447ed67d3e7c)](https://www.codacy.com/gh/hrntsm/HoaryFox/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=hrntsm/HoaryFox&amp;utm_campaign=Badge_Grade)
[![Maintainability](https://api.codeclimate.com/v1/badges/bc78a575fcf5e9448929/maintainability)](https://codeclimate.com/github/hrntsm/HoaryFox/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/bc78a575fcf5e9448929/test_coverage)](https://codeclimate.com/github/hrntsm/HoaryFox/test_coverage)

HoaryFox は 建築構造架構の可視化ツールです。  
ST-Bridge file(.stb) を読み込んで、表示をします。  
対応するデータの形式を一貫構造計算ソフトから出力されてるstbデータとすることで、使用しているソフトに限らずモデルの生成ができ、かつ可視化用に別途モデリングする必要がないことが特徴です。  
Rhinoを使わないスタンドアロンのビューアが必要な場合は [STEVIA](https://github.com/hrntsm/STEVIA-Stb2U/wiki) を使ってみてください。  
試験的な機能としてモデルをKarambaへ変換する機能を追加しました。梁要素、断面情報がKarambaのモデルとしてst-bridgeデータから変換されます。

## Install

1. Grasshopper内で, File > Special Folders > Components folderを選択し、gha fileをそこに保存します。
   + 上記にかかわらず、Karambaへのコンバートを使う場合は、Karamba.ghaと同じフォルダに保存することを推奨します
2. コンポーネントを右クリックし プロパティの中で、ブロックしない に設定する
3. Rhinoを再起動する。

## Usage

STB Loaderコンポーネントにst-bridgeデータのパスを入力することデータをGrasshopper向けに変換し、それを各コンポーネント渡すことでコンポーネントに応じた図化がされます。  
HoaryFox ver1.1 より ST-bridge ver2.0 に対応しました。  
詳しくは Samples のフォルダ内の参考データを参照してください。

## Convert to karamba

Karambaへのデータのコンバートは、梁要素のみ対応です。床や壁などの面材は非対応です。  
L型断面、C型断面はKarambaが非対応な断面形状なため、等価な軸断面積の矩形断面に置換しています。
Karambaへのコンバートを使う場合は、Karamba.ghaと同じフォルダにHoaryFox.ghaに保存してください。

## About ST-Bridge

ST-Bridgeの規格を作成している [building SMART Japan の構造設計小委員会様](https://www.building-smart.or.jp/meeting/buildall/structural-design/) での記載を引用します。

> ST-Bridgeとは・・・日本国内の建築構造分野での情報交換のための標準フォーマット
>
> + 利用範囲を明確にすることによって、IFCよりシンプルで扱い易い
> + 日本独自の表現方法を取り込む（通り芯、部材配置と断面符号、配筋情報）
> + 国内の構造系アプリ、躯体積算アプリ、3次元オブジェクトCADとの連携を目指す


## Contact information

[![Twitter](https://img.shields.io/twitter/follow/hiron_rgkr?style=social)](https://twitter.com/hiron_rgkr)
+ HP : [https://hrntsm.github.io/](https://hrntsm.github.io/)
+ Blog : [https://rgkr-memo.blogspot.com/](https://rgkr-memo.blogspot.com/)
+ Mail : support(at)hrntsm.com
  + (at) を @ に置き換えてください
  
## License

HoaryFox is licensed under the [MIT](https://github.com/hrntsm/HoaryFox/blob/master/LICENSE) license.  
Copyright© 2019-2020, hrntsm
