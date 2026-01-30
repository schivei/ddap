# DDAP API-Referenz

Willkommen zur DDAP API-Referenzdokumentation. Dieser Abschnitt bietet detaillierte API-Dokumentation, die aus XML-Kommentaren im Quellcode generiert wurde.

## Namespaces

Durchsuchen Sie die API-Dokumentation nach Namespace:

- **Ddap.Core** - Kern-Abstraktionen und Schnittstellen
- **Ddap.Data.Dapper** - Generischer Dapper-Provider für jede Datenbank
- **Ddap.Data.EntityFramework** - Entity Framework Core Provider
- **Ddap.Rest** - REST-API-Provider
- **Ddap.GraphQL** - GraphQL-Provider
- **Ddap.Grpc** - gRPC-Provider
- **Ddap.CodeGen** - Quellcode-Generatoren
- **Ddap.Aspire** - .NET Aspire-Integration
- **Ddap.Auth** - Authentifizierung und Autorisierung
- **Ddap.Subscriptions** - Echtzeit-Abonnements
- **Ddap.Client.Core** - Client-Kern-Abstraktionen
- **Ddap.Client.Rest** - REST-Client
- **Ddap.Client.GraphQL** - GraphQL-Client
- **Ddap.Client.Grpc** - gRPC-Client

## Hauptschnittstellen

### Hauptschnittstellen

- `IEntityConfiguration` - Repräsentiert Entity-Metadaten
- `IPropertyConfiguration` - Repräsentiert Eigenschafts-/Spalten-Metadaten
- `IIndexConfiguration` - Repräsentiert Index-Metadaten
- `IRelationshipConfiguration` - Repräsentiert Fremdschlüssel-Beziehungen
- `IEntityRepository` - Entity-Register
- `IDataProvider` - Datenbank-Provider-Abstraktion
- `IDdapBuilder` - Fließende Konfigurations-API

## Erste Schritte

Um zu verstehen, wie diese APIs verwendet werden, lesen Sie unsere Anleitungen:

- [Erste Schritte](get-started.md)
- [Architektur-Übersicht](architecture.md)
- [Erweiterte Verwendung](advanced.md)

## Mitwirken

Haben Sie ein Problem in der API-Dokumentation gefunden? Bitte [öffnen Sie ein Issue](https://github.com/schivei/ddap/issues) auf GitHub.
