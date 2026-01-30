# Quick Reference - Unlist Deprecated NuGet Packages

## Setup (Uma única vez)
```bash
export NUGET_API_KEY=sua-chave-api-aqui
# Adicione ao ~/.bashrc para tornar permanente
```

## Comandos Rápidos

### Teste (Dry Run)
```bash
./unlist-deprecated-packages.sh --dry-run
```

### Executar (Real)
```bash
./unlist-deprecated-packages.sh
```

## Adicionar Novo Pacote Deprecado

Edite `unlist-deprecated-packages.sh` linha ~74:

```bash
DEPRECATED_PACKAGES=(
    "Ddap.Memory"
    "Ddap.NovoPackage"  # <- Adicione aqui
)
```

## Status Atual

**Pacotes Deprecados:** Ddap.Memory (11 versões)

**Pacotes Ativos (15):**
- Ddap.Aspire
- Ddap.Auth
- Ddap.Client.Core
- Ddap.Client.GraphQL
- Ddap.Client.Grpc
- Ddap.Client.Rest
- Ddap.CodeGen
- Ddap.Core
- Ddap.Data.Dapper
- Ddap.Data.EntityFramework
- Ddap.GraphQL
- Ddap.Grpc
- Ddap.Rest
- Ddap.Subscriptions
- Ddap.Templates

## Documentação Completa

Ver: [UNLIST_PACKAGES_README.md](./UNLIST_PACKAGES_README.md)
