# HoaryFox

[![License](https://img.shields.io/github/license/hrntsm/HoaryFox)](https://github.com/hrntsm/HoaryFox/blob/master/LICENSE)
[![Release](https://img.shields.io/github/v/release/hrntsm/HoaryFox)](https://github.com/hrntsm/HoaryFox/releases)
[![download](https://img.shields.io/badge/download-Food4Rhino-lightgray)](https://www.food4rhino.com/app/hoaryfox)

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/c0a462728dce4983802d447ed67d3e7c)](https://www.codacy.com/gh/hrntsm/HoaryFox/dashboard?utm_source=github.com&utm_medium=referral&utm_content=hrntsm/HoaryFox&utm_campaign=Badge_Grade)
[![Maintainability](https://api.codeclimate.com/v1/badges/bc78a575fcf5e9448929/maintainability)](https://codeclimate.com/github/hrntsm/HoaryFox/maintainability)

HoaryFox は 建築構造架構の可視化ツールです。  
ST-Bridge file(.stb) を読み込んで、表示をします。  
対応するデータの形式を一貫構造計算ソフトから出力されてる stb データとすることで、使用しているソフトに限らずモデルの生成ができ、かつ可視化用に別途モデリングする必要がないことが特徴です。  
Rhino を使わないスタンドアロンのビューアが必要な場合は [STEVIA](https://github.com/hrntsm/STEVIA-Stb2U/wiki) を使ってみてください。  
モデルを Karamba3D へ変換する機能、および Karamba3D のモデルを ST-Bridge データとして出力する機能を追加しました。

## Install

1. Grasshopper 内で, File > Special Folders > Components folder を選択し、HoaryFox のフォルダをそこに保存します。
2. コンポーネントを右クリックし プロパティの中で、ブロックしない に設定する
3. Rhino を再起動する。

詳しいインストール方法は [ドキュメントサイトのインストール](https://hiron.dev/HoaryFox/docs/Usage/HowToInstall) を参照してください。

## Usage

STB Loader コンポーネントに ST-Bridge データのパスを入力することデータを Grasshopper 向けに変換し、それを各コンポーネント渡すことでコンポーネントに応じた図化がされます。  
HoaryFox ver1.1 より ST-Bridge ver2.0 に対応しました。  
Samples のフォルダ内にあるサンプルの Grasshopper データを参照してください。

詳しい使い方は [ドキュメントサイト](https://hiron.dev/HoaryFox/) を参照してください。

## Karamba3D Integration

### Karamba3D への入力

ST-Bridge データの Karamba3D へのコンバートは、梁要素のみ対応です。床や壁などの面材は非対応です。  
L 型断面、C 型断面は Karamba3D が非対応な断面形状なため、等価な軸断面積の矩形断面に置換しています。

### Karamba3D モデルの ST-Bridge データ化

部材の配置および断面情報を Karamba3D のデータを使用して作成します。
上記と合わせて軸、階の情報を入力することで、ST-Bridge データの作成し出力を行います。
手元のモデルでは以下のソフトで読み込めることを確認しています。（読み込みには各社が開発している専用のアドインが必要になります。）

- ARCHICAD
- REVIT

## About ST-Bridge

ST-Bridge の規格を作成している [building SMART Japan の構造設計小委員会様](https://www.building-smart.or.jp/meeting/buildall/structural-design/) での記載を引用します。

> ST-Bridge とは・・・日本国内の建築構造分野での情報交換のための標準フォーマット
>
> - 利用範囲を明確にすることによって、IFC よりシンプルで扱い易い
> - 日本独自の表現方法を取り込む（通り芯、部材配置と断面符号、配筋情報）
> - 国内の構造系アプリ、躯体積算アプリ、3 次元オブジェクト CAD との連携を目指す

## Contact information

[![Twitter](https://img.shields.io/twitter/follow/hiron_rgkr?style=social)](https://twitter.com/hiron_rgkr)

- HP : [https://hiron.dev/](https://hiron.dev/)
- Mail : support(at)hrntsm.com
  - (at) を @ に置き換えてください

## Donation

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/G2G5C2MIU)

上記、または [PixivFANBOX](https://hiron.fanbox.cc/) よりドネートしていただけると開発のモチベーションが上がります。

## License

HoaryFox is licensed under the [MIT](https://github.com/hrntsm/HoaryFox/blob/master/LICENSE) license.  
Copyright© 2019-2021, hrntsm
