# Começando com DDAP

Bem-vindo ao DDAP (Dynamic Data API Provider)! Este guia ajudará você a começar a criar APIs REST, gRPC e GraphQL automáticas a partir do seu esquema de banco de dados.

## O que é DDAP?

DDAP é uma biblioteca .NET 10 que gera automaticamente APIs a partir do seu esquema de banco de dados. Aponte-o para o seu banco de dados e ele irá:

- Carregar os metadados do seu banco de dados (tabelas, colunas, relacionamentos, índices)
- Gerar endpoints REST, gRPC e/ou GraphQL
- Suportar negociação de conteúdo JSON e XML
- Fornecer extensibilidade através de classes parciais

## Pré-requisitos

Antes de começar, certifique-se de ter:

- **.NET 10 SDK** ou posterior instalado
- Um banco de dados (SQL Server, MySQL ou PostgreSQL)
- Conhecimento básico de ASP.NET Core
- Sua string de conexão do banco de dados

## Início Rápido com Templates

A maneira mais rápida de começar com DDAP é usando o template de projeto:

```bash
# Instalar o template
dotnet new install Ddap.Templates

# Criar uma nova API DDAP (modo interativo solicitará)
dotnet new ddap-api --name MyDdapApi
cd MyDdapApi

# Executar sua API
dotnet run
```

É isso! Sua API agora está em execução com endpoints gerados automaticamente para seu banco de dados.

> **Saiba mais:** Veja [Guia de Templates](./templates.md) para opções detalhadas de template e personalização.

## Instalação Manual

Se você preferir configuração manual ou quiser adicionar DDAP a um projeto existente:

### Passo 1: Criar um Novo Projeto ASP.NET Core

```bash
dotnet new webapi -n MyDdapApi
cd MyDdapApi
```

### Passo 2: Instalar Pacotes NuGet

```bash
# Pacote principal
dotnet add package Ddap.Core

# Escolha seu provedor de banco de dados
dotnet add package Ddap.Data.Dapper          # Para qualquer banco de dados
# OU
dotnet add package Ddap.Data.EntityFramework # Se você já usa EF Core

# Escolha seus provedores de API
dotnet add package Ddap.Rest      # APIs REST
dotnet add package Ddap.GraphQL   # APIs GraphQL (opcional)
dotnet add package Ddap.Grpc      # APIs gRPC (opcional)
```

### Passo 3: Configurar DDAP

Edite seu `Program.cs`:

```csharp
using Microsoft.Data.SqlClient;
using Ddap.Data.Dapper;

var builder = WebApplication.CreateBuilder(args);

// Configurar DDAP
builder.Services.AddDdap()
    .AddDapper(() => new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ))
    .AddRest()
    .AddGraphQL();

var app = builder.Build();

// Mapear endpoints
app.MapControllers();
app.MapGraphQL();

app.Run();
```

### Passo 4: Adicionar String de Conexão

Edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDatabase;Integrated Security=true;"
  }
}
```

## Próximos Passos

- [Arquitetura](./architecture.md) - Entenda como o DDAP funciona
- [Provedores de Banco de Dados](./database-providers.md) - Escolha Dapper vs Entity Framework
- [Provedores de API](./api-providers.md) - Configure REST, GraphQL e gRPC
- [Uso Avançado](./advanced.md) - Personalize e estenda sua API
