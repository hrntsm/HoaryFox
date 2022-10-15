---
id: Changelog
title: Changelog
---

---

## [v2.3.0 - 2022-10-15](https://github.com/hrntsm/HoaryFox/releases/tag/v2.3.0)

### 追加

- 断面の設定（StbSection）に適切な値がない場合、エラーを出力するようにした。

### 変更

- Axes コンポーネントの出力をツリーとした。
  - 階ごとに分けて出力されるようになった。
- stb2brep コンポーネントで中空断面を作成するとき、中空断面の Brep が作成されるようにした。
  - これまでは外形を合わせた矩形中実断面を出力していた。
  - この変更によって LCC 連携などで使用する部材体積の正確性が向上しました。

### 修正

- 部材が null のときエラーになる問題を修正

---

## [v2.2.1 - 2022-3-22](https://github.com/hrntsm/HoaryFox/releases/tag/v2.2.1)

### 修正

- v2.2.0 の変更で壁生成に失敗することがあったため、修正

---

## [v2.2.0 - 2022-3-21](https://github.com/hrntsm/HoaryFox/releases/tag/v2.2.0)

### 追加

- RC 壁の壁開口に対応した。

---

## [v2.1.1 - 2022-1-6](https://github.com/hrntsm/HoaryFox/releases/tag/v2.1.1)

### 追加

- STBDotNet を使ったコーディングのサンプルページの追加

### 変更

- 上記サンプル作成に当たり STBDotNet のバグを見つけたので修正して参照するバージョンを修正後の 0.2.3 に変更

---

## [v2.1.0 - 2021-12-29](https://github.com/hrntsm/HoaryFox/releases/tag/v2.1.0)

### 追加

- LCA 解析への出力のためのフィルター機能を追加
  - MaterialType
  - Story
  - Filter by Material

### 修正

- バージョンごとに整合した組み合わせでないと動かないことがわかったため、マルチターゲットビルドするようにした
- Karamba3D 連携で、断面形状が null のときエラーで止まっていたため修正

### 変更

- Stb to Brep コンポーネントで Brep を作成する方法を変更した。
  - これまでの方法では、体積が負になることがあったため Brep の法線を確認して体積が負になるならば面を反転する処理を追加した。
  - Planer の面にならないスラブに厚さを与えるようにした。

---

## [v2.0.1 - 2021-10-10](https://github.com/hrntsm/HoaryFox/releases/tag/v2.0.1)

### 追加

- ウェブサイトに Algolia を使った検索バーを追加した

### 修正

- デッキスラブとプレキャストスラブが含まれる場合、Brep の作成に失敗し Stb2Brep コンポーネントがエラーになる問題を修正
- 2 断面の梁を Stb2Brep で変換する際のエラーを修正
  - これによって片側ハンチでもそれを反映した Brep が作成されるようになった

---

## [v2.0.0 - 2021-09-17](https://github.com/hrntsm/HoaryFox/releases/tag/v2.0.0)

### 変更

- これまでは ST-Bridge v1 と v2 の両方に対応していたが、本バージョンから v2 のみ対応とした
- Stb2Brep、Stb2Line のコンポーネントの機能を使い Bake した際より多くの情報が Rhino での UserText として出力されるようにした
- Stb2Brep で部材のハンチに対応した Brep が表示されるようにした
- Tag コンポーネントはこれまで 中央断面等の代表断面のみを表示していたが、全ての断面情報が表示されるようにした
  - SRC の場合は、RC 形状、鉄骨形状 ともに出力される
- 壁開口が非対応となった
- Stb2Brep コンポーネントでスラブと壁を一枚のサーフェスではなく Closed Brep で出力するようにした
- HoaryFox v1 のときは、Karamba3D から ST-Bridge に変換した際に、変換状況の概要がパネルで確認できたが、v2 では現状できなくなった

### 追加

- スラブと壁のタグ出力が可能になった
  - SlabNameTag, WallNameTag, SlabSectionTag, WallSectionTag の追加
- 軸の表示機能の追加
  - Axis コンポーネントを使うことで、階情報、軸情報が出力される
- Karamba3D への変化の際、ST-Bridge 内での RC の材料名称に応じて Karamba3D ないでの材質を指定するようにした。
  - 例えば材質が FC21 となっている場合、建築学会の基準に基づき FC21 相当の材料物性が入力される
  - 強度に応じてヤング率、せん断弾性係数、密度が変わる
  - 対応は fc18, 21, 24, 26, 30, 33, 36, 40, 42, 45, 50, 55, 60 でそれ以外は fc21 相当の材質で変換
  - 鋼材は強度以外は材質間で変化ないのですべて SN400 扱い
- Karamba3D データの ST-Bridge への変換出力がこれまでの ST-Bridge v1.4 準拠の内容から v2.0.2 準拠の内容に変更した

### その他

- これまで ST-Bridge の取り扱いには STBDotNet と STBReader 二つのライブラリを使っていたが、STBDotNet に統一した
- GitHub の CodeQL 機能を使用してコードの脆弱性をチェックするようにした

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

- ST-Bridge から Karamba3D への変換で、材料の単位が間違っていたものを修正

### 変更

- ST-Bridge から Karamba3D への変換で、RC の断面名を「Id + 数字」から、「BD- や CD- + 断面サイズ」の表現とした
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

- ST-Bridge への書き出しが可能になった

### その他

- ST-Bridge を扱う部分を [STBDotNet](https://github.com/hrntsm/STBDotNet/tree/main) として分離したライブラリにし、保守性をあげた

---

## [v0.9.0 - v1.1.3](https://github.com/hrntsm/HoaryFox/releases)

上記バージョンは GitHub のリリースページを確認してください。
