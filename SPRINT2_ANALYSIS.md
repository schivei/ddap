# Sprint 2: Package References Analysis

## Problemas Identificados

### 1. Pacotes Inexistentes Referenciados

**DdapApi.csproj** referencia pacotes que não existem no NuGet:

#### Dapper (Linhas 13-19)
- `Ddap.Data.Dapper.SqlServer` ❌ NÃO EXISTE
- `Ddap.Data.Dapper.MySQL` ❌ NÃO EXISTE
- `Ddap.Data.Dapper.PostgreSQL` ❌ NÃO EXISTE

#### SQLite (Linha 22-23)
- Usa `Ddap.Data.Dapper` ✅ EXISTE
- Usa `Microsoft.Data.Sqlite` ✅ EXISTE

### 2. Pomelo Forçado (Linha 31)

```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="10.0.*" />
```

**Problema**: Viola filosofia "Developer in Control"
- Pomelo é um package comunitário (não oficial da Oracle)
- Usuário não tem escolha
- Documentação não explica alternativa oficial

### 3. Inconsistência no Program.cs

**Program.cs** também referencia namespaces inexistentes:
- Linha 3: `using Ddap.Data.Dapper.SqlServer;` ❌
- Linha 6: `using Ddap.Data.Dapper.MySQL;` ❌
- Linha 9: `using Ddap.Data.Dapper.PostgreSQL;` ❌

E chama métodos de extensão inexistentes:
- Linha 57: `ddapBuilder.AddSqlServerDapper();` ❌
- Linha 60: `ddapBuilder.AddMySqlDapper();` ❌
- Linha 63: `ddapBuilder.AddPostgreSqlDapper();` ❌

## Solução Proposta

### Estratégia: Pacote Base + Driver do Usuário

Seguir o modelo do SQLite (que já está correto):
- Usar pacote base DDAP (`Ddap.Data.Dapper`)
- Usuário adiciona driver ADO.NET apropriado
- Documentar claramente as opções

### Correções Específicas

#### A. SQL Server + Dapper
```xml
<!-- Antes (NÃO FUNCIONA) -->
<PackageReference Include="Ddap.Data.Dapper.SqlServer" Version="1.0.*" />

<!-- Depois (CORRETO) -->
<PackageReference Include="Ddap.Data.Dapper" Version="1.0.*" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.0.*" />
```

**Program.cs**:
```csharp
// Antes
using Ddap.Data.Dapper.SqlServer;
ddapBuilder.AddSqlServerDapper();

// Depois
using Ddap.Data.Dapper;
ddapBuilder.AddDapper(); // Detecta provider automaticamente
```

#### B. MySQL + Dapper
```xml
<!-- Antes (NÃO FUNCIONA) -->
<PackageReference Include="Ddap.Data.Dapper.MySQL" Version="1.0.*" />

<!-- Depois (CORRETO) -->
<PackageReference Include="Ddap.Data.Dapper" Version="1.0.*" />
<PackageReference Include="MySqlConnector" Version="2.0.*" />
```

**Program.cs**:
```csharp
// Antes
using Ddap.Data.Dapper.MySQL;
ddapBuilder.AddMySqlDapper();

// Depois
using Ddap.Data.Dapper;
ddapBuilder.AddDapper();
```

#### C. PostgreSQL + Dapper
```xml
<!-- Antes (NÃO FUNCIONA) -->
<PackageReference Include="Ddap.Data.Dapper.PostgreSQL" Version="1.0.*" />

<!-- Depois (CORRETO) -->
<PackageReference Include="Ddap.Data.Dapper" Version="1.0.*" />
<PackageReference Include="Npgsql" Version="10.0.*" />
```

**Program.cs**:
```csharp
// Antes
using Ddap.Data.Dapper.PostgreSQL;
ddapBuilder.AddPostgreSqlDapper();

// Depois
using Ddap.Data.Dapper;
ddapBuilder.AddDapper();
```

#### D. MySQL + Entity Framework
```xml
<!-- Antes (FORÇADO) -->
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="10.0.*" />

<!-- Depois (USUÁRIO ESCOLHE) -->
<!-- Opção será documentada, não forçada -->
```

**Opções de Driver MySQL** (documentadas):
1. **Pomelo** (comunitário, popular): `Pomelo.EntityFrameworkCore.MySql`
2. **MySQL.EntityFrameworkCore** (oficial Oracle): `MySql.EntityFrameworkCore`

## Impacto na Filosofia

### Antes (Violações)
❌ Força Pomelo sem explicação  
❌ Referencia pacotes inexistentes  
❌ Usuário não tem controle  

### Depois (Alinhado)
✅ Usuário escolhe o driver MySQL  
✅ Apenas pacotes que existem  
✅ Documentação clara das opções  
✅ "Developer in Control" respeitado  

## Arquivos a Modificar

1. **templates/ddap-api/DdapApi.csproj** - Corrigir package references
2. **templates/ddap-api/Program.cs** - Corrigir usings e chamadas de método
3. **templates/ddap-api/README.md** - Adicionar guia de drivers (CRIAR)
4. **docs/database-providers.md** - Documentar escolhas MySQL
5. **README.md** - Atualizar seção de pacotes (se necessário)

## Próximos Passos

1. ✅ Análise completa (este documento)
2. [ ] Implementar correções no .csproj
3. [ ] Implementar correções no Program.cs
4. [ ] Criar README no template explicando drivers
5. [ ] Atualizar docs/database-providers.md
6. [ ] Testar geração com cada combinação
7. [ ] Validar builds
8. [ ] Commit e push
