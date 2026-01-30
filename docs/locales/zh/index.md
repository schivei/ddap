# DDAP API参考

欢迎使用DDAP API参考文档。本节提供从源代码中的XML注释生成的详细API文档。

## 命名空间

按命名空间浏览API文档：

- **Ddap.Core** - 核心抽象和接口
- **Ddap.Data.Dapper** - 适用于任何数据库的通用Dapper提供程序
- **Ddap.Data.EntityFramework** - Entity Framework Core提供程序
- **Ddap.Rest** - REST API提供程序
- **Ddap.GraphQL** - GraphQL提供程序
- **Ddap.Grpc** - gRPC提供程序
- **Ddap.CodeGen** - 源代码生成器
- **Ddap.Aspire** - .NET Aspire集成
- **Ddap.Auth** - 身份验证和授权
- **Ddap.Subscriptions** - 实时订阅
- **Ddap.Client.Core** - 客户端核心抽象
- **Ddap.Client.Rest** - REST客户端
- **Ddap.Client.GraphQL** - GraphQL客户端
- **Ddap.Client.Grpc** - gRPC客户端

## 主要接口

### 主要接口

- `IEntityConfiguration` - 表示实体元数据
- `IPropertyConfiguration` - 表示属性/列元数据
- `IIndexConfiguration` - 表示索引元数据
- `IRelationshipConfiguration` - 表示外键关系
- `IEntityRepository` - 实体注册表
- `IDataProvider` - 数据库提供程序抽象
- `IDdapBuilder` - 流畅的配置API

## 入门

要了解如何使用这些API，请查看我们的指南：

- [入门](get-started.md)
- [架构概述](architecture.md)
- [高级用法](advanced.md)

## 贡献

在API文档中发现问题？请在GitHub上[提交issue](https://github.com/schivei/ddap/issues)。
