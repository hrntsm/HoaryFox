---
id: GettingStartSTBCoding
title: Getting Start
---

## 環境構築

HoaryFox は ST-Bridge のデータの処理に .NET のライブラリである STBDotNet を使っています。
そちらをダウンロードして ST-Bridge をコードから扱う準備をしましょう。

STBDotNet は OSS で開発しているライブラリになります。
コードの中身が気になる方は、以下 GitHub のページから確認してください。

- [STBDotNet](https://github.com/hrntsm/STBDotNet)

公式のドキュメントサイトは以下になります。

- [ドキュメントサイト](https://hiron.dev/STBDotNet/docs/index.html)

### Rhino 向け

Rhino の中のコードエディタや Grasshopper の各スクリプトのコンポーネントから使用したい場合は、nuget のサイトから必要なバージョンをダウンロードしてください。
STBDotNet の ページは以下です。

- [nuget/STBDotNet](https://www.nuget.org/packages/STBDotNet/)

:::note
nuget とは C# などが動作している .NET 向けのライブラリが複数あげられているサイトです。
パッケージマネージャーと言われます。
Food4Rhino の C# 版というようなイメージで間違いありません。
:::

### IDE 向け

パッケージマネージャーを使って nuget から参照してください。
以下では VisualStudio を使った場合の例をあげます。

## ST-Bridge データをコードから読み込む

```cs
using STBDotNet;

class Program
{
    static void Main(string[] args)
    {
        string stbPath = "Set Your STB Path";
        var model = STBDotNet.Serialization.Serializer.Deserialize(stbPath, STBDotNet.Enums.Version.Stb202);

        Console.WriteLine(model);
    }
}
```
