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

## 実務逆引き

業務バックエンドで毎回調べがちな日付・時刻処理を、困りごとから探せる形でまとめます。

既存実装は [date-and-time-samples.md](./date-and-time-samples.md) と `src/DotnetBackendSnippets.Core/DateAndTime/DateAndTimeSamples.cs` を参照してください。未実装のものは「追加候補」として示します。

### DateOnly / TimeOnly / DateTimeOffset

- 日付だけを扱いたい → `DateOnly` を使う。実装済み: `StartOfMonth`, `EndOfMonth`, `IsWeekend`, `CalculateAge`, `DaysBetween`
- 時刻だけを扱いたい → 追加候補: `TimeOnly` で営業時間や締切時刻を表す
- タイムゾーン付きの瞬間を扱いたい → 追加候補: `DateTimeOffset` をAPIやログの時刻に使う
- `DateTime` から日付だけを取り出したい → 追加候補: `DateOnly.FromDateTime`
- `DateOnly` と `TimeOnly` から日時を作りたい → 追加候補: `DateOnly.ToDateTime(TimeOnly)`
- `DateTimeKind` の違いを整理したい → 追加候補: `Utc`, `Local`, `Unspecified` の変換ルール
- DB保存用に日付だけを保持したい → 追加候補: `DateOnly` の保存形式と変換
- API DTOで期限日だけを受けたい → 追加候補: `DateOnly? DueDate` の入力検証
- 日時のオフセットを維持して受けたい → 追加候補: `DateTimeOffset` 入力の正規化
- 現在時刻の取得をテスト可能にしたい → 追加候補: `TimeProvider` を注入する

### 日単位の開始・終了

- 日の開始時刻を作りたい → 実装済み: `StartOfDay`
- 日の終了時刻を作りたい → 実装済み: `EndOfDay`
- 今日の開始時刻をUTCで作りたい → 追加候補: `TimeProvider.GetUtcNow()` から日境界を作る
- ローカル日の開始をUTCに変換したい → 追加候補: `TimeZoneInfo` で日付境界を変換
- 日付範囲検索で終了日の23:59:59を避けたい → 追加候補: 半開区間 `[start, nextDay)` を使う
- 日付だけで同日判定したい → 追加候補: `DateOnly.FromDateTime(value)` で比較
- 1日の期間を作りたい → 追加候補: `startInclusive` と `endExclusive`
- 日付をまたぐ処理を検出したい → 追加候補: `start.Date != end.Date`
- 日次バッチの対象日を決めたい → 追加候補: 実行日ではなく処理対象日を引数化
- 日次集計のキーを作りたい → 追加候補: `yyyy-MM-dd` または `DateOnly`

### 月・四半期・年度

- 月初を求めたい → 実装済み: `StartOfMonth`
- 月末を求めたい → 実装済み: `EndOfMonth`
- 翌月初を求めたい → 追加候補: `StartOfMonth(value).AddMonths(1)`
- 前月末を求めたい → 追加候補: `StartOfMonth(value).AddDays(-1)`
- 月の日数を求めたい → 追加候補: `DateTime.DaysInMonth`
- 月次集計の範囲を作りたい → 追加候補: `[monthStart, nextMonthStart)`
- 四半期の開始月を求めたい → 追加候補: `((month - 1) / 3) * 3 + 1`
- 四半期の期間を作りたい → 追加候補: 四半期開始と3か月後の半開区間
- 会計年度の開始日を求めたい → 追加候補: 開始月を設定値にする
- 月末締めの期限日を作りたい → 追加候補: 月末が休日なら前営業日に寄せる

### 週・営業日・休日

- 週末か判定したい → 実装済み: `IsWeekend`
- 平日か判定したい → 追加候補: `!IsWeekend(value)`
- 次の営業日を求めたい → 追加候補: 週末と祝日を飛ばす
- 前の営業日を求めたい → 追加候補: 週末と祝日を戻る方向で飛ばす
- 営業日数を数えたい → 追加候補: 期間を走査して営業日だけ数える
- N営業日後を求めたい → 追加候補: 加算しながら営業日のみカウント
- 祝日カレンダーを注入したい → 追加候補: `IHolidayCalendar`
- 週の開始日を求めたい → 追加候補: 月曜始まり/日曜始まりを選べる関数
- 週番号を扱いたい → 追加候補: `ISOWeek` を使う
- 休日なら翌営業日にずらしたい → 追加候補: 支払日・通知日の調整

### 年齢・期限・期間

- 年齢を計算したい → 実装済み: `CalculateAge`
- 2月29日生まれの年齢を扱いたい → 実装済み: `CalculateAge`
- 2日付の日数差を求めたい → 実装済み: `DaysBetween`
- 期限切れか判定したい → 追加候補: `now >= expiresAt`
- 期限までの残り時間を出したい → 追加候補: `expiresAt - now`
- 猶予期間を含めたい → 追加候補: `dueAt.Add(gracePeriod)`
- 有効期間内か判定したい → 追加候補: `[validFrom, validUntil)`
- 経過月数を求めたい → 追加候補: 年月差から日付補正を入れる
- 契約年数を求めたい → 追加候補: 記念日前なら1年引く
- SLA期限を計算したい → 追加候補: 営業時間と営業日を考慮する

### タイムゾーン・UTC

- 保存時刻をUTCに統一したい → 追加候補: DB保存前にUTCへ正規化
- 表示時だけユーザーのタイムゾーンにしたい → 追加候補: `TimeZoneInfo.ConvertTime`
- IANAとWindowsタイムゾーンIDの違いを扱いたい → 追加候補: OS差異を吸収する変換
- JSTの現在時刻を作りたい → 追加候補: `Asia/Tokyo` または `Tokyo Standard Time`
- UTCからローカル日付を求めたい → 追加候補: ユーザーのタイムゾーンへ変換後 `DateOnly`
- ローカル日付範囲をUTC範囲にしたい → 追加候補: 日付境界をタイムゾーン変換
- 夏時間の存在しない時刻を検出したい → 追加候補: `IsInvalidTime`
- 夏時間の重複時刻を検出したい → 追加候補: `IsAmbiguousTime`
- サーバーローカル時刻に依存したくない → 追加候補: `DateTime.Now` を避ける
- APIでUTC ISO 8601を返したい → 追加候補: `DateTimeOffset` の `O` 書式

### 範囲検索・DB保存

- 日付範囲でDB検索したい → 追加候補: `>= start` かつ `< endExclusive`
- 終了日を含めた検索をしたい → 追加候補: 終了日の翌日を `endExclusive` にする
- 月次データを検索したい → 追加候補: 月初から翌月初の半開区間
- インデックスが効く日時条件にしたい → 追加候補: カラム側に関数をかけない
- DBにUTCで保存したい → 追加候補: 保存ルールを1つに固定
- DBに日付のみ保存したい → 追加候補: `DateOnly` または `date` 型
- 楽観ロックに更新日時を使いたい → 追加候補: `UpdatedAt` と concurrency token
- 作成日時・更新日時を自動設定したい → 追加候補: SaveChanges interceptor
- 削除日時で論理削除したい → 追加候補: `DeletedAt` と query filter
- 監査ログに発生時刻を残したい → 追加候補: `OccurredAtUtc`

### API・入力検証

- 日付文字列を安全にパースしたい → 追加候補: `DateOnly.TryParseExact`
- 時刻文字列を安全にパースしたい → 追加候補: `TimeOnly.TryParseExact`
- 受け付ける日付書式を固定したい → 追加候補: `yyyy-MM-dd`
- 未来日だけ許可したい → 追加候補: `value >= today`
- 過去日だけ許可したい → 追加候補: `value <= today`
- 開始日が終了日以下か検証したい → 追加候補: `start <= end`
- 最大検索期間を制限したい → 追加候補: `DaysBetween` 相当で上限チェック
- nullableな期限日を扱いたい → 追加候補: 未指定なら期限なし
- タイムゾーンIDを入力検証したい → 追加候補: `FindSystemTimeZoneById` をtryする
- APIのエラーに日付制約を出したい → 追加候補: ProblemDetails の validation errors

### ログ・ジョブ・キャッシュ

- ログ時刻をUTCで出したい → 追加候補: ログ基盤側のUTC設定を確認
- correlation id と時刻を一緒に残したい → 追加候補: scope に `OccurredAtUtc`
- バッチの実行間隔を表したい → 追加候補: `TimeSpan`
- 定期実行の次回時刻を計算したい → 追加候補: cronや固定間隔を明示
- タイムアウトを設定したい → 追加候補: `CancellationTokenSource.CancelAfter`
- キャッシュのTTLを設定したい → 追加候補: absolute expiration と sliding expiration
- リトライ間隔を計算したい → 追加候補: exponential backoff
- 処理時間を測りたい → 追加候補: `Stopwatch`
- 遅い処理だけログに出したい → 追加候補: 閾値を設定値化
- ジョブの対象期間をログに出したい → 追加候補: `targetFrom` / `targetTo`

### テスト・設計

- 現在時刻を固定してテストしたい → 追加候補: `TimeProvider` または時計インターフェース
- 日付境界のテストを増やしたい → 追加候補: 月末、年末、うるう日
- タイムゾーン変換をテストしたい → 追加候補: UTCとJSTで期待値を分ける
- 夏時間のテストをしたい → 追加候補: DSTがある地域の境界を使う
- 期限切れ判定を安定させたい → 追加候補: `now` を引数で渡す
- 日付計算の仕様を集約したい → 追加候補: `BusinessDateCalculator`
- 休日カレンダーを差し替えたい → 追加候補: テスト用 `IHolidayCalendar`
- ログ用時刻と業務日付を分けたい → 追加候補: `OccurredAtUtc` と `BusinessDate`
- 分散システムの時刻ずれを考慮したい → 追加候補: 許容誤差を設定する
- 日付処理の方針をドキュメント化したい → 追加候補: UTC保存、表示時変換、半開区間を明記する
