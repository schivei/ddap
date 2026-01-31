# Análise de Pacotes Oficiais - Todos os Providers

## Status Atual dos Pacotes

### Database Providers (ADO.NET/Dapper)

#### SQL Server + Dapper ✅ OFICIAL
```xml
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.0.*" />
```
- **Fornecedor**: Microsoft
- **Status**: ✅ Oficial
- **Notas**: Pacote oficial da Microsoft para SQL Server

#### MySQL + Dapper ⚠️ VERIFICAR
```xml
<PackageReference Include="MySqlConnector" Version="2.0.*" />
```
- **Fornecedor**: Comunidade (MySqlConnector)
- **Status**: ⚠️ NÃO É O OFICIAL
- **Oficial**: `MySql.Data` (Oracle)
- **Problema**: MySqlConnector é comunitário, não oficial

#### PostgreSQL + Dapper ✅ OFICIAL
```xml
<PackageReference Include="Npgsql" Version="10.0.*" />
```
- **Fornecedor**: Npgsql Development Team (oficial)
- **Status**: ✅ Oficial
- **Notas**: Npgsql é o driver oficial para PostgreSQL no .NET

#### SQLite + Dapper ✅ OFICIAL
```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.*" />
```
- **Fornecedor**: Microsoft
- **Status**: ✅ Oficial
- **Notas**: Implementação oficial da Microsoft

---

### Entity Framework Core Providers

#### SQL Server + EF Core ✅ OFICIAL
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.*" />
```
- **Fornecedor**: Microsoft
- **Status**: ✅ Oficial

#### MySQL + EF Core ✅ OFICIAL (CORRIGIDO)
```xml
<PackageReference Include="MySql.EntityFrameworkCore" Version="8.0.*" />
```
- **Fornecedor**: Oracle
- **Status**: ✅ Oficial
- **Notas**: Corrigido no Sprint 2

#### PostgreSQL + EF Core ✅ OFICIAL
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.*" />
```
- **Fornecedor**: Npgsql Development Team (oficial)
- **Status**: ✅ Oficial

#### SQLite + EF Core ✅ OFICIAL
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.*" />
```
- **Fornecedor**: Microsoft
- **Status**: ✅ Oficial

---

## Problemas Identificados

### 1. MySQL + Dapper usa MySqlConnector (Comunitário)

**Atual**:
```xml
<PackageReference Include="MySqlConnector" Version="2.0.*" />
```

**Deveria ser**:
```xml
<PackageReference Include="MySql.Data" Version="9.0.*" />
```

**Análise**:
- `MySqlConnector` é um driver comunitário (não mantido pela Oracle)
- `MySql.Data` é o driver oficial ADO.NET da Oracle
- Para consistência, devemos usar o oficial

**PORÉM**: Há um dilema:
- `MySqlConnector` tem melhor performance e é async-first
- `MySql.Data` é oficial mas tem issues de performance conhecidos
- Comunidade .NET prefere MySqlConnector

**Decisão necessária**: 
1. Seguir filosofia estrita: usar `MySql.Data` (oficial)
2. Exceção pragmática: usar `MySqlConnector` (melhor, mas comunitário)

---

## Recomendações

### Opção A: Estrita (Apenas Oficiais)
Trocar `MySqlConnector` por `MySql.Data`

**Prós**:
- ✅ Filosofia consistente
- ✅ 100% pacotes oficiais
- ✅ Suporte direto da Oracle

**Contras**:
- ❌ Performance inferior
- ❌ Problemas conhecidos com async
- ❌ Comunidade prefere MySqlConnector

### Opção B: Pragmática (Exceção para MySQL Dapper)
Manter `MySqlConnector` com documentação explicando

**Prós**:
- ✅ Melhor performance
- ✅ Async-first design
- ✅ Preferido pela comunidade
- ✅ Mantido ativamente

**Contras**:
- ❌ Não é oficial Oracle
- ❌ Inconsistência na filosofia

### Opção C: Deixar Usuário Escolher (Como MySQL EF)
Sem pacote padrão, usuário adiciona

**Prós**:
- ✅ Usuário tem controle total

**Contras**:
- ❌ Projeto não funciona out-of-the-box
- ❌ Inconsistente com outros bancos

---

## Recomendação Final

**Opção B (Pragmática)** com documentação clara:

1. **Manter `MySqlConnector` para Dapper**
   - Justificativa: Performance e async superior
   - Documentar no README que é comunitário mas preferido
   - Oferecer instruções para trocar por `MySql.Data`

2. **Manter `MySql.EntityFrameworkCore` para EF Core**
   - É oficial Oracle
   - Funciona bem com EF Core

3. **Documentar Exceção**
   - README deve explicar que MySQL Dapper é exceção
   - Razões técnicas (performance, async)
   - Como trocar se preferir oficial

---

## Resumo de Conformidade

| Provider | Dapper | EF Core |
|----------|--------|---------|
| SQL Server | ✅ Microsoft oficial | ✅ Microsoft oficial |
| **MySQL** | ⚠️ MySqlConnector (comunitário) | ✅ Oracle oficial |
| PostgreSQL | ✅ Npgsql oficial | ✅ Npgsql oficial |
| SQLite | ✅ Microsoft oficial | ✅ Microsoft oficial |

**Score**: 7/8 oficial (87.5%)

---

## Decisão Necessária

**Pergunta para o usuário**: 

Para MySQL + Dapper, qual abordagem seguir?

1. **Estrita**: Trocar para `MySql.Data` (oficial, mas performance inferior)
2. **Pragmática**: Manter `MySqlConnector` (comunitário, mas superior)
3. **Escolha do Usuário**: Sem pacote padrão

**Minha recomendação**: Opção 2 (Pragmática) com documentação clara da exceção e razões técnicas.
