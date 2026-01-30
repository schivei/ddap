# Referencia de la API DDAP

Bienvenido a la documentación de referencia de la API DDAP. Esta sección proporciona documentación detallada de la API generada a partir de comentarios XML en el código fuente.

## Espacios de nombres

Navegue por la documentación de la API por espacio de nombres:

- **Ddap.Core** - Abstracciones e interfaces principales
- **Ddap.Data.Dapper** - Proveedor Dapper genérico para cualquier base de datos
- **Ddap.Data.EntityFramework** - Proveedor Entity Framework Core
- **Ddap.Rest** - Proveedor de API REST
- **Ddap.GraphQL** - Proveedor GraphQL
- **Ddap.Grpc** - Proveedor gRPC
- **Ddap.Memory** - Gestión de entidades en memoria
- **Ddap.CodeGen** - Generadores de código fuente
- **Ddap.Aspire** - Integración con .NET Aspire
- **Ddap.Auth** - Autenticación y autorización
- **Ddap.Subscriptions** - Suscripciones en tiempo real

## Interfaces Principales

### Interfaces Principales

- `IEntityConfiguration` - Representa metadatos de entidad
- `IPropertyConfiguration` - Representa metadatos de propiedad/columna
- `IIndexConfiguration` - Representa metadatos de índice
- `IRelationshipConfiguration` - Representa relaciones de clave externa
- `IEntityRepository` - Registro de entidades
- `IDataProvider` - Abstracción de proveedor de base de datos
- `IDdapBuilder` - API de configuración fluida

## Empezando

Para entender cómo usar estas APIs, consulte nuestras guías:

- [Empezando](get-started.md)
- [Descripción General de la Arquitectura](architecture.md)
- [Uso Avanzado](advanced.md)

## Contribuyendo

¿Encontró un problema en la documentación de la API? Por favor, [abra un issue](https://github.com/schivei/ddap/issues) en GitHub.
