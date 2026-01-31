# 🧪 Sprint 4: Add Template Tests - Guia Completo

**Tempo Estimado**: 8-12 horas  
**Status**: 📋 Pronto para Implementação  
**Branch**: `feat/add-template-tests`  
**Base**: `copilot/improve-ddap-project`

---

## 🎯 Objetivo

Criar testes automatizados abrangentes para o template `ddap-api`, validando todas as combinações de parâmetros e garantindo que projetos gerados compilam e executam corretamente.

---

## 📋 Checklist Completo

### Fase 1: Análise e Planejamento (1h)

- [ ] **Revisar template atual**
  - [ ] Analisar `templates/ddap-api/.template.config/template.json`
  - [ ] Listar todos os parâmetros disponíveis
  - [ ] Identificar todas as combinações críticas

- [ ] **Definir cenários de teste**
  - [ ] Criar matriz de teste (database × provider × API)
  - [ ] Priorizar cenários mais comuns
  - [ ] Identificar casos extremos

- [ ] **Planejar estrutura de testes**
  - [ ] Definir localização (tests/TemplateTests/)
  - [ ] Decidir framework (xUnit + FluentAssertions)
  - [ ] Planejar helpers e utilitários

---

### Fase 2: Script de Validação Base (2-3h)

- [ ] **Criar tests/validate-template.sh**
  ```bash
  #!/bin/bash
  # Template validation script
  
  set -e
  
  TEMPLATE_PATH="templates/ddap-api"
  TEMP_DIR=$(mktemp -d)
  
  function cleanup {
    rm -rf "$TEMP_DIR"
  }
  trap cleanup EXIT
  
  # Test function
  test_template() {
    local name=$1
    local database_provider=$2
    local database_type=$3
    local rest=$4
    local graphql=$5
    local grpc=$6
    
    echo "Testing: $name"
    
    # Generate project
    dotnet new ddap-api \
      --name "$name" \
      --database-provider "$database_provider" \
      --database-type "$database_type" \
      --rest "$rest" \
      --graphql "$graphql" \
      --grpc "$grpc" \
      --output "$TEMP_DIR/$name"
    
    # Verify files exist
    if [ ! -f "$TEMP_DIR/$name/$name.csproj" ]; then
      echo "❌ FAILED: Project file not created"
      return 1
    fi
    
    # Restore packages
    cd "$TEMP_DIR/$name"
    dotnet restore
    
    # Build project
    dotnet build --no-restore
    
    # Verify package references
    # Check for correct packages based on parameters
    
    echo "✅ PASSED: $name"
  }
  
  # Run tests
  echo "🧪 Template Validation Tests"
  echo "=============================="
  
  test_template "Test1" "dapper" "sqlserver" "true" "false" "false"
  test_template "Test2" "entityframework" "mysql" "false" "true" "false"
  # ... more tests
  
  echo ""
  echo "✅ All template tests passed!"
  ```

- [ ] **Fazer script executável**
  ```bash
  chmod +x tests/validate-template.sh
  ```

- [ ] **Testar script básico**
  ```bash
  ./tests/validate-template.sh
  ```

---

### Fase 3: Cenários de Teste (3-4h)

#### 3.1 Testes de Database Providers

- [ ] **SQL Server + Dapper**
  ```bash
  test_template "SqlServerDapper" "dapper" "sqlserver" "true" "false" "false"
  ```
  - [ ] Verificar pacote: `Microsoft.Data.SqlClient`
  - [ ] Verificar pacote: `Ddap.Data.Dapper`
  - [ ] Verificar ausência de: `Ddap.Data.Dapper.SqlServer` (não existe)

- [ ] **SQL Server + Entity Framework**
  ```bash
  test_template "SqlServerEF" "entityframework" "sqlserver" "false" "true" "false"
  ```
  - [ ] Verificar pacote: `Microsoft.EntityFrameworkCore.SqlServer`
  - [ ] Verificar pacote: `Ddap.Data.EntityFramework`

- [ ] **MySQL + Dapper**
  ```bash
  test_template "MySqlDapper" "dapper" "mysql" "false" "false" "true"
  ```
  - [ ] Verificar pacote: `MySqlConnector`
  - [ ] Verificar pacote: `Ddap.Data.Dapper`

- [ ] **MySQL + Entity Framework**
  ```bash
  test_template "MySqlEF" "entityframework" "mysql" "true" "true" "false"
  ```
  - [ ] Verificar pacote: `MySql.EntityFrameworkCore` (Oracle oficial)
  - [ ] Verificar ausência de: Pomelo (não forçado)

- [ ] **PostgreSQL + Dapper**
  ```bash
  test_template "PostgresDapper" "dapper" "postgresql" "true" "false" "false"
  ```
  - [ ] Verificar pacote: `Npgsql`
  - [ ] Verificar pacote: `Ddap.Data.Dapper`

- [ ] **PostgreSQL + Entity Framework**
  ```bash
  test_template "PostgresEF" "entityframework" "postgresql" "false" "true" "false"
  ```
  - [ ] Verificar pacote: `Npgsql.EntityFrameworkCore.PostgreSQL`

- [ ] **SQLite + Dapper**
  ```bash
  test_template "SqliteDapper" "dapper" "sqlite" "false" "false" "true"
  ```
  - [ ] Verificar pacote: `Microsoft.Data.Sqlite`

- [ ] **SQLite + Entity Framework**
  ```bash
  test_template "SqliteEF" "entityframework" "sqlite" "true" "false" "false"
  ```
  - [ ] Verificar pacote: `Microsoft.EntityFrameworkCore.Sqlite`

#### 3.2 Testes de API Providers

- [ ] **REST API**
  ```bash
  test_template "RestOnly" "dapper" "sqlserver" "true" "false" "false"
  ```
  - [ ] Verificar pacote: `Ddap.Rest`
  - [ ] Verificar ausência de: `Ddap.GraphQL`, `Ddap.Grpc`

- [ ] **GraphQL API**
  ```bash
  test_template "GraphQLOnly" "dapper" "sqlserver" "false" "true" "false"
  ```
  - [ ] Verificar pacote: `Ddap.GraphQL`
  - [ ] Verificar ausência de: `Ddap.Rest`, `Ddap.Grpc`

- [ ] **gRPC API**
  ```bash
  test_template "GrpcOnly" "dapper" "sqlserver" "false" "false" "true"
  ```
  - [ ] Verificar pacote: `Ddap.Grpc`
  - [ ] Verificar ausência de: `Ddap.Rest`, `Ddap.GraphQL`

- [ ] **REST + GraphQL**
  ```bash
  test_template "RestGraphQL" "dapper" "mysql" "true" "true" "false"
  ```
  - [ ] Verificar pacotes: `Ddap.Rest`, `Ddap.GraphQL`

- [ ] **REST + gRPC**
  ```bash
  test_template "RestGrpc" "entityframework" "postgresql" "true" "false" "true"
  ```
  - [ ] Verificar pacotes: `Ddap.Rest`, `Ddap.Grpc`

- [ ] **GraphQL + gRPC**
  ```bash
  test_template "GraphQLGrpc" "dapper" "sqlite" "false" "true" "true"
  ```
  - [ ] Verificar pacotes: `Ddap.GraphQL`, `Ddap.Grpc`

- [ ] **Todas as APIs**
  ```bash
  test_template "AllAPIs" "entityframework" "sqlserver" "true" "true" "true"
  ```
  - [ ] Verificar todos os pacotes de API

#### 3.3 Testes de Features Opcionais

- [ ] **Autenticação**
  ```bash
  test_template "WithAuth" "dapper" "sqlserver" "true" "false" "false" "true"
  ```
  - [ ] Verificar pacote: `Ddap.Auth`
  - [ ] Verificar configuração JWT em Program.cs

- [ ] **Subscriptions**
  ```bash
  test_template "WithSubs" "dapper" "sqlserver" "false" "true" "false" "false" "true"
  ```
  - [ ] Verificar pacote: `Ddap.Subscriptions`
  - [ ] Verificar configuração de subscriptions

- [ ] **Auto-reload**
  ```bash
  test_template "WithReload" "dapper" "sqlserver" "true" "false" "false" "false" "false" "true"
  ```
  - [ ] Verificar configuração de auto-reload

#### 3.4 Testes de Combinações Complexas

- [ ] **Configuração Mínima**
  ```bash
  test_template "Minimal" "dapper" "sqlite" "false" "false" "false" "false" "false" "false"
  ```
  - [ ] Apenas pacotes essenciais

- [ ] **Configuração Máxima**
  ```bash
  test_template "Maximum" "entityframework" "sqlserver" "true" "true" "true" "true" "true" "true"
  ```
  - [ ] Todos os pacotes e features

---

### Fase 4: Integração com CI (2h)

- [ ] **Adicionar job ao build.yml**
  ```yaml
  template-tests:
    name: Template Tests
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Install template
        run: |
          dotnet new install templates/ddap-api
      
      - name: Run template tests
        run: ./tests/validate-template.sh
  ```

- [ ] **Testar localmente antes de commit**
  ```bash
  # Simular CI localmente
  dotnet new install templates/ddap-api
  ./tests/validate-template.sh
  ```

- [ ] **Verificar que CI passa**
  - [ ] Push changes
  - [ ] Aguardar CI executar
  - [ ] Verificar logs se falhar

---

### Fase 5: Documentação (1h)

- [ ] **Criar tests/README.md**
  ```markdown
  # Template Tests
  
  Automated tests for the ddap-api template.
  
  ## Running Tests
  
  ```bash
  ./tests/validate-template.sh
  ```
  
  ## Test Coverage
  
  - Database providers: SQL Server, MySQL, PostgreSQL, SQLite
  - Data access: Dapper, Entity Framework
  - API types: REST, GraphQL, gRPC
  - Features: Authentication, Subscriptions, Auto-reload
  
  ## Adding New Tests
  
  Edit `validate-template.sh` and add new `test_template` calls.
  ```

- [ ] **Atualizar README.md principal**
  - [ ] Adicionar badge de template tests
  - [ ] Mencionar que template é testado

- [ ] **Atualizar CONTRIBUTING.md**
  - [ ] Adicionar seção sobre template testing
  - [ ] Explicar como executar testes localmente

---

### Fase 6: Validação Final (1-2h)

- [ ] **Executar todos os testes**
  ```bash
  ./tests/validate-template.sh
  ```
  - [ ] Verificar que todos passam
  - [ ] Corrigir falhas se houver

- [ ] **Revisar cobertura**
  - [ ] Confirmar que todos os cenários estão cobertos
  - [ ] Adicionar testes faltantes se necessário

- [ ] **Testar em ambiente limpo**
  ```bash
  # Docker container limpo
  docker run --rm -it -v $(pwd):/app mcr.microsoft.com/dotnet/sdk:10.0 bash
  cd /app
  dotnet new install templates/ddap-api
  ./tests/validate-template.sh
  ```

- [ ] **Code review**
  - [ ] Revisar código do script
  - [ ] Verificar se segue padrões do projeto
  - [ ] Validar comentários e documentação

---

## 📊 Cenários de Teste (Total: 30+)

### Matriz de Teste

| # | Database | Provider | REST | GraphQL | gRPC | Auth | Subs | Reload |
|---|----------|----------|------|---------|------|------|------|--------|
| 1 | SQL Server | Dapper | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| 2 | SQL Server | EF | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| 3 | MySQL | Dapper | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ |
| 4 | MySQL | EF | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| 5 | PostgreSQL | Dapper | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ |
| 6 | PostgreSQL | EF | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| 7 | SQLite | Dapper | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| 8 | SQLite | EF | ✅ | ❌ | ❌ | ✅ | ❌ | ❌ |
| 9 | SQL Server | Dapper | ❌ | ✅ | ❌ | ❌ | ✅ | ❌ |
| 10 | MySQL | EF | ❌ | ❌ | ✅ | ❌ | ❌ | ✅ |
| ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 29 | SQLite | Dapper | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ← Minimal
| 30 | SQL Server | EF | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ← Maximum

---

## 🎯 Critérios de Sucesso

### Para cada teste

✅ **Geração**: Template gera projeto sem erros  
✅ **Estrutura**: Arquivos corretos estão presentes  
✅ **Pacotes**: Referências de pacotes corretas no .csproj  
✅ **Restore**: `dotnet restore` executa com sucesso  
✅ **Build**: `dotnet build` compila sem erros ou warnings  
✅ **Configuração**: Program.cs contém código correto para os parâmetros

### Para o Sprint 4 completo

✅ **Cobertura**: 30+ cenários testados  
✅ **Automatização**: Script executável e mantível  
✅ **CI/CD**: Integrado no pipeline  
✅ **Documentação**: README completo  
✅ **Qualidade**: Todos os testes passando  

---

## 💡 Dicas de Implementação

### Otimização de Performance

```bash
# Executar testes em paralelo (se possível)
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# Usar cache de pacotes
export NUGET_PACKAGES="$HOME/.nuget/packages"
```

### Debugging

```bash
# Manter diretório temporário para debug
TEMP_DIR="./temp-template-tests"
mkdir -p "$TEMP_DIR"

# Não fazer cleanup em caso de erro
trap 'if [ $? -ne 0 ]; then echo "Test artifacts in: $TEMP_DIR"; fi' EXIT
```

### Validação de Pacotes

```bash
# Verificar pacote específico no .csproj
grep -q 'PackageReference Include="Ddap.Rest"' "$PROJECT_DIR/*.csproj"

# Verificar ausência de pacote
if grep -q 'Pomelo.EntityFrameworkCore.MySql' "$PROJECT_DIR/*.csproj"; then
  echo "❌ FAILED: Pomelo should not be included"
  return 1
fi
```

---

## 📝 Comandos Úteis

### Preparação

```bash
# Instalar template localmente
dotnet new install templates/ddap-api

# Listar templates instalados
dotnet new list | grep ddap

# Ver opções do template
dotnet new ddap-api --help
```

### Execução

```bash
# Executar todos os testes
./tests/validate-template.sh

# Executar teste específico (modificar script temporariamente)
# Comentar outros testes, deixar apenas o desejado

# Ver logs detalhados
./tests/validate-template.sh 2>&1 | tee template-tests.log
```

### Limpeza

```bash
# Desinstalar template
dotnet new uninstall templates/ddap-api

# Limpar cache de templates
rm -rf ~/.templateengine/

# Limpar pacotes NuGet
dotnet nuget locals all --clear
```

---

## 🚀 Próximos Passos Após Sprint 4

Após completar Sprint 4:

1. **Merge Sprint 4** para a epic branch
2. **Criar PR final** do epic para main
3. **Celebrar** 🎉 - Epic completo!

Sprints futuras (opcional):
- Sprint 5: Performance testing
- Sprint 6: Integration tests
- Sprint 7: E2E scenarios

---

## 📚 Referências

- [.NET Template Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)
- [Testing Templates Best Practices](https://github.com/dotnet/templating/wiki/Testing-Templates)
- ROTEIRO_ACOES.md - Ação 3.1 (Template Tests)
- GUIA_SPRINTS_SEQUENCIAIS.md - Sprint 4 Section

---

**Criado**: 2026-01-31  
**Sprint**: 4 de 4  
**Status**: 📋 Pronto para Implementação  
**Estimativa**: 8-12 horas  

**Boa sorte com a implementação! 🚀**
