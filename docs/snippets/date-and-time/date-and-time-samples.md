# 日付操作

## 目的

日の開始・終了、月初・月末、週末判定、年齢計算、日数差分など、日付処理でよく使う基本パターンをまとめます。

## 実装

`src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/DateAndTime/DateAndTimeSamplesTests.cs`

## 使い方

- `StartOfDay` は対象日の 00:00:00 を返します。
- `EndOfDay` は対象日の最後の tick を返します。tick は .NET の日時の最小単位です。
- `StartOfMonth` と `EndOfMonth` は月初・月末の日付を返します。
- `IsWeekend` は土曜日または日曜日かを判定します。
- `CalculateAge` は誕生日と基準日から年齢を計算します。
- `DaysBetween` は2つの日付の差を日数で返します。

## メモ

- 日付だけを扱う処理には `DateOnly` を使っています。時刻やタイムゾーンを混ぜたくない場合に便利です。
- 2月29日生まれの年齢計算では、うるう年でない年の誕生日を2月28日として扱います。
