# Problemas Conhecidos

Este documento lista as limitações e problemas conhecidos do DDAP que os usuários devem estar cientes.

## Limitações Atuais

### 1. Requisito do .NET 10

**Problema:** DDAP requer o SDK do .NET 10.

**Status:** Por design - DDAP usa os recursos mais recentes do .NET para desempenho e experiência do desenvolvedor.

**Solução alternativa:** Instale o SDK do .NET 10 de https://dotnet.microsoft.com/download

---

### 2. Chaves Primárias Obrigatórias

**Problema:** Tabelas sem chaves primárias não são descobríveis pelo DDAP.

**Status:** Por design - DDAP requer chaves primárias para identificação de entidades.

**Solução alternativa:** Adicione chaves primárias a todas as tabelas que você deseja expor via API.

```sql
-- Adicionar chave primária a tabela existente
ALTER TABLE MinhaTabela ADD CONSTRAINT PK_MinhaTabela PRIMARY KEY (Id);
```

---

### 3. Migração de Pacotes (v0.x para v1.0+)

**Problema:** Pacotes Dapper específicos de banco de dados (`Ddap.Data.Dapper.SqlServer`, etc.) não existem mais.

**Status:** Mudança arquitetural intencional - Seguindo a filosofia "Desenvolvedor no Controle".

**Migração:**

**Antigo** (v0.x):
```xml
<PackageReference Include="Ddap.Data.Dapper.SqlServer" Version="0.x" />
```

**Novo** (v1.0+):
```xml
<PackageReference Include="Ddap.Data.Dapper" Version="1.*" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.*" />
```

Consulte [Solução de Problemas - Migração da API Antiga](troubleshooting.md#migration-from-old-api) para guia completo de migração.

---

### 4. Desempenho do Auto-Reload

**Problema:** O recarregamento do esquema pode causar breve interrupção do serviço ou aumento do uso de memória.

**Status:** Conhecido - Compromisso entre zero downtime e uso de recursos.

**Soluções alternativas:**

1. **Para zero downtime:**
   ```csharp
   options.AutoReload.Behavior = ReloadBehavior.ServeOldSchema;
   ```

2. **Para memória mínima:**
   ```csharp
   options.AutoReload.Strategy = ReloadStrategy.HotReload;
   ```

3. **Aumentar timeout de inatividade para reduzir frequência de recarga:**
   ```csharp
   options.AutoReload.IdleTimeout = TimeSpan.FromMinutes(15);
   ```

Consulte [Documentação de Auto-Reload](auto-reload.md) para detalhes.

---

### 5. Geração de Templates - Suporte ao Aspire

**Problema:** Templates com integração Aspire podem requerer configuração manual adicional.

**Status:** Conhecido - Integração Aspire é complexa e pode precisar de customização.

**Solução alternativa:** Siga o [Guia de Introdução](get-started.md) para instruções de configuração.

---

## Reportando Problemas

Encontrou um bug ou limitação não listada aqui?

1. **Verifique:** [GitHub Issues](https://github.com/schivei/ddap/issues)
2. **Busque:** [Guia de Solução de Problemas](troubleshooting.md)
3. **Reporte:** [Abra uma nova issue](https://github.com/schivei/ddap/issues/new)

Ao reportar, inclua:
- Versão do DDAP (versão do pacote `Ddap.Core`)
- Versão do .NET (`dotnet --version`)
- Tipo e versão do banco de dados
- Mensagem de erro e stack trace
- Código mínimo de reprodução

---

## Problemas Corrigidos

Problemas que foram resolvidos em versões recentes:

### ✅ Escolha do Provedor MySQL (Corrigido na 1.0.0)

**Era:** Template forçava `Pomelo.EntityFrameworkCore.MySql`.

**Agora:** Usa o `MySql.EntityFrameworkCore` oficial por padrão, com Pomelo como alternativa documentada.

Usuário mantém controle total sobre a escolha do provedor MySQL.

---

### ✅ Pacotes Inexistentes (Corrigido na 1.0.0)

**Era:** Templates referenciavam pacotes inexistentes como `Ddap.Data.Dapper.SqlServer`.

**Agora:** Usa o pacote base `Ddap.Data.Dapper` com drivers oficiais de banco de dados.

Todos os templates geram corretamente com referências de pacotes adequadas.

---

## Próximos Passos

- [Solução de Problemas](troubleshooting.md) - Soluções detalhadas para problemas comuns
- [Primeiros Passos](get-started.md) - Comece a usar o DDAP
- [Filosofia](philosophy.md) - Entendendo a abordagem "Desenvolvedor no Controle" do DDAP
- [GitHub Issues](https://github.com/schivei/ddap/issues) - Reporte bugs e solicite recursos
