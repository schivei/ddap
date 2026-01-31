# 🗺️ Guia Consolidado: Sprints Sequenciais do Épico DDAP

**Versão**: 1.0  
**Última Atualização**: 2026-01-31  
**Objetivo**: Guia completo para execução sequencial de todas as sprints do épico

---

## 📊 Visão Geral do Épico

### Status Atual

| Sprint | Status | Branch | PR | Tempo | Próxima Ação |
|--------|--------|--------|----|-------|--------------|
| Epic Base | ✅ Completo | copilot/improve-ddap-project | Este | 13-15h | Manter como base |
| Sprint 1 | ✅ Completo | feat/fix-template-flags | #24 | 2h | ✅ Merged |
| Sprint 2 | ✅ Completo | feat/resolve-packages | #25 | 4h | ✅ Merged |
| Sprint 3 | ✅ Completo | docs-site updates | Incluído | 6h | ✅ Completo (icon, why-ddap, known-issues, multilang) |
| **Sprint 4** | 📋 **Próximo** | - | - | 8-12h | **Template tests - Ver SPRINT4_PR_INSTRUCTIONS.md** |

**Progresso**: 3/4 sprints completos (75%)  
**Tempo Investido**: 25-27 horas  
**Tempo Restante**: 8-12 horas (apenas Sprint 4)

---

## 🎯 Sprint 3: Update Documentation Site

**Tempo Estimado**: 5-9 horas  
**Quando Iniciar**: Após Sprint 2 merged para epic branch  
**Branch**: `feat/update-docs-site`  
**Base**: `copilot/improve-ddap-project` (com Sprint 2 incluído)

### Objetivos Principais

1. ✅ Adicionar avisos sobre issues conhecidos
2. ✅ Integrar ícone profissional no site
3. ✅ Criar página dedicada "Why DDAP?"
4. ✅ Atualizar documentação de database providers
5. ✅ Publicar website em GitHub Pages

---

### Ação 3.1: Avisos sobre Issues Conhecidos (1-2h)

**Objetivo**: Informar usuários sobre bugs conhecidos enquanto correções não estão em produção.

#### Checklist Detalhado

- [ ] **Editar README.md**
  - [ ] Adicionar banner de aviso após título principal
  - [ ] Texto: "⚠️ **Aviso Importante**: Há issues conhecidos no template 1.0.2. [Ver detalhes](https://github.com/schivei/ddap/issues). Correções em andamento na versão 1.0.3."
  - [ ] Adicionar link para issues do GitHub
  - [ ] Manter formatação consistente

- [ ] **Criar docs/known-issues.md**
  - [ ] Seção: "Bugs Conhecidos no Template"
  - [ ] Bug 1: Template API flags não funcionam (até 1.0.2)
    - Descrição do problema
    - Versões afetadas
    - Workaround temporário
    - Status da correção
  - [ ] Bug 2: Pacotes inexistentes no template (até 1.0.2)
    - Quais pacotes
    - Impacto
    - Como resolver manualmente
  - [ ] Seção: "Versões e Status"
    - Tabela com versões
    - O que está fixado em cada versão
  - [ ] Seção: "Como Reportar Issues"
    - Link para GitHub Issues
    - Template de report

- [ ] **Gerar página HTML**
  ```bash
  cd docs
  ./generate-doc-pages.sh
  ```
  - [ ] Verificar que known-issues.html foi gerado
  - [ ] Validar formatação e links

- [ ] **Adicionar ao menu de navegação**
  - [ ] Editar docs/index.html
  - [ ] Adicionar item "Known Issues" no menu
  - [ ] Traduzir título para os 6 idiomas (docs/index-translations.json)
  - [ ] Regenerar locales: `node docs/generate-locales.js`

- [ ] **Validar em todos os idiomas**
  - [ ] Testar en (inglês)
  - [ ] Testar pt-br (português)
  - [ ] Testar es (espanhol)
  - [ ] Testar fr (francês)
  - [ ] Testar de (alemão)
  - [ ] Testar ja (japonês)
  - [ ] Testar zh (chinês)

**Critérios de Sucesso**:
- ✅ Banner visível no README
- ✅ Página known-issues acessível no site
- ✅ Disponível em 7 idiomas
- ✅ Links funcionando

**Arquivos Modificados**:
- `README.md`
- `docs/known-issues.md` (novo)
- `docs/known-issues.html` (gerado)
- `docs/index.html`
- `docs/index-translations.json`
- `docs/pt-br/known-issues.html`, etc. (gerados)

---

### Ação 3.2: Integrar Ícone no Site (1-2h)

**Objetivo**: Adicionar identidade visual ao site com o ícone profissional criado.

#### Checklist Detalhado

- [ ] **Copiar ícone para docs**
  ```bash
  cp icons/icon.svg docs/icon.svg
  ```
  - [ ] Verificar que arquivo foi copiado
  - [ ] Validar que é o SVG correto

- [ ] **Editar homepage (docs/index.html)**
  - [ ] Localizar seção hero/header
  - [ ] Adicionar elemento `<img>` ou `<object>` com o ícone
  - [ ] Posicionamento: Acima ou ao lado do título "DDAP"
  - [ ] Tamanho: 64px ou 128px (testar o que fica melhor)
  - [ ] Alt text: "DDAP - Developer in Control"
  - [ ] CSS: Centralizar e estilizar
    ```html
    <div class="hero-icon">
      <img src="icon.svg" alt="DDAP - Developer in Control" width="128" height="128">
    </div>
    ```

- [ ] **Ajustar CSS**
  - [ ] Adicionar estilos para `.hero-icon`
  - [ ] Margin/padding adequados
  - [ ] Centralização
  - [ ] Responsivo (tamanho menor em mobile)

- [ ] **Adicionar favicon**
  - [ ] Gerar favicon.ico do SVG (se possível)
  - [ ] OU documentar como gerar
  - [ ] Adicionar `<link rel="icon">` no `<head>`

- [ ] **Regenerar versões traduzidas**
  ```bash
  cd docs
  node generate-locales.js
  ```
  - [ ] Verificar que todas as 6 traduções foram atualizadas
  - [ ] Validar que ícone aparece em todas

- [ ] **Testar localmente**
  ```bash
  cd docs
  python3 -m http.server 8000
  # Abrir http://localhost:8000
  ```
  - [ ] Verificar ícone no inglês
  - [ ] Verificar em pt-br
  - [ ] Verificar em outros idiomas
  - [ ] Testar responsive (mobile)

- [ ] **Validar qualidade visual**
  - [ ] Ícone renderiza corretamente
  - [ ] Sem pixelização
  - [ ] Cores corretas
  - [ ] Alinhamento perfeito

**Critérios de Sucesso**:
- ✅ Ícone visível na homepage
- ✅ Funciona em todos os 7 idiomas
- ✅ Responsive (mobile e desktop)
- ✅ Qualidade visual perfeita

**Arquivos Modificados**:
- `docs/icon.svg` (novo)
- `docs/index.html`
- `docs/pt-br/index.html`, etc. (regenerados)

---

### Ação 3.3: Criar Página "Why DDAP?" (2-3h)

**Objetivo**: Página dedicada explicando por que usar DDAP, baseada na seção do README.

#### Checklist Detalhado

- [ ] **Criar docs/why-ddap.md**
  - [ ] Copiar conteúdo da seção "Why DDAP?" do README
  - [ ] Expandir com mais detalhes se necessário
  - [ ] Adicionar seções:
    - Por que DDAP existe?
    - Problemas que resolve
    - Developer in Control
    - Filosofia de dependências mínimas
    - Abstração resiliente
    - Quando usar (e quando não usar)
  - [ ] Adicionar exemplos de código
  - [ ] Adicionar comparações visuais
  - [ ] Call-to-action no final (link para Get Started)

- [ ] **Gerar página HTML**
  ```bash
  cd docs
  ./generate-doc-pages.sh
  ```
  - [ ] Verificar why-ddap.html gerado
  - [ ] Validar formatação

- [ ] **Adicionar traduções**
  - [ ] Editar `docs/index-translations.json`
  - [ ] Adicionar traduções do título "Why DDAP?" para:
    - pt-br: "Por Que DDAP?"
    - es: "¿Por Qué DDAP?"
    - fr: "Pourquoi DDAP?"
    - de: "Warum DDAP?"
    - ja: "なぜDDAPか？"
    - zh: "为什么选择DDAP？"
  - [ ] Salvar arquivo

- [ ] **Adicionar ao menu de navegação**
  - [ ] Editar docs/index.html
  - [ ] Adicionar item "Why DDAP?" no menu principal
  - [ ] Posição: Logo após "Home" ou "Get Started"
  - [ ] Link para /why-ddap.html
  - [ ] Destaque visual (opcional: bold ou cor diferente)

- [ ] **Regenerar todas as versões**
  ```bash
  cd docs
  node generate-locales.js
  ```
  - [ ] Verificar 7 versões geradas
  - [ ] Validar links funcionam

- [ ] **Testar navegação**
  ```bash
  cd docs
  python3 -m http.server 8000
  ```
  - [ ] Clicar em "Why DDAP?" no menu
  - [ ] Verificar página carrega
  - [ ] Testar em todos os idiomas
  - [ ] Validar links internos (voltar, etc.)

- [ ] **Melhorias visuais (opcional)**
  - [ ] Adicionar ícones nas seções
  - [ ] Boxes destacados para pontos importantes
  - [ ] Exemplos de código com syntax highlighting
  - [ ] Gráficos/diagramas se apropriado

**Critérios de Sucesso**:
- ✅ Página why-ddap.html existe e está completa
- ✅ Disponível em 7 idiomas
- ✅ Link proeminente no menu
- ✅ Conteúdo convincente e claro
- ✅ Navegação funcional

**Arquivos Modificados**:
- `docs/why-ddap.md` (novo)
- `docs/why-ddap.html` (gerado)
- `docs/index.html`
- `docs/index-translations.json`
- `docs/pt-br/why-ddap.html`, etc. (gerados)

---

### Ação 3.4: Publicar Website (1-2h)

**Objetivo**: Fazer deploy do site atualizado para GitHub Pages.

#### Checklist Detalhado

- [ ] **Verificar configuração GitHub Actions**
  - [ ] Arquivo `.github/workflows/docs.yml` existe?
  - [ ] Se NÃO existe, criar workflow:
    ```yaml
    name: Deploy Documentation
    on:
      push:
        branches: [main]
        paths: ['docs/**']
    jobs:
      deploy:
        runs-on: ubuntu-latest
        steps:
          - uses: actions/checkout@v3
          - name: Deploy to GitHub Pages
            uses: peaceiris/actions-gh-pages@v3
            with:
              github_token: ${{ secrets.GITHUB_TOKEN }}
              publish_dir: ./docs
    ```
  - [ ] Verificar que workflow está correto

- [ ] **Configurar GitHub Pages**
  - [ ] Ir para Settings → Pages no GitHub
  - [ ] Source: Deploy from a branch
  - [ ] Branch: gh-pages (será criado pelo Actions)
  - [ ] Folder: / (root)
  - [ ] Salvar

- [ ] **Validar arquivos antes do deploy**
  - [ ] Todos os HTMLs gerados
  - [ ] Ícone copiado
  - [ ] Known issues criado
  - [ ] Why DDAP criado
  - [ ] Traduções regeneradas

- [ ] **Merge para main (ou deploy direto)**
  - [ ] Opção A: Merge do epic inteiro
  - [ ] Opção B: Cherry-pick apenas docs
  - [ ] Fazer push

- [ ] **Acompanhar GitHub Actions**
  - [ ] Ir para Actions tab no GitHub
  - [ ] Verificar workflow "Deploy Documentation" rodando
  - [ ] Esperar conclusão
  - [ ] Verificar logs se houver erro

- [ ] **Testar site publicado**
  - [ ] Acessar https://schivei.github.io/ddap
  - [ ] OU https://elton.schivei.nom.br/ddap (se redirect configurado)
  - [ ] Verificar homepage carrega
  - [ ] Verificar ícone aparece
  - [ ] Clicar "Known Issues"
  - [ ] Clicar "Why DDAP?"
  - [ ] Testar navegação

- [ ] **Validar todos os idiomas**
  - [ ] https://schivei.github.io/ddap (en)
  - [ ] https://schivei.github.io/ddap/pt-br/ (pt-br)
  - [ ] https://schivei.github.io/ddap/es/ (es)
  - [ ] https://schivei.github.io/ddap/fr/ (fr)
  - [ ] https://schivei.github.io/ddap/de/ (de)
  - [ ] https://schivei.github.io/ddap/ja/ (ja)
  - [ ] https://schivei.github.io/ddap/zh/ (zh)

- [ ] **Validar links**
  - [ ] Links internos funcionam
  - [ ] Links para GitHub funcionam
  - [ ] Links de navegação funcionam
  - [ ] Sem erros 404

- [ ] **Testar responsividade**
  - [ ] Desktop (1920x1080)
  - [ ] Tablet (768px)
  - [ ] Mobile (375px)
  - [ ] Ícone ajusta tamanho
  - [ ] Menu funciona em mobile

**Critérios de Sucesso**:
- ✅ Site acessível em https://schivei.github.io/ddap
- ✅ Todos os 7 idiomas funcionam
- ✅ Ícone visível
- ✅ Known Issues acessível
- ✅ Why DDAP acessível
- ✅ Sem erros 404
- ✅ Responsivo

**Arquivos Verificados**:
- `.github/workflows/docs.yml` (se necessário)
- Todos em `docs/`

---

### Comandos Git para Sprint 3

#### Criar Branch Sprint 3
```bash
# Certifique-se que está na epic branch com Sprint 2 merged
git checkout copilot/improve-ddap-project
git pull origin copilot/improve-ddap-project

# Criar nova branch para Sprint 3
git checkout -b feat/update-docs-site
```

#### Durante Desenvolvimento
```bash
# Commits incrementais
git add docs/known-issues.md README.md
git commit -m "Add known issues warnings to README and docs"

git add docs/index.html docs/icon.svg
git commit -m "Integrate professional icon into documentation site"

git add docs/why-ddap.md docs/index-translations.json
git commit -m "Add dedicated Why DDAP page with translations"

git add .github/workflows/docs.yml
git commit -m "Setup GitHub Pages deployment workflow"
```

#### Push e Criar PR
```bash
# Push da branch
git push -u origin feat/update-docs-site

# Criar PR no GitHub:
# Base: copilot/improve-ddap-project
# Compare: feat/update-docs-site
# Title: "Sprint 3: Update Documentation Site"
```

#### Após Aprovação
```bash
# Merge para epic branch
git checkout copilot/improve-ddap-project
git merge feat/update-docs-site --no-ff
git push origin copilot/improve-ddap-project
```

---

## 🧪 Sprint 4: Add Template Tests

**Tempo Estimado**: 8-12 horas  
**Quando Iniciar**: Após Sprint 3 merged para epic branch  
**Branch**: `feat/add-template-tests`  
**Base**: `copilot/improve-ddap-project` (com Sprint 3 incluído)

### Objetivos Principais

1. ✅ Criar script de validação de templates
2. ✅ Implementar 64+ cenários de teste
3. ✅ Validar pacotes corretos são incluídos
4. ✅ Validar que projetos compilam
5. ✅ Integrar testes no CI
6. ✅ Documentar processo de teste

---

### Fase 4.1: Análise e Planejamento (1h)

#### Checklist

- [ ] **Revisar cenários de teste**
  - [ ] Ler TEMPLATE_TESTING_DETAILED.md
  - [ ] Listar 64+ cenários a cobrir
  - [ ] Priorizar cenários críticos

- [ ] **Definir estrutura de testes**
  - [ ] Decidir linguagem (Bash, PowerShell, ou C#)
  - [ ] Estrutura de diretórios
  - [ ] Formato de output (logs, JUnit XML, etc.)

- [ ] **Preparar ambiente**
  - [ ] Verificar dotnet CLI disponível
  - [ ] Instalar template localmente
  - [ ] Testar geração manual

**Arquivos a Criar**:
- `tests/README.md` (documentação)
- `tests/template-validation/` (diretório)

---

### Fase 4.2: Criar Script Base (2-3h)

#### Checklist

- [ ] **Criar tests/template-validation/validate-template.sh**
  - [ ] Shebang: `#!/bin/bash`
  - [ ] Configurar `set -euo pipefail`
  - [ ] Funções helper:
    - `setup_test_env()` - criar diretório temporário
    - `cleanup()` - limpar após testes
    - `test_template_generation()` - gerar projeto
    - `verify_packages()` - validar .csproj
    - `verify_build()` - tentar compilar
    - `log_result()` - registrar sucesso/falha
  - [ ] Variáveis:
    - `TEST_DIR` - diretório temporário
    - `PASSED` - contador sucessos
    - `FAILED` - contador falhas
    - `TOTAL` - total de testes

- [ ] **Implementar geração de projeto**
  ```bash
  test_template_generation() {
    local name=$1
    local db_provider=$2
    local db_type=$3
    local rest=$4
    local graphql=$5
    
    dotnet new ddap-api \
      --name "$name" \
      --database-provider "$db_provider" \
      --database-type "$db_type" \
      --rest "$rest" \
      --graphql "$graphql" \
      --output "$TEST_DIR/$name"
  }
  ```

- [ ] **Implementar verificação de pacotes**
  ```bash
  verify_packages() {
    local project=$1
    local expected_packages=("${@:2}")
    
    for package in "${expected_packages[@]}"; do
      if ! grep -q "$package" "$TEST_DIR/$project/$project.csproj"; then
        return 1
      fi
    done
    return 0
  }
  ```

- [ ] **Implementar verificação de build**
  ```bash
  verify_build() {
    local project=$1
    cd "$TEST_DIR/$project"
    dotnet build --no-restore > /dev/null 2>&1
    local result=$?
    cd - > /dev/null
    return $result
  }
  ```

- [ ] **Implementar logging**
  ```bash
  log_result() {
    local test_name=$1
    local status=$2
    local message=$3
    
    if [ "$status" -eq 0 ]; then
      echo "✅ PASS: $test_name"
      ((PASSED++))
    else
      echo "❌ FAIL: $test_name - $message"
      ((FAILED++))
    fi
    ((TOTAL++))
  }
  ```

- [ ] **Tornar executável**
  ```bash
  chmod +x tests/template-validation/validate-template.sh
  ```

**Arquivos Criados**:
- `tests/template-validation/validate-template.sh`

---

### Fase 4.3: Implementar Cenários de Teste (3-4h)

#### Cenários Críticos (Mínimo)

- [ ] **SQL Server + Dapper**
  ```bash
  test_template_generation "TestSqlServerDapper" "dapper" "sqlserver" "false" "false"
  verify_packages "TestSqlServerDapper" "Ddap.Data.Dapper" "Microsoft.Data.SqlClient"
  verify_build "TestSqlServerDapper"
  log_result "SQL Server + Dapper" $? ""
  ```

- [ ] **SQL Server + Entity Framework**
  ```bash
  test_template_generation "TestSqlServerEF" "entityframework" "sqlserver" "false" "false"
  verify_packages "TestSqlServerEF" "Ddap.Data.EntityFramework" "Microsoft.EntityFrameworkCore.SqlServer"
  verify_build "TestSqlServerEF"
  log_result "SQL Server + EF" $? ""
  ```

- [ ] **MySQL + Dapper**
  ```bash
  test_template_generation "TestMySqlDapper" "dapper" "mysql" "false" "false"
  verify_packages "TestMySqlDapper" "Ddap.Data.Dapper" "MySqlConnector"
  verify_build "TestMySqlDapper"
  log_result "MySQL + Dapper" $? ""
  ```

- [ ] **MySQL + Entity Framework**
  ```bash
  test_template_generation "TestMySqlEF" "entityframework" "mysql" "false" "false"
  verify_packages "TestMySqlEF" "Ddap.Data.EntityFramework" "MySql.EntityFrameworkCore"
  verify_build "TestMySqlEF"
  log_result "MySQL + EF" $? ""
  ```

- [ ] **PostgreSQL + Dapper**
  ```bash
  test_template_generation "TestPostgreSqlDapper" "dapper" "postgresql" "false" "false"
  verify_packages "TestPostgreSqlDapper" "Ddap.Data.Dapper" "Npgsql"
  verify_build "TestPostgreSqlDapper"
  log_result "PostgreSQL + Dapper" $? ""
  ```

- [ ] **PostgreSQL + Entity Framework**
  ```bash
  test_template_generation "TestPostgreSqlEF" "entityframework" "postgresql" "false" "false"
  verify_packages "TestPostgreSqlEF" "Ddap.Data.EntityFramework" "Npgsql.EntityFrameworkCore.PostgreSQL"
  verify_build "TestPostgreSqlEF"
  log_result "PostgreSQL + EF" $? ""
  ```

- [ ] **SQLite + Dapper**
  ```bash
  test_template_generation "TestSqliteDapper" "dapper" "sqlite" "false" "false"
  verify_packages "TestSqliteDapper" "Ddap.Data.Dapper" "Microsoft.Data.Sqlite"
  verify_build "TestSqliteDapper"
  log_result "SQLite + Dapper" $? ""
  ```

- [ ] **SQLite + Entity Framework**
  ```bash
  test_template_generation "TestSqliteEF" "entityframework" "sqlite" "false" "false"
  verify_packages "TestSqliteEF" "Ddap.Data.EntityFramework" "Microsoft.EntityFrameworkCore.Sqlite"
  verify_build "TestSqliteEF"
  log_result "SQLite + EF" $? ""
  ```

#### Cenários com API Providers

- [ ] **SQL Server + REST**
  ```bash
  test_template_generation "TestRest" "dapper" "sqlserver" "true" "false"
  verify_packages "TestRest" "Ddap.Rest"
  verify_build "TestRest"
  log_result "REST API Provider" $? ""
  ```

- [ ] **SQL Server + GraphQL**
  ```bash
  test_template_generation "TestGraphQL" "dapper" "sqlserver" "false" "true"
  verify_packages "TestGraphQL" "Ddap.GraphQL"
  verify_build "TestGraphQL"
  log_result "GraphQL API Provider" $? ""
  ```

- [ ] **SQL Server + REST + GraphQL**
  ```bash
  test_template_generation "TestRestGraphQL" "dapper" "sqlserver" "true" "true"
  verify_packages "TestRestGraphQL" "Ddap.Rest" "Ddap.GraphQL"
  verify_build "TestRestGraphQL"
  log_result "REST + GraphQL" $? ""
  ```

#### Adicionar Mais Cenários

- [ ] Implementar 50+ cenários adicionais
- [ ] Cobrir todas as combinações importantes
- [ ] Edge cases (todos os flags habilitados, etc.)

**Total**: 64+ cenários

---

### Fase 4.4: Integração com CI (2h)

#### Checklist

- [ ] **Criar/Editar .github/workflows/build.yml**
  - [ ] Adicionar job "validate-templates"
  - [ ] Executar após build
  - [ ] Configurar:
    ```yaml
    validate-templates:
      name: Validate Templates
      runs-on: ubuntu-latest
      needs: build
      steps:
        - uses: actions/checkout@v3
        
        - name: Setup .NET
          uses: actions/setup-dotnet@v3
          with:
            dotnet-version: '8.0.x'
        
        - name: Install Template
          run: dotnet new install ./templates/ddap-api
        
        - name: Run Template Tests
          run: |
            chmod +x tests/template-validation/validate-template.sh
            ./tests/template-validation/validate-template.sh
        
        - name: Upload Test Results
          if: always()
          uses: actions/upload-artifact@v3
          with:
            name: template-test-results
            path: tests/template-validation/results/
    ```

- [ ] **Testar workflow localmente (se possível)**
  - [ ] Usar act ou similar
  - [ ] OU fazer PR draft para testar

- [ ] **Configurar para bloquear merge**
  - [ ] Settings → Branches → main
  - [ ] Require status checks: "Validate Templates"

**Arquivos Modificados**:
- `.github/workflows/build.yml`

---

### Fase 4.5: Documentação (1h)

#### Checklist

- [ ] **Criar tests/README.md**
  - [ ] Seção: "Template Validation Tests"
  - [ ] Como executar localmente
  - [ ] Como adicionar novos testes
  - [ ] Como interpretar resultados
  - [ ] Troubleshooting comum

- [ ] **Documentar no README.md principal**
  - [ ] Adicionar badge de status dos testes
  - [ ] Link para tests/README.md
  - [ ] Menção que templates são testados

- [ ] **Adicionar comentários no script**
  - [ ] Explicar cada função
  - [ ] Documentar variáveis importantes
  - [ ] Exemplos de uso

**Arquivos Modificados**:
- `tests/README.md` (novo)
- `README.md`
- `tests/template-validation/validate-template.sh`

---

### Fase 4.6: Testes e Validação (1-2h)

#### Checklist

- [ ] **Executar script localmente**
  ```bash
  cd tests/template-validation
  ./validate-template.sh
  ```
  - [ ] Verificar todos os testes passam
  - [ ] Verificar output é claro
  - [ ] Verificar tempo de execução aceitável

- [ ] **Testar no CI**
  - [ ] Criar PR draft
  - [ ] Verificar workflow executa
  - [ ] Verificar resultados são reportados
  - [ ] Ajustar se necessário

- [ ] **Validar cobertura**
  - [ ] 64+ cenários implementados
  - [ ] Todos os bancos cobertos
  - [ ] Todos os API providers cobertos
  - [ ] Combinações importantes cobertas

- [ ] **Performance**
  - [ ] Script completa em tempo razoável (<15 min)
  - [ ] Paralelização se necessário
  - [ ] Cache de builds se apropriado

**Critérios de Sucesso**:
- ✅ Script executa sem erros
- ✅ 64+ cenários testados
- ✅ Taxa de sucesso 100% (após Sprint 1 e 2)
- ✅ CI integrado e funcionando
- ✅ Documentação completa

---

### Comandos Git para Sprint 4

#### Criar Branch Sprint 4
```bash
# Certifique-se que está na epic branch com Sprint 3 merged
git checkout copilot/improve-ddap-project
git pull origin copilot/improve-ddap-project

# Criar nova branch para Sprint 4
git checkout -b feat/add-template-tests
```

#### Durante Desenvolvimento
```bash
# Commits por fase
git add tests/README.md tests/template-validation/
git commit -m "Add template validation test structure"

git add tests/template-validation/validate-template.sh
git commit -m "Implement base validation script with helper functions"

git add tests/template-validation/validate-template.sh
git commit -m "Add 64+ template validation scenarios"

git add .github/workflows/build.yml
git commit -m "Integrate template tests into CI pipeline"

git add tests/README.md README.md
git commit -m "Add comprehensive testing documentation"
```

#### Push e Criar PR
```bash
# Push da branch
git push -u origin feat/add-template-tests

# Criar PR no GitHub:
# Base: copilot/improve-ddap-project
# Compare: feat/add-template-tests
# Title: "Sprint 4: Add Comprehensive Template Tests"
```

#### Após Aprovação
```bash
# Merge para epic branch
git checkout copilot/improve-ddap-project
git merge feat/add-template-tests --no-ff
git push origin copilot/improve-ddap-project
```

---

## 🔄 Workflow Completo de Execução

### Ordem Sequencial

1. **AGORA**: Sprint 2
   - [ ] Criar PR do Sprint 2
   - [ ] Review
   - [ ] Merge para `copilot/improve-ddap-project`

2. **PRÓXIMO**: Sprint 3 (após Sprint 2 merged)
   - [ ] Criar branch `feat/update-docs-site` a partir de epic
   - [ ] Implementar Ação 3.1 (Known Issues)
   - [ ] Implementar Ação 3.2 (Ícone)
   - [ ] Implementar Ação 3.3 (Why DDAP)
   - [ ] Implementar Ação 3.4 (Publicar)
   - [ ] Criar PR
   - [ ] Review
   - [ ] Merge para `copilot/improve-ddap-project`

3. **DEPOIS**: Sprint 4 (após Sprint 3 merged)
   - [ ] Criar branch `feat/add-template-tests` a partir de epic
   - [ ] Implementar Fase 4.1 (Planejamento)
   - [ ] Implementar Fase 4.2 (Script base)
   - [ ] Implementar Fase 4.3 (Cenários)
   - [ ] Implementar Fase 4.4 (CI)
   - [ ] Implementar Fase 4.5 (Docs)
   - [ ] Implementar Fase 4.6 (Validação)
   - [ ] Criar PR
   - [ ] Review
   - [ ] Merge para `copilot/improve-ddap-project`

4. **FINAL**: Merge Epic
   - [ ] Todos os 4 sprints completos e merged no epic
   - [ ] Criar PR final: `copilot/improve-ddap-project` → `main`
   - [ ] Review completo
   - [ ] **MERGE FINAL** - tudo de uma vez!
   - [ ] 🎉 Épico completo!

---

## 📋 Checklist Geral do Épico

### Sprint 1 ✅
- [x] Template API flags corrigidos
- [x] PR #24 criado
- [x] Aguardando merge

### Sprint 2 ✅
- [x] Pacotes inexistentes removidos
- [x] MySQL usa pacote oficial
- [x] Versões atualizadas
- [ ] **Criar PR** ⏳
- [ ] Aguardando merge

### Sprint 3 📋
- [ ] Known issues documentado
- [ ] Ícone integrado
- [ ] Página Why DDAP criada
- [ ] Website publicado
- [ ] PR criado
- [ ] Merged para epic

### Sprint 4 📋
- [ ] Script de validação criado
- [ ] 64+ cenários implementados
- [ ] CI integrado
- [ ] Documentação completa
- [ ] PR criado
- [ ] Merged para epic

### Merge Final 📋
- [ ] Epic → main
- [ ] Review final
- [ ] Merge completo
- [ ] 🎉 Projeto melhorado!

---

## 💡 Dicas de Execução

### Para Cada Sprint

1. **Antes de Começar**
   - ✅ Verifique que sprint anterior foi merged
   - ✅ Atualize sua epic branch: `git pull origin copilot/improve-ddap-project`
   - ✅ Crie branch nova do sprint
   - ✅ Revise checklist completo

2. **Durante Execução**
   - ✅ Commits incrementais (não um commit gigante)
   - ✅ Teste localmente antes de push
   - ✅ Marque checkboxes conforme completa
   - ✅ Documente decisões importantes

3. **Ao Finalizar**
   - ✅ Revise todos os arquivos modificados
   - ✅ Execute testes se aplicável
   - ✅ Faça push
   - ✅ Crie PR com descrição detalhada
   - ✅ Aguarde review

4. **Após Merge**
   - ✅ Atualize sua epic branch local
   - ✅ Verifique que mudanças estão incluídas
   - ✅ Inicie próximo sprint

### Troubleshooting

**Conflito ao criar branch nova**:
```bash
git fetch origin
git checkout copilot/improve-ddap-project
git reset --hard origin/copilot/improve-ddap-project
git checkout -b feat/nova-branch
```

**Esqueci de basear no epic**:
```bash
git checkout feat/minha-branch
git rebase copilot/improve-ddap-project
```

**Preciso atualizar branch durante sprint**:
```bash
git checkout feat/minha-branch
git fetch origin
git rebase origin/copilot/improve-ddap-project
```

---

## 📊 Estimativas Revisadas

| Sprint | Estimativa Original | Tempo Real Esperado | Nota |
|--------|---------------------|---------------------|------|
| Sprint 1 | 4-6h | 2h | ✅ Mais rápido |
| Sprint 2 | 6-11h | 4h | ✅ Mais rápido |
| Sprint 3 | 5-9h | 5-9h | Realista |
| Sprint 4 | 8-12h | 8-12h | Realista |
| **Total** | **23-38h** | **19-27h** | Eficiência aprendida |

**Observação**: Sprints 1 e 2 foram mais rápidos devido à experiência adquirida. Sprints 3 e 4 mantêm estimativas conservadoras.

---

## 🎯 Próximas Ações Imediatas

1. **AGORA**: 
   ```bash
   git push -u origin feat/resolve-packages
   ```
   Criar PR do Sprint 2 no GitHub

2. **AGUARDAR**: Review e merge do Sprint 2

3. **SEGUIR**: Este guia para Sprint 3
   - Seção "Sprint 3: Update Documentation Site"
   - Seguir checklist detalhado
   - Ação 3.1 → 3.2 → 3.3 → 3.4

4. **DEPOIS**: Sprint 4 seguindo mesmo processo

---

## 📚 Documentos de Referência

- **ROTEIRO_ACOES.md**: Detalhes técnicos de cada ação
- **ESTRATEGIA_EPICO.md**: Como funciona estratégia de épico
- **ANALISE_TEMPO_PRODUTIVIDADE.md**: Métricas e estimativas
- **SPRINT1_PR_INSTRUCTIONS.md**: Exemplo de PR Sprint 1
- **SPRINT2_PR_INSTRUCTIONS.md**: Exemplo de PR Sprint 2
- **SPRINT2_RESUMO_FINAL.md**: Resumo Sprint 2

---

**Versão**: 1.0  
**Status**: ✅ Pronto para uso  
**Uso**: Siga este guia sequencialmente após cada merge

🚀 **Boa sorte com as sprints!**
