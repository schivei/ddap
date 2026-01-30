# Référence de l'API DDAP

Bienvenue dans la documentation de référence de l'API DDAP. Cette section fournit une documentation détaillée de l'API générée à partir des commentaires XML dans le code source.

## Espaces de noms

Parcourir la documentation de l'API par espace de noms :

- **Ddap.Core** - Abstractions et interfaces principales
- **Ddap.Data.Dapper** - Fournisseur Dapper générique pour toute base de données
- **Ddap.Data.EntityFramework** - Fournisseur Entity Framework Core
- **Ddap.Rest** - Fournisseur d'API REST
- **Ddap.GraphQL** - Fournisseur GraphQL
- **Ddap.Grpc** - Fournisseur gRPC
- **Ddap.Memory** - Gestion des entités en mémoire
- **Ddap.CodeGen** - Générateurs de code source
- **Ddap.Aspire** - Intégration avec .NET Aspire
- **Ddap.Auth** - Authentification et autorisation
- **Ddap.Subscriptions** - Abonnements en temps réel

## Interfaces Principales

### Interfaces Principales

- `IEntityConfiguration` - Représente les métadonnées d'entité
- `IPropertyConfiguration` - Représente les métadonnées de propriété/colonne
- `IIndexConfiguration` - Représente les métadonnées d'index
- `IRelationshipConfiguration` - Représente les relations de clé étrangère
- `IEntityRepository` - Registre des entités
- `IDataProvider` - Abstraction du fournisseur de base de données
- `IDdapBuilder` - API de configuration fluide

## Démarrage

Pour comprendre comment utiliser ces APIs, consultez nos guides :

- [Démarrage](get-started.md)
- [Aperçu de l'Architecture](architecture.md)
- [Utilisation Avancée](advanced.md)

## Contribuer

Vous avez trouvé un problème dans la documentation de l'API ? Veuillez [ouvrir un issue](https://github.com/schivei/ddap/issues) sur GitHub.
