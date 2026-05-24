# テスト系パッケージの major version 採用

## 背景

このリポジトリは Target Framework を `net10.0` に固定しつつ、テスト実行基盤の一部に major version が新しいパッケージを使っています。

対象:

- `coverlet.collector`
- `Microsoft.NET.Test.Sdk`
- `xunit.runner.visualstudio`

## 方針

テスト系パッケージは、実装スニペットの公開 API や対象 Target Framework を直接変えないため、CI で `net10.0` の restore / build / test / coverage が通るものは採用します。

ただし、次の変更は移行 PR として扱います。

- `xunit` 本体を v3 系へ上げる変更
- テスト属性、fixture、assertion の書き換えが必要な変更
- coverage artifact の形式や CI 実行方法が変わる変更
- `.NET SDK` または Target Framework の更新が必要になる変更

## 理由

- テスト基盤はセキュリティ更新や runner 対応を早めに取り込む価値があるため。
- 実装プロジェクトは `net10.0` に統一し、利用者向けのスニペット再現性を優先するため。
- major version の採用理由を残し、Dependabot PR のレビュー観点を明確にするため。

## 運用

- Dependabot は test dependencies を別グループにし、minor / patch 更新を中心に扱います。
- major 更新が来た場合は、CI 結果だけでなく、この decision の条件に照らして移行 PR として確認します。
- `Directory.Packages.props` の test 系 package を更新したら、必要に応じてこの decision も見直します。
