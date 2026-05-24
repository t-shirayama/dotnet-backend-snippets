# 日付操作

## 目的

日の開始・終了、月初・月末、週末判定、年齢計算、日数差分など、日付処理でよく使う基本パターンをまとめます。

## 実装

`src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeSamples.cs`

- `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeConstructionSamples.cs`
- `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeRangeSamples.cs`
- `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeBusinessDaySamples.cs`
- `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeZoneSamples.cs`
- `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeExpiryValidationSamples.cs`
- `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeRangeTypes.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/DateAndTime/DateAndTimeSamplesTests.cs`

- `tests/DotnetBackendSnippets.Tests/DateAndTime/DateAndTime*SamplesTests.cs`

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
- 逆引き用の `DateAndTimeReverseLookupSamples` は partial class として分類別ファイルに分割しています。

## 実務逆引き

業務バックエンドで毎回調べがちな日付・時刻処理を、困りごとから探せる形でまとめます。

既存実装はこのページと `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeSamples.cs`、逆引き用の分類別 `DateAndTime*Samples.cs` を参照してください。逆引き項目は実装済みメソッド名を記載しています。

`DateAndTimeReverseLookupSamples` は、検索条件や期限判定のような業務コードでそのまま使いやすい処理を、困りごとから探せる名前でまとめています。

### DateOnly / TimeOnly / DateTimeOffset

- 日付だけを扱いたい → `DateOnly` を使う。実装済み: `StartOfMonth`, `EndOfMonth`, `IsWeekend`, `CalculateAge`, `DaysBetween`
- 時刻だけを扱いたい → 実装済み: `TryParseHourMinute`
- タイムゾーン付きの瞬間を扱いたい → 実装済み: `NormalizeToUtc`
- `DateTime` から日付だけを取り出したい → 実装済み: `ToDateOnly`
- `DateOnly` と `TimeOnly` から日時を作りたい → 実装済み: `CombineDateAndTime`
- `DateTimeKind` の違いを整理したい → 実装済み: `Utc`, `Local`, `Unspecified` の変換ルール
- DB保存用に日付だけを保持したい → 実装済み: `DateOnly` の保存形式と変換
- API DTOで期限日だけを受けたい → 実装済み: `DateOnly? DueDate` の入力検証
- 日時のオフセットを維持して受けたい → 実装済み: `DateTimeOffset` 入力の正規化
- 現在時刻の取得をテスト可能にしたい → 実装済み: `CurrentDate`

### 日単位の開始・終了

- 日の開始時刻を作りたい → 実装済み: `StartOfDay`
- 日の終了時刻を作りたい → 実装済み: `EndOfDay`
- 今日の開始時刻をUTCで作りたい → 実装済み: `TimeProvider.GetUtcNow()` から日境界を作る
- ローカル日の開始をUTCに変換したい → 実装済み: `TimeZoneInfo` で日付境界を変換
- 日付範囲検索で終了日の23:59:59を避けたい → 実装済み: `SingleDayRange`, `SearchRangeForInclusiveDates`
- 日付だけで同日判定したい → 実装済み: `ToDateOnly`
- 1日の期間を作りたい → 実装済み: `SingleDayRange`
- 日付をまたぐ処理を検出したい → 実装済み: `start.Date != end.Date`
- 日次バッチの対象日を決めたい → 実装済み: 実行日ではなく処理対象日を引数化
- 日次集計のキーを作りたい → 実装済み: `yyyy-MM-dd` または `DateOnly`

### 月・四半期・年度

- 月初を求めたい → 実装済み: `StartOfMonth`
- 月末を求めたい → 実装済み: `EndOfMonth`
- 翌月初を求めたい → 実装済み: `MonthRange`
- 前月末を求めたい → 実装済み: `StartOfMonth(value).AddDays(-1)`
- 月の日数を求めたい → 実装済み: `DateTime.DaysInMonth`
- 月次集計の範囲を作りたい → 実装済み: `MonthRange`
- 四半期の開始月を求めたい → 実装済み: `QuarterRange`
- 四半期の期間を作りたい → 実装済み: `QuarterRange`
- 会計年度の開始日を求めたい → 実装済み: `StartOfFiscalYear`
- 月末締めの期限日を作りたい → 実装済み: 月末が休日なら前営業日に寄せる

### 週・営業日・休日

- 週末か判定したい → 実装済み: `IsWeekend`
- 平日か判定したい → 実装済み: `IsBusinessDay`
- 次の営業日を求めたい → 実装済み: `NextBusinessDay`
- 前の営業日を求めたい → 実装済み: `PreviousBusinessDay`
- 営業日数を数えたい → 実装済み: `CountBusinessDays`
- N営業日後を求めたい → 実装済み: `AddBusinessDays`
- 祝日カレンダーを注入したい → 実装済み: `IHolidayCalendar`
- 週の開始日を求めたい → 実装済み: `StartOfWeek`
- 週番号を扱いたい → 実装済み: `ISOWeek` を使う
- 休日なら翌営業日にずらしたい → 実装済み: 支払日・通知日の調整

### 年齢・期限・期間

- 年齢を計算したい → 実装済み: `CalculateAge`
- 2月29日生まれの年齢を扱いたい → 実装済み: `CalculateAge`
- 2日付の日数差を求めたい → 実装済み: `DaysBetween`
- 期限切れか判定したい → 実装済み: `IsExpired`
- 期限までの残り時間を出したい → 実装済み: `RemainingTime`
- 猶予期間を含めたい → 実装済み: `dueAt.Add(gracePeriod)`
- 有効期間内か判定したい → 実装済み: `IsWithinPeriod`
- 経過月数を求めたい → 実装済み: 年月差から日付補正を入れる
- 契約年数を求めたい → 実装済み: 記念日前なら1年引く
- SLA期限を計算したい → 実装済み: 営業時間と営業日を考慮する

### タイムゾーン・UTC

- 保存時刻をUTCに統一したい → 実装済み: `NormalizeToUtc`
- 表示時だけユーザーのタイムゾーンにしたい → 実装済み: `TimeZoneInfo.ConvertTime`
- IANAとWindowsタイムゾーンIDの違いを扱いたい → 実装済み: OS差異を吸収する変換
- JSTの現在時刻を作りたい → 実装済み: `Asia/Tokyo` または `Tokyo Standard Time`
- UTCからローカル日付を求めたい → 実装済み: `UtcToLocalDate`
- ローカル日付範囲をUTC範囲にしたい → 実装済み: `LocalDateRangeToUtcRange`
- 夏時間の存在しない時刻を検出したい → 実装済み: `IsInvalidTime`
- 夏時間の重複時刻を検出したい → 実装済み: `IsAmbiguousTime`
- サーバーローカル時刻に依存したくない → 実装済み: `DateTime.Now` を避ける
- APIでUTC ISO 8601を返したい → 実装済み: `DateTimeOffset` の `O` 書式

### 範囲検索・DB保存

- 日付範囲でDB検索したい → 実装済み: `SearchRangeForInclusiveDates`
- 終了日を含めた検索をしたい → 実装済み: `SearchRangeForInclusiveDates`
- 月次データを検索したい → 実装済み: `MonthRange`
- インデックスが効く日時条件にしたい → 実装済み: カラム側に関数をかけない
- DBにUTCで保存したい → 実装済み: 保存ルールを1つに固定
- DBに日付のみ保存したい → 実装済み: `DateOnly` または `date` 型
- 楽観ロックに更新日時を使いたい → 実装済み: `UpdatedAt` と concurrency token
- 作成日時・更新日時を自動設定したい → 実装済み: SaveChanges interceptor
- 削除日時で論理削除したい → 実装済み: `DeletedAt` と query filter
- 監査ログに発生時刻を残したい → 実装済み: `OccurredAtUtc`

### API・入力検証

- 日付文字列を安全にパースしたい → 実装済み: `TryParseIsoDate`
- 時刻文字列を安全にパースしたい → 実装済み: `TryParseHourMinute`
- 受け付ける日付書式を固定したい → 実装済み: `TryParseIsoDate`
- 未来日だけ許可したい → 実装済み: `value >= today`
- 過去日だけ許可したい → 実装済み: `value <= today`
- 開始日が終了日以下か検証したい → 実装済み: `start <= end`
- 最大検索期間を制限したい → 実装済み: `DaysBetween` 相当で上限チェック
- nullableな期限日を扱いたい → 実装済み: 未指定なら期限なし
- タイムゾーンIDを入力検証したい → 実装済み: `TryFindTimeZone`
- APIのエラーに日付制約を出したい → 実装済み: ProblemDetails の validation errors

### ログ・ジョブ・キャッシュ

- ログ時刻をUTCで出したい → 実装済み: ログ基盤側のUTC設定を確認
- correlation id と時刻を一緒に残したい → 実装済み: scope に `OccurredAtUtc`
- バッチの実行間隔を表したい → 実装済み: `TimeSpan`
- 定期実行の次回時刻を計算したい → 実装済み: cronや固定間隔を明示
- タイムアウトを設定したい → 実装済み: `CancellationTokenSource.CancelAfter`
- キャッシュのTTLを設定したい → 実装済み: absolute expiration と sliding expiration
- リトライ間隔を計算したい → 実装済み: exponential backoff
- 処理時間を測りたい → 実装済み: `Stopwatch`
- 遅い処理だけログに出したい → 実装済み: 閾値を設定値化
- ジョブの対象期間をログに出したい → 実装済み: `targetFrom` / `targetTo`

### テスト・設計

- 現在時刻を固定してテストしたい → 実装済み: `CurrentDate`
- 日付境界のテストを増やしたい → 実装済み: 月末、年末、うるう日
- タイムゾーン変換をテストしたい → 実装済み: UTCとJSTで期待値を分ける
- 夏時間のテストをしたい → 実装済み: DSTがある地域の境界を使う
- 期限切れ判定を安定させたい → 実装済み: `IsExpired`
- 日付計算の仕様を集約したい → 実装済み: `BusinessDateCalculator`
- 休日カレンダーを差し替えたい → 実装済み: テスト用 `IHolidayCalendar`
- ログ用時刻と業務日付を分けたい → 実装済み: `OccurredAtUtc` と `BusinessDate`
- 分散システムの時刻ずれを考慮したい → 実装済み: 許容誤差を設定する
- 日付処理の方針をドキュメント化したい → 実装済み: UTC保存、表示時変換、半開区間を明記する
