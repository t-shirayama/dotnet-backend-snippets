# DTO Mapping の基本

## 目的

API DTO と domain object / EF Core entity の境界を明確にし、過剰公開や over-posting を避けるための手書き mapping スニペットです。

## 実装

`src/DotnetBackendSnippets.Core/DtoMapping/DtoMappingSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/DtoMapping/DtoMappingSamplesTests.cs`

## 使い方

- `ToCommand` は request DTO を domain command に変換します。`IsAdmin` のような過剰投稿値は domain へ渡しません。
- `ToResponse` は entity を API response DTO に変換し、返す項目を明示します。
- `ApplyProfilePatch` は部分更新 DTO の non-null 値だけを entity に反映します。

## メモ

- EF entity を API response として直接返すと、内部状態や navigation property を過剰公開しやすくなります。
- request DTO を entity として直接保存すると、クライアントが更新してはいけない値まで書き換える over-posting の危険があります。
- 小さい mapping は手書きにすると、境界と意図がレビューしやすくなります。
- PATCH では「指定なし」と「null でクリア」を区別したい場合、専用の optional 型や JSON Patch の採用を検討します。

## 実務逆引き

- request DTO を command に変換したい → `ToCommand`
- entity を response DTO に変換したい → `ToResponse`
- nested DTO を mapping したい → `ToResponse`
- partial update を手書きしたい → `ApplyProfilePatch`
- over-posting を防ぎたい → `ToCommand` / `ApplyProfilePatch`
