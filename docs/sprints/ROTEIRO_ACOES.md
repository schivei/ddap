# Roteiro de Ações - DDAP Project

**Data de Criação**: 30 de Janeiro de 2026  
**Última Atualização**: 31 de Janeiro de 2026  
**Objetivo**: Guia passo a passo para correção dos problemas identificados, sem perder o foco

---

## ⏱️ NOTA SOBRE ESTIMATIVAS DE TEMPO

**Estimativas revisadas** para serem mais realistas baseadas na complexidade das tarefas:
- **Original**: 13-17 horas total
- **Revisado**: 25-39 horas total (ver ANALISE_TEMPO_PRODUTIVIDADE.md)

As estimativas consideram:
- Debugging e iterações
- Testes extensivos (64+ cenários)
- Code review e refinamentos
- Documentação
- Imprevistos (~20-30% buffer)

**Recomendação**: Trabalhe em **sprints pequenos** (1-2 ações por vez) com PRs separados.

---

## 📋 Visão Geral

Este documento organiza todas as ações necessárias em ordem de prioridade, com passos claros e objetivos mensuráveis. Trabalhe **um item por vez**, marcando como concluído antes de passar ao próximo.

**Legenda de Prioridade**:
- 🔴 **CRÍTICO**: Bloqueia usuários, deve ser resolvido imediatamente
- 🟡 **IMPORTANTE**: Impacta experiência, resolver logo após críticos
- 🟢 **MELHORIA**: Adiciona valor, pode ser feito quando houver tempo

---

## Fase 1: Correções Críticas (Bloqueadores) 🔴

Estes problemas impedem o uso básico do DDAP e devem ser corrigidos **imediatamente**.

### ✅ Ação 1.1: Corrigir Template - API Provider Flags

**Prioridade**: 🔴 **CRÍTICO**  
**Tempo Estimado**: 4-6 horas (revisado de 2-4h)  
**Status**: [ ] Não iniciado

**Problema**:
- Flags `--rest`, `--graphql`, `--grpc` não funcionam
- Template gera projetos sem APIs
- 100% dos novos usuários afetados

**Passos de Execução**:

1. **Analisar o problema** (30 min)
   - [ ] Abrir `templates/ddap-api/.template.config/template.json`
   - [ ] Localizar seção `"IncludeRest"`, `"IncludeGraphQL"`, `"IncludeGrpc"`
   - [ ] Identificar a expressão booleana problemática

2. **Implementar a correção** (1-2 horas)
   
   **Opção A - Simplificar (RECOMENDADO)**:
   ```json
   // Substituir computed symbols por parâmetros diretos
   "IncludeRest": {
     "type": "parameter",
     "datatype": "bool",
     "defaultValue": "true"
   }
   ```
   
   **Opção B - Corrigir avaliação**:
   ```json
   // Se precisar manter computed, ajustar lógica
   "IncludeRest": {
     "type": "computed",
     "value": "(rest == true)"
   }
   ```

3. **Testar a correção** (1 hora)
   - [ ] Criar projeto: `dotnet new ddap-api --name Test1 --rest true`
   - [ ] Verificar se `Ddap.Rest` está no .csproj
   - [ ] Verificar se código REST está no Program.cs
   - [ ] Criar projeto: `dotnet new ddap-api --name Test2 --graphql true`
   - [ ] Verificar se `Ddap.GraphQL` está incluído
   - [ ] Testar todas as combinações (REST+GraphQL, REST+gRPC, etc.)

4. **Validar build** (30 min)
   - [ ] Build do projeto gerado: `dotnet build`
   - [ ] Verificar se não há erros de compilação
   - [ ] Testar com diferentes bancos de dados

5. **Documentar e publicar** (30 min)
   - [ ] Atualizar CHANGELOG.md
   - [ ] Incrementar versão para 1.0.3
   - [ ] Commit: "Fix template API provider flags evaluation"
   - [ ] Publicar novo pacote no NuGet

**Critérios de Sucesso**:
- ✅ `dotnet new ddap-api --rest true` inclui Ddap.Rest
- ✅ `dotnet new ddap-api --graphql true` inclui Ddap.GraphQL
- ✅ Projeto gerado compila sem erros
- ✅ Testes automatizados passam

**Arquivos a Modificar**:
- `templates/ddap-api/.template.config/template.json`

**Dependências**: Nenhuma

---

### ✅ Ação 1.2: Resolver Referências a Pacotes Inexistentes

**Prioridade**: 🔴 **CRÍTICO**  
**Tempo Estimado**: 4-8 horas (revisado de 2-3h)  
**Status**: [ ] Não iniciado

**Problema**:
- Template referencia `Ddap.Data.Dapper.SqlServer`, `Ddap.Data.Dapper.MySQL`, etc.
- Esses pacotes NÃO EXISTEM no repositório
- Projetos gerados não compilam

**Decisão Necessária**:

**Opção A - Usar Apenas Pacote Base (RECOMENDADO - Alinhado com Filosofia)**:

Passos:
1. **Modificar template** (1 hora)
   - [ ] Editar `templates/ddap-api/DdapApi.csproj`
   - [ ] Substituir:
     ```xml
     <!-- De: -->
     <PackageReference Include="Ddap.Data.Dapper.SqlServer" Version="1.0.*" />
     
     <!-- Para: -->
     <PackageReference Include="Ddap.Data.Dapper" Version="1.0.*" />
     <PackageReference Include="Microsoft.Data.SqlClient" Version="5.0.*" />
     ```

2. **Atualizar Program.cs** (1 hora)
   - [ ] Editar `templates/ddap-api/Program.cs`
   - [ ] Substituir:
     ```csharp
     // De:
     using Ddap.Data.Dapper.SqlServer;
     ddapBuilder.AddSqlServerDapper();
     
     // Para:
     using Ddap.Data.Dapper;
     using Microsoft.Data.SqlClient;
     ddapBuilder.AddDapper(() => new SqlConnection(connectionString));
     ```

3. **Repetir para outros bancos** (1 hora)
   - [ ] MySQL: usar `MySqlConnector`
   - [ ] PostgreSQL: usar `Npgsql`
   - [ ] SQLite: usar `Microsoft.Data.Sqlite`

**Opção B - Criar Pacotes Faltantes** (Mais trabalho, mas mais amigável):

Passos:
1. **Criar Ddap.Data.Dapper.SqlServer** (2 horas)
   - [ ] Criar projeto em `src/Ddap.Data.Dapper.SqlServer/`
   - [ ] Adicionar dependência em Microsoft.Data.SqlClient
   - [ ] Implementar extensão `AddSqlServerDapper()`
   - [ ] Testar

2. **Repetir para MySQL, PostgreSQL** (4 horas cada)

**Recomendação**: **Opção A** - mais rápido e alinhado com filosofia "Developer in Control"

**Critérios de Sucesso**:
- ✅ Projeto gerado compila sem erros
- ✅ Sem referências a pacotes inexistentes
- ✅ Documentação atualizada explicando dependências

**Arquivos a Modificar**:
- `templates/ddap-api/DdapApi.csproj`
- `templates/ddap-api/Program.cs`
- `docs/database-providers.md`

**Dependências**: Deve ser feito após Ação 1.1

---

### ✅ Ação 1.3: Remover/Documentar Dependência Forçada do Pomelo

**Prioridade**: 🔴 **CRÍTICO** (Violação de Filosofia)  
**Tempo Estimado**: 2-3 horas (revisado de 1-2h)  
**Status**: [ ] Não iniciado

**Problema**:
- Template força `Pomelo.EntityFrameworkCore.MySql` (pacote comunitário)
- Usuário não pode escolher `MySql.EntityFrameworkCore` (oficial)
- Contradiz filosofia "Developer in Control"

**Decisão Necessária**:

**Opção A - Remover Dependência Forçada (RECOMENDADO)**:

Passos:
1. **Modificar template** (30 min)
   - [ ] Editar `templates/ddap-api/DdapApi.csproj`
   - [ ] Remover linha 31:
     ```xml
     <!-- REMOVER: -->
     <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="10.0.*" />
     ```

2. **Documentar escolha** (30 min)
   - [ ] Adicionar em `templates/ddap-api/README.md`:
     ```markdown
     ## MySQL com Entity Framework
     
     Escolha o provider MySQL:
     
     **Opção 1 - Pomelo (Comunitário, mais popular)**:
     ```bash
     dotnet add package Pomelo.EntityFrameworkCore.MySql
     ```
     
     **Opção 2 - Oficial (Oracle)**:
     ```bash
     dotnet add package MySql.EntityFrameworkCore
     ```
     ```

**Opção B - Adicionar Mecanismo de Escolha**:

Passos:
1. **Adicionar parâmetro ao template** (1 hora)
   ```json
   "mysql-ef-provider": {
     "type": "parameter",
     "datatype": "choice",
     "defaultValue": "none",
     "choices": [
       { "choice": "pomelo", "description": "Pomelo (Community)" },
       { "choice": "official", "description": "MySql.EntityFrameworkCore (Official)" },
       { "choice": "none", "description": "User will add manually" }
     ]
   }
   ```

2. **Atualizar condicionais** (30 min)

**Recomendação**: **Opção A** - mais simples e filosoficamente correto

**Critérios de Sucesso**:
- ✅ Template não força pacotes não-oficiais
- ✅ Documentação explica escolhas
- ✅ Usuário tem controle total

**Arquivos a Modificar**:
- `templates/ddap-api/DdapApi.csproj`
- `templates/ddap-api/README.md`
- `docs/database-providers.md`

**Dependências**: Nenhuma (pode ser feito em paralelo com 1.1)

---

## Fase 2: Documentação e Website 🟡

Melhorias importantes para experiência do usuário e marketing do projeto.

### ✅ Ação 2.1: Adicionar Aviso sobre Issues Conhecidos

**Prioridade**: 🟡 **IMPORTANTE**  
**Tempo Estimado**: 1-2 horas (revisado de 30min)  
**Status**: [ ] Não iniciado

**Objetivo**: Avisar usuários sobre problemas conhecidos enquanto correções não estão prontas

**Passos**:

1. **Adicionar banner no README** (10 min)
   - [ ] Editar `/home/runner/work/ddap/ddap/README.md`
   - [ ] Adicionar após título:
     ```markdown
     > ⚠️ **Aviso Importante**: Há issues conhecidos no template. 
     > [Veja issues conhecidos](https://github.com/schivei/ddap/issues) 
     > ou use configuração manual até correção. Fix em andamento.
     ```

2. **Adicionar página de issues conhecidos no site** (20 min)
   - [ ] Criar `docs/known-issues.md`
   - [ ] Listar problemas com workarounds
   - [ ] Gerar HTML: `./generate-doc-pages.sh`
   - [ ] Adicionar ao menu de navegação

**Critérios de Sucesso**:
- ✅ Aviso visível no README
- ✅ Página de issues no site
- ✅ Workarounds documentados

**Arquivos a Criar/Modificar**:
- `README.md`
- `docs/known-issues.md`

**Dependências**: Nenhuma

---

### ✅ Ação 2.2: Integrar Ícone no Website

**Prioridade**: 🟡 **IMPORTANTE**  
**Tempo Estimado**: 1-2 horas (revisado de 1h)  
**Status**: [ ] Não iniciado

**Objetivo**: Adicionar ícone profissional criado ao site de documentação

**Passos**:

1. **Copiar ícone para docs** (5 min)
   - [ ] `cp icons/icon.svg docs/`
   - [ ] Verificar se arquivo está acessível

2. **Atualizar index.html** (20 min)
   - [ ] Editar `docs/index.html`
   - [ ] Adicionar no hero section:
     ```html
     <div class="hero-icon" style="text-align: center; margin-bottom: 2rem;">
       <img src="icon.svg" alt="DDAP - Developer in Control" 
            width="128" height="128" 
            style="filter: drop-shadow(0 4px 12px rgba(0,0,0,0.15));">
     </div>
     ```

3. **Atualizar versões traduzidas** (30 min)
   - [ ] Regenerar locales: `node docs/generate-locales.js`
   - [ ] Verificar ícone em todas as 7 línguas

4. **Testar localmente** (5 min)
   - [ ] `cd docs && python3 -m http.server 8000`
   - [ ] Abrir http://localhost:8000
   - [ ] Verificar ícone aparece corretamente

**Critérios de Sucesso**:
- ✅ Ícone aparece na homepage
- ✅ Funciona em todos os idiomas
- ✅ Aparência profissional

**Arquivos a Modificar**:
- `docs/index.html`
- Regenerar: `docs/pt-br/index.html`, `docs/es/index.html`, etc.

**Dependências**: Nenhuma

---

### ✅ Ação 2.3: Criar Página "Why DDAP?" no Site

**Prioridade**: 🟡 **IMPORTANTE**  
**Tempo Estimado**: 2-3 horas (revisado de 2h)  
**Status**: [ ] Não iniciado

**Objetivo**: Transformar seção do README em página dedicada no site

**Passos**:

1. **Criar arquivo markdown** (30 min)
   - [ ] Criar `docs/why-ddap.md`
   - [ ] Copiar conteúdo da seção "Why DDAP?" do README
   - [ ] Adaptar formato para página standalone
   - [ ] Adicionar introdução e conclusão

2. **Gerar HTML** (5 min)
   - [ ] Executar: `./docs/generate-doc-pages.sh`
   - [ ] Verificar `docs/why-ddap.html` criado

3. **Adicionar ao menu de navegação** (15 min)
   - [ ] Editar `docs/index.html` (menu)
   - [ ] Adicionar link para "Why DDAP?"
   - [ ] Posicionar como item destacado

4. **Traduzir para outros idiomas** (1 hora)
   - [ ] Adicionar traduções em `docs/index-translations.json`
   - [ ] Chaves: `why_ddap_title`, `why_ddap_desc`
   - [ ] Regenerar: `node docs/generate-locales.js`

5. **Testar navegação** (10 min)
   - [ ] Verificar links funcionam
   - [ ] Testar em todos os idiomas

**Critérios de Sucesso**:
- ✅ Página "Why DDAP?" acessível
- ✅ Conteúdo completo e bem formatado
- ✅ Disponível em todos os idiomas
- ✅ Link proeminente na navegação

**Arquivos a Criar/Modificar**:
- `docs/why-ddap.md` (novo)
- `docs/index.html`
- `docs/index-translations.json`
- Executar scripts de geração

**Dependências**: Nenhuma

---

### ✅ Ação 2.4: Publicar Website no GitHub Pages

**Prioridade**: 🟡 **IMPORTANTE**  
**Tempo Estimado**: 1-2 horas (revisado de 30min)  
**Status**: [ ] Não iniciado

**Objetivo**: Tornar documentação acessível em https://schivei.github.io/ddap

**Passos**:

1. **Verificar GitHub Actions** (10 min)
   - [ ] Abrir `.github/workflows/docs.yml`
   - [ ] Verificar se workflow existe e está correto
   - [ ] Confirmar trigger em push para main

2. **Fazer merge do PR** (5 min)
   - [ ] Revisar todas as mudanças
   - [ ] Aprovar PR
   - [ ] Merge para main

3. **Verificar deployment** (10 min)
   - [ ] Acompanhar GitHub Actions
   - [ ] Aguardar conclusão do workflow
   - [ ] Verificar logs para erros

4. **Testar site publicado** (5 min)
   - [ ] Abrir https://schivei.github.io/ddap
   - [ ] Testar todos os idiomas
   - [ ] Verificar todos os links
   - [ ] Testar tema claro/escuro
   - [ ] Testar navegação

**Critérios de Sucesso**:
- ✅ Site acessível publicamente
- ✅ Todos os idiomas funcionam
- ✅ Redirect para elton.schivei.nom.br configurado (se aplicável)
- ✅ Sem erros 404

**Arquivos Envolvidos**:
- `.github/workflows/docs.yml`
- Todos os arquivos em `docs/`

**Dependências**: Deve ser feito após Ações 2.1, 2.2, 2.3

---

## Fase 3: Testes Automatizados 🟢

Prevenir regressões futuras.

### ✅ Ação 3.1: Adicionar Testes Automatizados de Template

**Prioridade**: 🟢 **MELHORIA**  
**Tempo Estimado**: 8-12 horas (revisado de 4h)  
**Status**: [ ] Não iniciado

**Objetivo**: Garantir que template sempre gera projetos corretos

**Passos**:

1. **Criar script de teste** (2 horas)
   - [ ] Criar `tests/template-validation.sh`
   - [ ] Testar todas as combinações de parâmetros
   - [ ] Validar que pacotes corretos estão incluídos
   - [ ] Validar que projeto compila

2. **Adicionar ao CI** (1 hora)
   - [ ] Editar `.github/workflows/build.yml`
   - [ ] Adicionar step de teste de template
   - [ ] Executar em PRs

3. **Documentar testes** (30 min)
   - [ ] Adicionar README em `tests/`
   - [ ] Explicar como executar localmente

4. **Executar e validar** (30 min)
   - [ ] Rodar testes localmente
   - [ ] Verificar que todos passam
   - [ ] Corrigir falhas se houver

**Critérios de Sucesso**:
- ✅ Testes cobrem 64+ cenários
- ✅ CI bloqueia PRs com templates quebrados
- ✅ Fácil de executar localmente

**Arquivos a Criar**:
- `tests/template-validation.sh`
- `.github/workflows/template-tests.yml`

**Dependências**: Deve ser feito após Ação 1.1 estar completa

---

## Fase 4: Melhorias de Arquitetura 🟢

Refinamentos de longo prazo.

### ✅ Ação 4.1: Refatorar Estrutura de Pacotes (Opcional)

**Prioridade**: 🟢 **MELHORIA**  
**Tempo Estimado**: 1-2 semanas  
**Status**: [ ] Não iniciado

**Objetivo**: Alinhar arquitetura de pacotes com filosofia

**Decisão**: Este item requer discussão arquitetural mais profunda.

**Opções**:
- A) Manter pacotes base apenas (Ddap.Data.Dapper genérico)
- B) Criar pacotes database-específicos (Ddap.Data.Dapper.SqlServer, etc.)

**Recomendação**: Discutir com time antes de implementar. Criar RFC/ADR.

**Arquivos Envolvidos**: Vários, mudança arquitetural significativa

**Dependências**: Todas as ações críticas devem estar completas

---

## Fase 5: Roadmap Estratégico 🟢

Implementação do plano de longo prazo.

### ✅ Ação 5.1: Iniciar LINQ Support - Fase 1

**Prioridade**: 🟢 **MELHORIA**  
**Tempo Estimado**: 3-4 meses  
**Status**: [ ] Não iniciado

**Objetivo**: Implementar query expression trees para clientes .NET

**Passos**: Ver STRATEGIC_ROADMAP.md para detalhes completos

**Dependências**: Todas as correções críticas devem estar completas

---

### ✅ Ação 5.2: Iniciar Cliente TypeScript

**Prioridade**: 🟢 **MELHORIA**  
**Tempo Estimado**: 4-5 meses  
**Status**: [ ] Não iniciado

**Objetivo**: Criar cliente TypeScript/JavaScript

**Passos**: Ver STRATEGIC_ROADMAP.md para detalhes completos

**Dependências**: Ação 5.1 pode ser em paralelo

---

## 📊 Resumo de Prioridades

### Ordem de Execução Recomendada

**Sprint 1 - Ação 1.1 APENAS** (recomendado):
1. ✅ Ação 1.1: Corrigir template API provider flags (4-6h)
   - **PR separado**, mais fácil de revisar
   - Validar antes de continuar

**Sprint 2 - Pacotes e Filosofia**:
2. ✅ Ação 1.2: Resolver pacotes inexistentes (4-8h)
3. ✅ Ação 1.3: Remover Pomelo forçado (2-3h)
   - **PR separado**, mudanças relacionadas

**Sprint 3 - Documentação**:
4. ✅ Ação 2.1: Adicionar avisos sobre issues (1-2h)
5. ✅ Ação 2.2: Integrar ícone no site (1-2h)
6. ✅ Ação 2.3: Criar página "Why DDAP?" (2-3h)
7. ✅ Ação 2.4: Publicar website (1-2h)
   - **PR separado**, pode ser mais rápido

**Sprint 4 - Testes**:
8. ✅ Ação 3.1: Testes automatizados de template (8-12h)
   - **PR separado**, investimento importante

**Futuro - Melhorias**:
9. ✅ Ação 4.1: Refatorar pacotes (discussão + implementação)
10. ✅ Ação 5.1: LINQ Support Fase 1 (3-4 meses)
11. ✅ Ação 5.2: Cliente TypeScript (4-5 meses)

### Tempo Total Estimado (REVISADO)

| Fase | Tempo Original | Tempo Revisado | Diferença |
|------|----------------|----------------|-----------|
| Fase 1 - Críticos | 5-9h | 12-20h | +7-11h |
| Fase 2 - Documentação | 4h | 5-7h | +1-3h |
| Fase 3 - Testes | 4h | 8-12h | +4-8h |
| **Total Sprint 1-4** | **13-17h** | **25-39h** | **+12-22h** |
| | **(2-3 dias)** | **(5-7 dias)** | **(1-2 semanas)** |
| Fase 4 - Arquitetura | 1-2 semanas | 1-2 semanas | - |
| Fase 5 - Roadmap | 7-9 meses | 7-9 meses | - |

**Nota**: Estimativas revisadas consideram complexidade real, debugging, testes extensivos, e buffer para imprevistos (~20-30%).

---

## 🎯 Como Usar Este Roteiro

### Processo de Trabalho

1. **Escolha o próximo item não iniciado**
   - Sempre seguir ordem de prioridade
   - Marcar como "Em andamento"

2. **Execute todos os passos do item**
   - Não pule etapas
   - Marque cada sub-item ao completar

3. **Valide criteriosamente**
   - Execute todos os testes
   - Verifique critérios de sucesso
   - Peça revisão se necessário

4. **Marque como completo**
   - [ ] → [x]
   - Faça commit: `git commit -m "Complete Ação X.Y: [descrição]"`

5. **Passe para o próximo item**
   - Não inicie múltiplos itens ao mesmo tempo
   - Mantenha foco

### Marcação de Status

Use emojis para indicar status:
- [ ] Não iniciado
- [🔄] Em andamento
- [✅] Completo
- [❌] Bloqueado (descrever motivo)

### Exemplo de Uso

```markdown
### ✅ Ação 1.1: Corrigir Template - API Provider Flags
**Status**: [🔄] Em andamento

**Passos de Execução**:
1. Analisar o problema
   - [✅] Abrir template.json
   - [✅] Localizar seção IncludeRest
   - [🔄] Identificar expressão problemática
```

---

## 📝 Notas de Progresso

### Registro de Mudanças

**2026-01-30**: Documento criado com todas as ações identificadas

*Adicione entradas aqui conforme completa ações:*

**Exemplo**:
- **2026-02-01**: [✅] Ação 1.1 completa - Template corrigido, testes passando
- **2026-02-02**: [✅] Ação 1.3 completa - Pomelo removido, filosofia restaurada

---

## 🆘 Resolução de Problemas

### Se Encontrar Bloqueios

1. **Documente o bloqueio** neste documento
2. **Marque ação como bloqueada**: [❌]
3. **Descreva o problema** em detalhe
4. **Identifique alternativas** se possível
5. **Peça ajuda** no GitHub Issues ou discussões

### Se Precisar Mudar Prioridade

1. **Justifique a mudança** neste documento
2. **Atualize ordem de execução**
3. **Comunique ao time**

---

## 📚 Documentos de Referência

Para detalhes técnicos completos, consulte:

- **TESTING_FINDINGS.md**: Bugs identificados inicialmente
- **TEMPLATE_TESTING_DETAILED.md**: Análise detalhada de 64+ cenários
- **PHILOSOPHY_COMPLIANCE_ANALYSIS.md**: Violações de filosofia
- **PACKAGE_INVENTORY_ANALYSIS.md**: Análise de pacotes
- **STRATEGIC_ROADMAP.md**: Plano de longo prazo
- **FINAL_COMPREHENSIVE_REPORT.md**: Resumo executivo completo

---

## ✅ Checklist Geral de Conclusão

Quando todas as ações críticas estiverem completas:

- [ ] Template gera projetos funcionais
- [ ] Nenhuma dependência forçada
- [ ] Website publicado com todas as melhorias
- [ ] Testes automatizados implementados
- [ ] Documentação atualizada
- [ ] Issues conhecidos resolvidos
- [ ] Versão 1.0.3+ publicada no NuGet

**Quando este checklist estiver completo, o projeto DDAP estará em estado estável e pronto para crescimento! 🎉**

---

**Mantenha este documento atualizado. É seu guia para não perder o foco! 📍**
