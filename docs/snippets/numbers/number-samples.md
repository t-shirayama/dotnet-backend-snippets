# 数値計算

## 目的

範囲制限、ゼロ除算の回避、割合計算、金額丸め、税込計算など、数値を扱うコードでよく出る小さな処理をまとめます。

## 実装

`src/DotnetBackendSnippets/Numbers/NumberSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Numbers/NumberSamplesTests.cs`

## 使い方

- `Clamp` は値を最小値と最大値の範囲内に収めます。
- `DivideOrDefault` は分母が0のときに既定値を返します。
- `Percentage` は割合をパーセントに変換し、指定桁数で丸めます。
- `RoundCurrency` は金額向けに小数第2位まで丸めます。
- `AddTax` は税率を加算した税込金額を返します。
- `IsBetween` は値が範囲内かどうかを判定します。

## メモ

- 金額には `decimal` を使っています。`double` よりも10進数の丸め誤差を避けやすいためです。
- 丸めは `MidpointRounding.AwayFromZero` を使い、0.005 のような値を直感的に切り上げます。
