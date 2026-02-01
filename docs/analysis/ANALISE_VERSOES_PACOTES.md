# Análise de Versões de Pacotes - Runtime Atual

## Problema Atual


```xml
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.0.*" />
<PackageReference Include="MySql.EntityFrameworkCore" Version="8.0.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.*" />
```

## Estratégias de Versionamento

### Opção 1: Versão Fixa (Atual - RUIM)
```xml
<PackageReference Include="Package" Version="5.0.1" />
```
❌ Fica desatualizado  
❌ Não recebe patches de segurança  
❌ Requer manutenção manual

### Opção 2: Wildcard Patch (Melhor para Produção)
```xml
<PackageReference Include="Package" Version="5.0.*" />
```
✅ Recebe patches (5.0.x)  
❌ Não pega minor versions (5.1, 5.2)  
⚠️ Pode ficar desatualizado em minor

### Opção 3: Wildcard Minor (Recomendado)
```xml
<PackageReference Include="Package" Version="5.*" />
```
✅ Recebe patches (5.0.x)  
✅ Recebe minor versions (5.1, 5.2)  
✅ Não pula major (breaking changes)  
✅ Melhor balanço

### Opção 4: Latest (PERIGOSO)
```xml
<PackageReference Include="Package" Version="*" />
```
✅ Sempre mais recente  
❌ Pode pegar breaking changes  
❌ Builds não reproduzíveis  
❌ NÃO RECOMENDADO

### Opção 5: Sem Versão (Depende de lock file)
```xml
<PackageReference Include="Package" />
```
⚠️ Depende de packages.lock.json  
⚠️ Comportamento varia

## Recomendação: Usar Major Version Wildcard


```xml
<!-- ADO.NET Drivers -->
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.*" />
<PackageReference Include="MySqlConnector" Version="2.*" />
<PackageReference Include="Npgsql" Version="8.*" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.*" />

<!-- Entity Framework (alinhado com .NET 10) -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />
<PackageReference Include="MySql.EntityFrameworkCore" Version="9.*" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.*" />

<!-- DDAP Packages -->
<PackageReference Include="Ddap.Core" Version="1.*" />
<PackageReference Include="Ddap.Data.Dapper" Version="1.*" />
<PackageReference Include="Ddap.Data.EntityFramework" Version="1.*" />
<PackageReference Include="Ddap.Rest" Version="1.*" />
<PackageReference Include="Ddap.GraphQL" Version="1.*" />
<PackageReference Include="Ddap.Grpc" Version="1.*" />
```

## Versões Atuais no Mercado (Jan 2026)

### ADO.NET Drivers
- **Microsoft.Data.SqlClient**: 5.x (latest stable)
- **MySqlConnector**: 2.x (latest stable)
- **MySql.Data**: 9.x (latest stable) 
- **Npgsql**: 8.x (latest stable)
- **Microsoft.Data.Sqlite**: 8.x (latest stable)

### Entity Framework Core
Para .NET 10 (net10.0):
- **EF Core**: 9.x (última major disponível para .NET 9+)
- Todos os providers devem usar 9.x para compatibilidade

### Atualizações Necessárias

```diff
  <!-- ANTES -->
- <PackageReference Include="Microsoft.Data.SqlClient" Version="5.0.*" />
- <PackageReference Include="MySqlConnector" Version="2.0.*" />
- <PackageReference Include="Npgsql" Version="10.0.*" />
- <PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.*" />

  <!-- DEPOIS -->
+ <PackageReference Include="Microsoft.Data.SqlClient" Version="5.*" />
+ <PackageReference Include="MySqlConnector" Version="2.*" />
+ <PackageReference Include="Npgsql" Version="8.*" />
+ <PackageReference Include="Microsoft.Data.Sqlite" Version="8.*" />
```

```diff
  <!-- EF Core - ANTES -->
- <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.*" />
- <PackageReference Include="MySql.EntityFrameworkCore" Version="8.0.*" />
- <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.*" />
- <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.*" />

  <!-- EF Core - DEPOIS -->
+ <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />
+ <PackageReference Include="MySql.EntityFrameworkCore" Version="9.*" />
+ <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.*" />
+ <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.*" />
```

```diff
  <!-- DDAP - ANTES -->
- <PackageReference Include="Ddap.Core" Version="1.0.*" />
- <PackageReference Include="Ddap.Data.Dapper" Version="1.0.*" />

  <!-- DDAP - DEPOIS -->
+ <PackageReference Include="Ddap.Core" Version="1.*" />
+ <PackageReference Include="Ddap.Data.Dapper" Version="1.*" />
```

## Notas Importantes

### Por Que 9.* para EF Core (não 10.*)?

EF Core 9 é a última versão estável:
- EF Core 9 funciona com .NET 9 e .NET 10
- Não existe EF Core 10 ainda
- Usar 9.* garante última versão estável

### Por Que Npgsql 8.* (não 10.*)?

Npgsql 8.x é a versão stable atual:
- Compatible com .NET 10
- Versão 10 ainda não foi lançada

### Por Que Microsoft.Data.Sqlite 8.* (não 10.*)?

Segue versionamento do .NET SDK:
- Versão 8.x para .NET 8/9/10
- Não existe versão 10 ainda

## Implementação


1. Trocar todos `X.0.*` por `X.*`
2. Corrigir versões incorretas (Npgsql, SQLite)
3. Atualizar MySQL EF Core de 8.* para 9.*
4. Manter consistência em todos os pacotes

## Benefícios

✅ **Sempre Atual**: Pega patches e minor versions automaticamente  
✅ **Segurança**: Patches de segurança aplicados automaticamente  
✅ **Estabilidade**: Não pega breaking changes (major versions)  
✅ **Reproduzível**: Com lock file, builds são consistentes
