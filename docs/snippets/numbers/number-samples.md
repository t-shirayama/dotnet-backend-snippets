# 数値計算

## 目的

範囲制限、ゼロ除算の回避、割合計算、金額丸め、税込計算など、数値を扱うコードでよく出る小さな処理をまとめます。

## 実装

- `src/DotnetBackendSnippets.Core/Numbers/NumberSamples.cs`
- `src/DotnetBackendSnippets.Core/Numbers/NumberReverseLookupSamples.cs`

## テスト

- `tests/DotnetBackendSnippets.Tests/Numbers/NumberSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/Numbers/NumberReverseLookupSamplesTests.cs`

## 使い方

- `Clamp` は値を最小値と最大値の範囲内に収めます。
- `DivideOrDefault` は分母が0のときに既定値を返します。
- `Percentage` は割合をパーセントに変換し、指定桁数で丸めます。
- `RoundCurrency` は金額向けに小数第2位まで丸めます。
- `AddTax` は税率を加算した税込金額を返します。
- `IsBetween` は値が範囲内かどうかを判定します。
- `NumberReverseLookupSamples` は、逆引き候補から使いやすい数値処理を追加でまとめます。

## メモ

- 金額には `decimal` を使っています。`double` よりも10進数の丸め誤差を避けやすいためです。
- 丸めは `MidpointRounding.AwayFromZero` を使い、0.005 のような値を直感的に切り上げます。

## 実務逆引き

### 使い方

バックエンド開発で数値処理をするときに毎回調べがちな内容を、困りごとから探せる逆引きとしてまとめます。

実装済みの項目は `NumberSamples.cs` または `NumberReverseLookupSamples.cs` のメソッド名を示します。逆引き項目は実装済みメソッド名を記載しています。

### decimal / int の基本

- 金額計算に `decimal` を使うべきか判断したい → このページのメモ
- 件数やページ番号に `int` を使うべきか決めたい → 実装済み: `RequirePositiveInt`
- DB の `decimal(18,2)` と C# の `decimal` を合わせたい → 実装済み
- `double` と `decimal` の丸め誤差の違いを説明したい → 実装済み
- 文字列から `int` を安全に変換したい → 実装済み: `ParseIntOrDefault`
- 文字列から `decimal` をカルチャ非依存で変換したい → 実装済み: `TryParseDecimalInvariant`
- nullable な数値を既定値付きで扱いたい → 実装済み: `DefaultIfNull`
- `0` と `null` を業務上どう区別するか整理したい → 実装済み
- 負数を許す項目と許さない項目を分けたい → 実装済み: `RequireNonNegative`
- 計算前に入力値を正規化したい → 実装済み: `RequirePositiveInt`, `RequireNonNegative`, `RequireFractionRate`

### 丸め

- 金額を小数第2位で丸めたい → 実装済み: `RoundCurrency`
- 0.005 を直感どおり切り上げたい → 実装済み: `RoundCurrency`
- `MidpointRounding.AwayFromZero` を使う場面を知りたい → このページのメモ
- 銀行丸めと通常の四捨五入を使い分けたい → 実装済み: `RoundBankers`, `RoundAwayFromZero`
- 小数第0位に丸めて整数表示したい → 実装済み: `RoundAwayFromZero`
- 切り上げで請求単位を作りたい → 実装済み: `CeilingToUnit`
- 切り捨てでポイントを計算したい → 実装済み: `FloorToUnit`
- 100円単位で丸めたい → 実装済み: `RoundToUnit`
- 税込後に丸めるか明細ごとに丸めるか整理したい → 実装済み
- 丸め桁数を引数で受けたい → 実装済み: `Percentage`

### 割合 / 比率

- 分子と分母からパーセントを出したい → 実装済み: `Percentage`
- 分母が0のときに0%として扱いたい → 実装済み: `Percentage`
- ゼロ除算を避けて既定値を返したい → 実装済み: `DivideOrDefault`
- 達成率を小数第1位で表示したい → 実装済み: `Percentage`
- 変化率を計算したい → 実装済み: `CalculateChangeRate`
- 前年比を計算したい → 実装済み: `CalculateChangeRate`
- 構成比を合計100%に近づけたい → 実装済み: `CalculateCompositionPercentages`
- 割引率から割引後価格を計算したい → 実装済み: `ApplyDiscountRate`
- 利益率を計算したい → 実装済み: `CalculateProfitMargin`
- 比率を `0.1234` と `12.34%` のどちらで保持するか決めたい → 実装済み: `CalculateRatio`, `FormatPercent`

### 税 / 料金

- 税抜金額から税込金額を出したい → 実装済み: `AddTax`
- 税率が負数なら例外にしたい → 実装済み: `AddTax`
- 税込金額から税抜金額を逆算したい → 実装済み: `CalculateTaxFromGross`
- 税額だけを計算したい → 実装済み: `CalculateTaxFromNet`, `CalculateTaxFromGross`
- 軽減税率など複数税率を扱いたい → 実装済み
- 明細単位の税額と合計税額の差分を調整したい → 実装済み
- 送料込みの総額を計算したい → 実装済み: `CalculateTotalWithShipping`
- 手数料率と固定手数料を合算したい → 実装済み: `CalculateFee`
- 最低手数料を適用したい → 実装済み: `CalculateFee`
- 税率を `0.1` と `10` のどちらで受けるか決めたい → 実装済み: `RequireFractionRate`

### 通貨 / 金額

- 金額表示を `1,234.56` の形にしたい → 実装済み: `FormatThousands`
- 円表示を小数なしにしたい → 実装済み: `FormatCurrencyCode`
- 通貨コード付きで表示したい → 実装済み: `FormatCurrencyCode`
- カルチャごとの通貨表示を使いたい → 実装済み
- 金額を最小通貨単位の整数で保持したい → 実装済み: `ToMinorCurrencyUnits`
- 小数通貨と整数通貨を分けて扱いたい → 実装済み: `ToMinorCurrencyUnits`, `FromMinorCurrencyUnits`
- 為替レートを掛けた後の丸めを決めたい → 実装済み: `ConvertCurrency`
- 金額がマイナスなら返金として扱いたい → 実装済み: `IsRefund`
- 金額の上限を超えたら拒否したい → 実装済み: `EnsureMaximumAmount`
- 金額入力を範囲内に収めたい → 実装済み: `Clamp`

### 範囲チェック / バリデーション

- 値を最小値と最大値の間に収めたい → 実装済み: `Clamp`
- 値が範囲内か判定したい → 実装済み: `IsBetween`
- 境界値を含む範囲判定をしたい → 実装済み: `IsBetween`
- 境界値を含まない範囲判定をしたい → 実装済み: `IsBetween`
- 最小値が最大値より大きい設定を検出したい → 実装済み: `Clamp`, `IsBetween`
- 年齢や数量など正の整数だけ許可したい → 実装済み: `RequirePositiveInt`
- 0以上100以下のスコアを検証したい → 実装済み: `IsBetween`
- API の query parameter の範囲を検証したい → 実装済み: `RequirePositiveInt`, `ClampPageSize`
- 設定値の min/max を起動時に検証したい → 実装済み: `RequirePositiveInt`, `EnsureMaximumAmount`
- 範囲外の値を例外にするか補正するか決めたい → 実装済み: `Clamp`, `IsBetween`

### ページング計算

- `pageNumber` と `pageSize` から `Skip` 件数を出したい → 実装済み: `CalculateSkip`
- `pageNumber` が1未満なら拒否したい → 実装済み: `RequirePositiveInt`
- `pageSize` の最大値を制限したい → 実装済み: `Clamp`, `ClampPageSize`
- `Skip` 計算の `int` オーバーフローを防ぎたい → 実装済み: `CalculateSkip`
- 総件数から総ページ数を計算したい → 実装済み: `CalculateTotalPages`
- 最終ページかどうか判定したい → 実装済み: `IsLastPage`
- `offset` / `limit` 形式に変換したい → 実装済み: `ToOffsetLimit`
- UI 用に表示開始件数と終了件数を出したい → 実装済み: `GetDisplayRange`
- 空ページを許すかエラーにするか決めたい → 実装済み: `GetDisplayRange`
- cursor pagination と offset pagination を使い分けたい → 実装済み

### 集計

- 合計金額を計算したい → 実装済み: `SumAmounts`
- 平均値を計算したい → 実装済み: `AverageOrDefault`
- 最小値と最大値をまとめて出したい → 実装済み: `MinMaxOrNull`
- 件数0の平均を安全に扱いたい → 実装済み: `DivideOrDefault`
- カテゴリ別に金額を合計したい → 実装済み: `SumByCategory`
- 加重平均を計算したい → 実装済み: `WeightedAverage`
- 中央値を計算したい → 実装済み: `Median`
- パーセンタイルを計算したい → 実装済み: `Percentile`
- 外れ値を除外して集計したい → 実装済み: `AverageWithoutOutliers`
- 集計結果の丸めタイミングを決めたい → 実装済み: `RoundCurrency`, `Percentage`

### オーバーフロー / 安全な計算

- `int` の加算オーバーフローを検出したい → 実装済み: `TryAddInt32`
- `checked` を使って危険な計算を落としたい → 実装済み: `CalculateSkip`, `SumAmounts`
- `decimal` の桁あふれを検出したい → 実装済み: `TryMultiplyDecimal`
- 乗算前に上限チェックしたい → 実装済み: `CanMultiplyWithoutExceeding`
- `Math.BigMul` を使う場面を知りたい → 実装済み: `BigMultiply`
- ID や件数を `long` にする基準を決めたい → 実装済み
- 大きい数値を安全に比較したい → 実装済み
- 分母が極小の比率計算を避けたい → 実装済み: `DivideOrDefault`
- 計算結果が `NaN` や `Infinity` になる型を避けたい → 実装済み: `IsFinite`
- バッチ集計で累積誤差を避けたい → 実装済み: `SumAmounts`

### 表示フォーマット

- 3桁区切りで表示したい → 実装済み: `FormatThousands`
- パーセントを `12.34%` と表示したい → 実装済み: `FormatPercent`
- 小数点以下の不要な0を消したい → 実装済み: `TrimTrailingZeros`
- 小数点以下を必ず2桁表示したい → 実装済み: `RoundCurrency`, `FormatFixedDecimal`
- 負数を括弧付きで表示したい → 実装済み: `FormatAccounting`
- 単位付きで `10 kg` のように表示したい → 実装済み: `FormatWithUnit`
- ファイルサイズを KB / MB / GB に変換したい → 実装済み: `FormatFileSize`
- 処理時間を ms / sec に変換して表示したい → 実装済み: `FormatDuration`
- API レスポンスでは数値を文字列化すべきか決めたい → 実装済み
- ログでは計算前の値と丸め後の値を両方出したい → 実装済み

### 補足

- 金額、税、割合は `decimal` を基本にします。
- DB、API、画面表示で丸めタイミングがずれると差分が出るため、境界を明確にします。
- 実装済みの小さな部品は `NumberSamples.cs` と `NumberReverseLookupSamples.cs` にあります。
