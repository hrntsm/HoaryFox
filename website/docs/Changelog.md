---
id: Changelog
title: Changelog
---

---

## [v1.3.1 - 2021-03-28](https://github.com/hrntsm/HoaryFox/releases/tag/v1.3.1)

### 追加

- Brep と Line を Rhino へ情報を持った形で Bake できるようにした。

### 変更

- Stb2Brep コンポーネントで出力される Brep を部材ごとに Closed Brep になるようにした。

---

## [v1.2.2 - 2021-02-17](https://github.com/hrntsm/HoaryFox/releases/tag/v1.2.2)

### 追加

- 本ドキュメントサイトの公開
- SetCroSecFamilyName コンポーネントによって、Karamba3D への変換で断面の Family 名を指定できるようにした

### 修正

- Stb から Karamba3D への変換で、材料の単位が間違っていたものを修正

### 変更

- Stb から Karamba3D への変換で、RC の断面名を「Id + 数字」から、「BD- や CD- + 断面サイズ」の表現とした
  - 例えば BD-300x600 のような形式
- FrameBuilder コンポーネントを FrameBuilder by angle コンポーネントと NodeBuilder コンポーネントに分けた
  - もともとは 45 度で柱と梁を区別していたが、この変更により入力した角度で区別するようにした
  - それに伴い、Rhino のビューポート状に柱、梁、ブレースのどの区分になっているかテキストで表示するようにした

---

## [v1.2.1 - 2020-12-31](https://github.com/hrntsm/HoaryFox/releases/tag/v1.2.1)

### 修正

- Karamba3D へ変換するコンポーネントがうまく動かなかったため、内部での Karamba3D への参照パスを変更し動作するようにした

---

## [v1.2.0 - 2020-12-30](https://github.com/hrntsm/HoaryFox/releases/tag/v1.2.0)

### 追加

- Stb への書き出しが可能になった

### その他

- Stb を扱う部分を [STBDotNet](https://github.com/hrntsm/STBDotNet/tree/main) として分離したライブラリにし、保守性をあげた

---

## [v0.9.0 - v1.1.3](https://github.com/hrntsm/HoaryFox/releases)

上記バージョンは GitHub のリリースページを確認してください。
