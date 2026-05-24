# Serialization の基本

## 目的

`System.Text.Json` で Web API 向けの JSON 変換を安全に扱うためのスニペットです。camelCase、文字列 enum、null 省略、`DateTimeOffset`、snake_case、未知フィールド、polymorphism、versioning、source generation を確認します。

## 実装

`src/DotnetBackendSnippets.Core/Serialization/SerializationSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Serialization/SerializationSamplesTests.cs`

## 使い方

- `CreateWebApiJsonOptions` は `JsonSerializerDefaults.Web` を基準に、camelCase、case-insensitive 読み取り、文字列 enum、null 省略を設定します。
- `SerializeOrder` は API レスポンス向けに注文 DTO を JSON 化します。
- `DeserializeOrder` は文字列 enum と `DateTimeOffset` を含む JSON を DTO に戻します。
- `DeserializeFlexibleOrderRequest` は `[JsonExtensionData]` で未知フィールドを保持します。
- `CreateSnakeCaseJsonOptions` は外部 API 連携向けに snake_case のプロパティ名を使います。
- `SerializePaymentMethod` / `DeserializePaymentMethod` は type discriminator 付き polymorphism の例です。
- `SerializeVersionedEnvelope` は API 契約バージョンを JSON に含めます。
- `SerializeOrderWithSourceGeneration` は `JsonSerializerContext` を使った source generation の例です。

## メモ

- enum を数値で出すとクライアントとの互換性が崩れやすいため、公開 API では文字列 enum を検討します。
- 日付時刻はタイムゾーン情報を落としにくい `DateTimeOffset` を優先すると事故を減らせます。
- null を省略するか明示するかは API 契約です。既存クライアントがいる場合は変更前に互換性を確認してください。
- 未知フィールドを無視するだけでなく保持すると、移行期のログや後方互換性確認に使えます。

## 実務逆引き

- Web API の JSON オプションを揃えたい → `CreateWebApiJsonOptions`
- enum を文字列として返したい → `JsonStringEnumConverter<OrderStatus>`
- null のプロパティを省略したい → `DefaultIgnoreCondition`
- unknown field を保持したい → `FlexibleOrderRequest.ExtensionData`
- snake_case の外部 API に合わせたい → `CreateSnakeCaseJsonOptions`
- polymorphism を使いたい → `PaymentMethodDto`
- API 互換性のために version を入れたい → `SerializeVersionedEnvelope`
- source generation を使いたい → `SnippetJsonSerializerContext`
