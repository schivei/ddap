# DDAP API Reference

Welcome to the DDAP API Reference documentation. This section provides detailed API documentation generated from XML comments in the source code.

## Namespaces

Browse the API documentation by namespace:

- **Ddap.Core** - Core abstractions and interfaces
- **Ddap.Data.Dapper** - Generic Dapper provider for any database
- **Ddap.Data.EntityFramework** - Entity Framework Core provider
- **Ddap.Rest** - REST API provider
- **Ddap.GraphQL** - GraphQL provider
- **Ddap.Grpc** - gRPC provider
- **Ddap.CodeGen** - Source generators
- **Ddap.Aspire** - .NET Aspire integration
- **Ddap.Auth** - Authentication and authorization
- **Ddap.Subscriptions** - Real-time subscriptions
- **Ddap.Client.Core** - Core client abstractions
- **Ddap.Client.Rest** - REST client
- **Ddap.Client.GraphQL** - GraphQL client
- **Ddap.Client.Grpc** - gRPC client

## Key Interfaces

### Core Interfaces

- `IEntityConfiguration` - Represents entity metadata
- `IPropertyConfiguration` - Represents property/column metadata
- `IIndexConfiguration` - Represents index metadata
- `IRelationshipConfiguration` - Represents foreign key relationships
- `IEntityRepository` - Entity registry
- `IDataProvider` - Database provider abstraction
- `IDdapBuilder` - Fluent configuration API

## Getting Started

To understand how to use these APIs, check out our guides:

- [Getting Started](get-started.md)
- [Architecture Overview](architecture.md)
- [Advanced Usage](advanced.md)

## Contributing

Found an issue in the API documentation? Please [open an issue](https://github.com/schivei/ddap/issues) on GitHub.
