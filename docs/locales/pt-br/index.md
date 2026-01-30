# Referência da API DDAP

Bem-vindo à documentação de referência da API DDAP. Esta seção fornece documentação detalhada da API gerada a partir de comentários XML no código-fonte.

## Namespaces

Navegue pela documentação da API por namespace:

- **Ddap.Core** - Abstrações e interfaces principais
- **Ddap.Data.Dapper** - Provedor Dapper genérico para qualquer banco de dados
- **Ddap.Data.EntityFramework** - Provedor Entity Framework Core
- **Ddap.Rest** - Provedor de API REST
- **Ddap.GraphQL** - Provedor GraphQL
- **Ddap.Grpc** - Provedor gRPC
- **Ddap.Memory** - Gerenciamento de entidades em memória
- **Ddap.CodeGen** - Geradores de código-fonte
- **Ddap.Aspire** - Integração com .NET Aspire
- **Ddap.Auth** - Autenticação e autorização
- **Ddap.Subscriptions** - Assinaturas em tempo real

## Interfaces Principais

### Interfaces Principais

- `IEntityConfiguration` - Representa metadados de entidade
- `IPropertyConfiguration` - Representa metadados de propriedade/coluna
- `IIndexConfiguration` - Representa metadados de índice
- `IRelationshipConfiguration` - Representa relacionamentos de chave estrangeira
- `IEntityRepository` - Registro de entidades
- `IDataProvider` - Abstração de provedor de banco de dados
- `IDdapBuilder` - API de configuração fluente

## Começando

Para entender como usar essas APIs, confira nossos guias:

- [Começando](get-started.md)
- [Visão Geral da Arquitetura](architecture.md)
- [Uso Avançado](advanced.md)

## Contribuindo

Encontrou um problema na documentação da API? Por favor, [abra um issue](https://github.com/schivei/ddap/issues) no GitHub.
