# DDAP APIリファレンス

DDAP APIリファレンスドキュメントへようこそ。このセクションでは、ソースコードのXMLコメントから生成された詳細なAPIドキュメントを提供します。

## 名前空間

名前空間別にAPIドキュメントを参照：

- **Ddap.Core** - コア抽象化とインターフェース
- **Ddap.Data.Dapper** - あらゆるデータベース用の汎用Dapperプロバイダー
- **Ddap.Data.EntityFramework** - Entity Framework Coreプロバイダー
- **Ddap.Rest** - REST APIプロバイダー
- **Ddap.GraphQL** - GraphQLプロバイダー
- **Ddap.Grpc** - gRPCプロバイダー
- **Ddap.CodeGen** - ソースコードジェネレーター
- **Ddap.Aspire** - .NET Aspire統合
- **Ddap.Auth** - 認証と認可
- **Ddap.Subscriptions** - リアルタイムサブスクリプション
- **Ddap.Client.Core** - クライアントコア抽象化
- **Ddap.Client.Rest** - RESTクライアント
- **Ddap.Client.GraphQL** - GraphQLクライアント
- **Ddap.Client.Grpc** - gRPCクライアント

## 主要インターフェース

### 主要インターフェース

- `IEntityConfiguration` - エンティティメタデータを表す
- `IPropertyConfiguration` - プロパティ/カラムメタデータを表す
- `IIndexConfiguration` - インデックスメタデータを表す
- `IRelationshipConfiguration` - 外部キー関係を表す
- `IEntityRepository` - エンティティレジストリ
- `IDataProvider` - データベースプロバイダー抽象化
- `IDdapBuilder` - 流暢な構成API

## はじめに

これらのAPIの使用方法を理解するには、ガイドをご確認ください：

- [はじめに](get-started.md)
- [アーキテクチャ概要](architecture.md)
- [高度な使用法](advanced.md)

## 貢献

APIドキュメントに問題を見つけましたか？GitHubで[イシューを開いてください](https://github.com/schivei/ddap/issues)。
