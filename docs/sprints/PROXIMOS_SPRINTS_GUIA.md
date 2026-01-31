# Próximos Sprints - Guia Rápido de Implementação

## 📊 Status dos Sprints

### ✅ Sprint 1: Fix Template API Provider Flags
**Status**: COMPLETO  
**Branch**: `feat/fix-template-flags`  
**Documento**: SPRINT1_PR_INSTRUCTIONS.md

---

## 🚀 Sprint 2: Resolve Package References

**Status**: AGUARDANDO Sprint 1  
**Tempo Estimado**: 6-11 horas  
**Prioridade**: 🔴 CRÍTICO

### Objetivo

Remover referências a pacotes inexistentes (`Ddap.Data.Dapper.SqlServer`, etc.) e usar apenas o pacote base + driver escolhido pelo usuário.

### Problema

Template referencia pacotes que não existem no repositório:
- `Ddap.Data.Dapper.SqlServer` ❌
- `Ddap.Data.Dapper.MySQL` ❌
- `Ddap.Data.Dapper.PostgreSQL` ❌

Estes pacotes precisam ser **OU**:
- A) Criados no repositório
- B) Removidos do template (usar `Ddap.Data.Dapper` base + driver do usuário)

**Recomendação**: Opção B (filosofia "Developer in Control")

### Arquivos a Modificar

1. **templates/ddap-api/DdapApi.csproj** (linhas 12-24)
   - Remover referências a pacotes inexistentes
   - Usar `Ddap.Data.Dapper` base + driver específico (Microsoft.Data.SqlClient, MySqlConnector, etc.)

2. **templates/ddap-api/Program.cs**
   - Atualizar using statements
   - Remover referências a `Ddap.Data.Dapper.SqlServer` namespace (não existe)

3. **docs/database-providers.md**
   - Documentar qual driver instalar para cada banco

### Como Começar

```bash
# Após Sprint 1 estar merged no epic
git checkout copilot/improve-ddap-project
git pull origin copilot/improve-ddap-project
git checkout -b feat/resolve-packages

# Fazer as mudanças...
# Testar com cada banco de dados
# Commit e push
```

### Testes Necessários

- [ ] Template com SQL Server + Dapper
- [ ] Template com MySQL + Dapper
- [ ] Template com PostgreSQL + Dapper
- [ ] Template com SQLite + Dapper
- [ ] Template com SQL Server + EF
- [ ] Template com MySQL + EF (verificar Pomelo)
- [ ] Template com PostgreSQL + EF
- [ ] Template com SQLite + EF
- [ ] Projetos gerados compilam
- [ ] Documentação atualizada

---

## 📚 Sprint 3: Update Documentation Site

**Status**: AGUARDANDO Sprint 2  
**Tempo Estimado**: 5-9 horas  
**Prioridade**: 🟡 IMPORTANTE

### Objetivo

Atualizar site de documentação com avisos, ícone, página "Why DDAP?" e publicar.

### Tarefas

1. **Avisos sobre Issues Conhecidos** (1-2h)
   - Adicionar banner no README.md
   - Criar `docs/known-issues.md`
   - Gerar HTML e traduzir

2. **Integrar Ícone** (1-2h)
   - Copiar `icons/icon.svg` para `docs/`
   - Atualizar `docs/index.html`
   - Regenerar locales (7 idiomas)

3. **Página "Why DDAP?"** (2-3h)
   - Criar `docs/why-ddap.md`
   - Traduzir para 6 idiomas
   - Adicionar ao menu

4. **Publicar Website** (1-2h)
   - Verificar GitHub Actions
   - Deploy para https://schivei.github.io/ddap
   - Testar todos os links

### Como Começar

```bash
# Após Sprint 2 estar merged no epic
git checkout copilot/improve-ddap-project
git pull origin copilot/improve-ddap-project
git checkout -b feat/update-docs-site

# Fazer as mudanças...
# Testar localmente: cd docs && python3 -m http.server 8000
# Commit e push
```

---

## ✅ Sprint 4: Add Template Tests

**Status**: AGUARDANDO Sprint 3  
**Tempo Estimado**: 8-12 horas  
**Prioridade**: 🟢 MELHORIA

### Objetivo

Criar testes automatizados para o template que rodam no CI e previnem regressões.

### Tarefas

1. **Script de Validação** (4-6h)
   - Criar `tests/template-validation.sh`
   - Testar 64+ cenários
   - Validar que pacotes corretos estão incluídos
   - Validar que projetos compilam

2. **Integração com CI** (2-3h)
   - Adicionar em `.github/workflows/build.yml`
   - Executar testes em PRs
   - Bloquear merge se testes falharem

3. **Documentação** (2-3h)
   - README em `tests/`
   - Como executar localmente
   - Como adicionar novos testes

### Como Começar

```bash
# Após Sprint 3 estar merged no epic
git checkout copilot/improve-ddap-project
git pull origin copilot/improve-ddap-project
git checkout -b feat/add-template-tests

# Criar script de teste...
# Integrar com CI...
# Commit e push
```

### Cenários de Teste (mínimo)

**Combinações de Banco de Dados** (4):
- SQL Server, MySQL, PostgreSQL, SQLite

**Combinações de ORM** (2):
- Dapper, Entity Framework

**Combinações de API** (7):
- Nenhuma API
- REST apenas
- GraphQL apenas
- gRPC apenas
- REST + GraphQL
- REST + gRPC
- Todas as APIs

**Total**: 4 × 2 × 7 = 56 cenários base

**Adicionais**:
- Com/sem Auth (+2)
- Com/sem Subscriptions (+2)
- Com/sem Aspire (+2)

**Total Expandido**: 56 × 6 = 336 cenários possíveis (testar subset representativo)

---

## 🔄 Workflow Geral

### Para Cada Sprint:

1. **Aguardar Sprint Anterior**
   - Sprint anterior deve estar merged no epic
   - Epic branch atualizada

2. **Criar Branch**
   ```bash
   git checkout copilot/improve-ddap-project
   git pull origin copilot/improve-ddap-project
   git checkout -b feat/<nome-do-sprint>
   ```

3. **Implementar**
   - Fazer as mudanças
   - Testar localmente
   - Validar extensivamente

4. **Commit e Push**
   ```bash
   git add .
   git commit -m "Mensagem descritiva"
   git push -u origin feat/<nome-do-sprint>
   ```

5. **Criar PR no GitHub**
   - **Base**: `copilot/improve-ddap-project` (epic branch)
   - **Compare**: `feat/<nome-do-sprint>`
   - **Title**: Sprint X: Nome do Sprint
   - **Description**: Usar template do documento de instruções

6. **Review e Merge**
   - Code review
   - Aprovação
   - Merge para epic branch

7. **Próximo Sprint**
   - Repetir processo

---

## 📋 Checklist Geral do Épico

### Sprint 1
- [x] Implementado
- [x] Testado
- [x] Documentado
- [ ] PR criado
- [ ] Aprovado
- [ ] Merged para epic

### Sprint 2
- [ ] Aguardando Sprint 1
- [ ] Implementado
- [ ] Testado
- [ ] Documentado
- [ ] PR criado
- [ ] Aprovado
- [ ] Merged para epic

### Sprint 3
- [ ] Aguardando Sprint 2
- [ ] Implementado
- [ ] Testado
- [ ] Documentado
- [ ] PR criado
- [ ] Aprovado
- [ ] Merged para epic

### Sprint 4
- [ ] Aguardando Sprint 3
- [ ] Implementado
- [ ] Testado
- [ ] Documentado
- [ ] PR criado
- [ ] Aprovado
- [ ] Merged para epic

### Epic Final
- [ ] Todos os sprints completos
- [ ] Epic validado como um todo
- [ ] PR final: epic → main
- [ ] Aprovado
- [ ] Merged para main
- [ ] Deploy em produção

---

## 📚 Referências

- **ROTEIRO_ACOES.md**: Detalhes completos de cada ação
- **ESTRATEGIA_EPICO.md**: Como funciona o fluxo de épico
- **ANALISE_TEMPO_PRODUTIVIDADE.md**: Estimativas de tempo
- **SPRINT1_PR_INSTRUCTIONS.md**: Exemplo completo de um sprint

---

**Última Atualização**: 31 de Janeiro de 2026  
**Status**: Sprint 1 completo, aguardando criação de PR
