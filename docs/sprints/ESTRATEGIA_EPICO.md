# Estratégia de Épico - DDAP Project Improvement

**Data**: 31 de Janeiro de 2026  
**Tipo**: Epic (múltiplos PRs encadeados)

---

## 🎯 Conceito de Épico

Tratar este trabalho como um **épico** significa criar uma cadeia de PRs onde cada PR subsequente é criado a partir da branch do PR anterior, formando uma hierarquia:

```
main
 │
 └─ PR #1: epic/improve-ddap-project (ESTE) ─── BASE DO ÉPICO
     │
     ├─ PR #2: feat/fix-template-flags ──────── Sprint 1
     │   │
     │   └─ (merge para epic/improve-ddap-project)
     │
     ├─ PR #3: feat/resolve-packages ───────── Sprint 2
     │   │
     │   └─ (merge para epic/improve-ddap-project)
     │
     ├─ PR #4: feat/update-docs-site ───────── Sprint 3
     │   │
     │   └─ (merge para epic/improve-ddap-project)
     │
     └─ PR #5: feat/add-template-tests ─────── Sprint 4
         │
         └─ (merge para epic/improve-ddap-project)

Quando todos os PRs do épico estiverem completos:
epic/improve-ddap-project ──────────► main (MERGE FINAL)
```

---

## ✅ Vantagens desta Abordagem

### 1. Organização Hierárquica
- ✅ Epic branch como "container" de todas as mudanças
- ✅ Cada sprint tem seu próprio PR focado
- ✅ Histórico de commits organizado por funcionalidade

### 2. Revisão Incremental
- ✅ Cada PR pode ser revisado independentemente
- ✅ Aprovações progressivas sem bloquear trabalho seguinte
- ✅ Feedback específico por funcionalidade

### 3. Flexibilidade
- ✅ Pode trabalhar em Sprint 2 enquanto Sprint 1 está em revisão
- ✅ Pode reordenar sprints se necessário
- ✅ Pode pausar o épico a qualquer momento

### 4. Rollback Granular
- ✅ Se Sprint 3 tiver problemas, reverte apenas aquele PR
- ✅ Sprints 1, 2, 4 não são afetados
- ✅ Menos risco de "jogar fora" muito trabalho

### 5. Deploy Progressivo (Opcional)
- ✅ Pode fazer merge parcial do épico (ex: só Sprints 1-2)
- ✅ Deploy em produção de forma incremental
- ✅ Validação em prod antes de continuar

---

## 📋 Estrutura do Épico

### Epic PR (PR #1 - ESTE)

**Branch**: `copilot/improve-ddap-project` (ou `epic/improve-ddap-project`)  
**Base**: `main`  
**Conteúdo**:
- ✅ Análise completa (12 documentos, ~167k palavras)
- ✅ Diagnóstico de bugs (3 críticos identificados)
- ✅ Roadmap estratégico e tático
- ✅ Site multi-idioma gerado
- ✅ Ícone profissional criado
- ✅ Branding melhorado

**Status**: ✅ Completo e pronto para revisão

**Propósito**: Foundation - Estabelece a base de conhecimento para todos os sprints seguintes

---

### Sprint 1: Fix Template API Flags (PR #2)

**Branch**: `feat/fix-template-flags`  
**Base**: `copilot/improve-ddap-project` ⬅️ **A partir do épico**  
**Tempo**: 4-6 horas  

**Escopo**:
- [ ] Corrigir `template.json` computed symbols
- [ ] Testar geração com `--rest`, `--graphql`, `--grpc`
- [ ] Validar build de projetos gerados
- [ ] Atualizar versão para 1.0.3
- [ ] Publicar no NuGet (opcional neste PR)

**Critérios de Aceite**:
- ✅ `dotnet new ddap-api --rest true` inclui Ddap.Rest
- ✅ Todas as combinações de flags funcionam
- ✅ Projetos gerados compilam sem erros

**Merge para**: `copilot/improve-ddap-project`

---

### Sprint 2: Resolve Package References (PR #3)

**Branch**: `feat/resolve-packages`  
**Base**: `feat/fix-template-flags` ⬅️ **A partir do Sprint 1**  
**Tempo**: 6-11 horas  

**Escopo**:
- [ ] Remover referências a pacotes inexistentes
- [ ] Usar `Ddap.Data.Dapper` base + driver do usuário
- [ ] Atualizar para SQL Server, MySQL, PostgreSQL, SQLite
- [ ] Remover Pomelo forçado (filosofia)
- [ ] Atualizar documentação

**Critérios de Aceite**:
- ✅ Template não referencia pacotes inexistentes
- ✅ Usuário escolhe driver MySQL (não forçado)
- ✅ Projetos gerados compilam para todos os DBs

**Merge para**: `copilot/improve-ddap-project`

---

### Sprint 3: Update Documentation Site (PR #4)

**Branch**: `feat/update-docs-site`  
**Base**: `feat/resolve-packages` ⬅️ **A partir do Sprint 2**  
**Tempo**: 5-9 horas  

**Escopo**:
- [ ] Adicionar avisos sobre issues conhecidos
- [ ] Integrar ícone no site
- [ ] Criar página "Why DDAP?"
- [ ] Publicar website no GitHub Pages
- [ ] Testar em todos os 7 idiomas

**Critérios de Aceite**:
- ✅ Site publicado em https://schivei.github.io/ddap
- ✅ Ícone aparece na homepage
- ✅ Página "Why DDAP?" acessível em 7 idiomas

**Merge para**: `copilot/improve-ddap-project`

---

### Sprint 4: Add Template Tests (PR #5)

**Branch**: `feat/add-template-tests`  
**Base**: `feat/update-docs-site` ⬅️ **A partir do Sprint 3**  
**Tempo**: 8-12 horas  

**Escopo**:
- [ ] Criar `tests/template-validation.sh`
- [ ] Implementar testes para 64+ cenários
- [ ] Integrar com CI (`.github/workflows/build.yml`)
- [ ] Documentar execução local
- [ ] Validar que todos os testes passam

**Critérios de Aceite**:
- ✅ 64+ cenários testados automaticamente
- ✅ CI bloqueia PRs com templates quebrados
- ✅ Testes podem ser executados localmente

**Merge para**: `copilot/improve-ddap-project`

---

## 🔄 Fluxo de Trabalho

### Fase 1: Setup do Épico

1. **Criar Epic PR** (ESTE - já feito ✅)
   ```bash
   # Branch: copilot/improve-ddap-project
   # Base: main
   # Status: Em revisão
   ```

2. **Revisar e Aprovar Epic PR** (parcialmente)
   - Documentação pode ser aprovada mesmo que sprints não estejam prontos
   - Estabelece a base de conhecimento

### Fase 2: Trabalho nos Sprints

3. **Criar Sprint 1**
   ```bash
   # A partir da branch do épico
   git checkout copilot/improve-ddap-project
   git pull origin copilot/improve-ddap-project
   git checkout -b feat/fix-template-flags
   
   # Fazer as mudanças...
   # Commit e push
   
   # Criar PR
   # Base: copilot/improve-ddap-project (não main!)
   # Compare: feat/fix-template-flags
   ```

4. **Revisar Sprint 1**
   - Revisão focada apenas nas mudanças do Sprint 1
   - Aprovação não bloqueia trabalho no Sprint 2

5. **Merge Sprint 1 → Epic**
   ```bash
   # Após aprovação
   git checkout copilot/improve-ddap-project
   git merge feat/fix-template-flags
   git push origin copilot/improve-ddap-project
   ```

6. **Criar Sprint 2** (a partir da branch atualizada do épico)
   ```bash
   git checkout copilot/improve-ddap-project
   git pull origin copilot/improve-ddap-project
   git checkout -b feat/resolve-packages
   
   # Agora tem as mudanças do Sprint 1 incluídas!
   ```

7. **Repetir para Sprints 3 e 4**

### Fase 3: Finalização

8. **Validação Final do Épico**
   - Todos os sprints merged para o épico
   - Epic branch contém trabalho completo
   - Revisão final do conjunto

9. **Merge Epic → Main**
   ```bash
   # Criar PR final
   # Base: main
   # Compare: copilot/improve-ddap-project
   
   # Após aprovação final
   git checkout main
   git merge copilot/improve-ddap-project
   git push origin main
   ```

---

## 📊 Comparação: Épico vs PRs Independentes

### Abordagem de Épico (RECOMENDADA)

```
main ← epic ← sprint1 ← sprint2 ← sprint3 ← sprint4
       │
       └─ Merge único no final com todo o trabalho validado
```

**Vantagens**:
- ✅ Sprints podem depender uns dos outros naturalmente
- ✅ Trabalho contínuo sem esperar merges em main
- ✅ Um único ponto de decisão final (merge do épico)
- ✅ Histórico limpo (epic como container)
- ✅ Fácil de abandonar/pausar todo o épico se necessário

**Desvantagens**:
- ⚠️ Epic branch vive por mais tempo (mais conflitos potenciais com main)
- ⚠️ Precisa sync periódico com main (`git merge main`)
- ⚠️ Merge final pode ser grande

### Abordagem de PRs Independentes

```
main ← sprint1
main ← sprint2 (depende de sprint1 estar em main)
main ← sprint3 (depende de sprint2 estar em main)
main ← sprint4 (depende de sprint3 estar em main)
```

**Vantagens**:
- ✅ Cada PR vai direto para main após aprovação
- ✅ Sem branch de longa duração
- ✅ Menos conflitos com main

**Desvantagens**:
- ❌ Sprint 2 não pode começar até Sprint 1 estar em main
- ❌ Depende de aprovações rápidas para não bloquear
- ❌ Mais difícil de pausar/abandonar trabalho parcial
- ❌ Se Sprint 3 tiver problemas, Sprints 1-2 já estão em prod

---

## 🎯 Recomendação Final

### Use Épico SE:
- ✅ Quer trabalhar continuamente sem esperar approvals
- ✅ Quer flexibilidade de reordenar sprints
- ✅ Quer decisão única de "go/no-go" no final
- ✅ Tem confidence no roadmap completo
- ✅ Pode fazer sync periódico com main

### Use PRs Independentes SE:
- ✅ Precisa de deploy incremental em produção
- ✅ Sprints são completamente independentes
- ✅ Tem aprovações muito rápidas (< 1 dia)
- ✅ Quer minimizar branch de longa duração
- ✅ Cada sprint agrega valor imediato

---

## 💡 Recomendação para DDAP

**Use ÉPICO** 🎯

**Razões**:
1. Sprints são **dependentes** (Sprint 2 usa base do Sprint 1)
2. Trabalho contínuo é **mais eficiente**
3. Revisão final do **conjunto** faz sentido
4. Pode **pausar** após qualquer sprint se necessário
5. **Um único deploy** de todas as correções juntas

**Estratégia**:
- Manter epic branch `copilot/improve-ddap-project`
- Sync com main semanalmente (`git merge main`)
- Criar PRs encadeados para cada sprint
- Merge final quando todos os sprints estiverem validados

---

## 📝 Comandos Git para Épico

### Setup Inicial (já feito)
```bash
# Epic branch já existe
git checkout copilot/improve-ddap-project
git push origin copilot/improve-ddap-project
```

### Criar Sprint 1
```bash
git checkout copilot/improve-ddap-project
git pull origin copilot/improve-ddap-project
git checkout -b feat/fix-template-flags

# Trabalhar...
git add .
git commit -m "Fix template API provider flags"
git push origin feat/fix-template-flags

# Criar PR no GitHub:
# Base: copilot/improve-ddap-project
# Compare: feat/fix-template-flags
```

### Merge Sprint 1 para Epic
```bash
# Após aprovação do PR
git checkout copilot/improve-ddap-project
git merge feat/fix-template-flags --no-ff
git push origin copilot/improve-ddap-project
```

### Criar Sprint 2 (a partir do épico atualizado)
```bash
git checkout copilot/improve-ddap-project
git pull origin copilot/improve-ddap-project  # Agora tem Sprint 1!
git checkout -b feat/resolve-packages

# Trabalhar...
git add .
git commit -m "Resolve package references"
git push origin feat/resolve-packages

# Criar PR no GitHub:
# Base: copilot/improve-ddap-project
# Compare: feat/resolve-packages
```

### Sync Epic com Main (periódico)
```bash
git checkout copilot/improve-ddap-project
git fetch origin
git merge origin/main
git push origin copilot/improve-ddap-project

# Propagar para branches ativas
git checkout feat/resolve-packages  # Se estiver trabalhando neste
git merge copilot/improve-ddap-project
git push origin feat/resolve-packages
```

### Merge Epic para Main (final)
```bash
# Quando todos os sprints estiverem completos e aprovados

# Criar PR final no GitHub:
# Base: main
# Compare: copilot/improve-ddap-project
# Título: "Epic: DDAP Project Improvement - Complete"

# Após aprovação
git checkout main
git merge copilot/improve-ddap-project --no-ff
git push origin main

# Deletar branches (opcional)
git branch -d copilot/improve-ddap-project
git push origin --delete copilot/improve-ddap-project
```

---

## 🎬 Próximos Passos Imediatos

### 1. Manter Este PR Como Epic Base
- [x] Este PR já está configurado corretamente
- [x] Branch: `copilot/improve-ddap-project`
- [x] Contém toda a análise e documentação
- [ ] Atualizar descrição do PR indicando que é um Epic

### 2. Preparar para Sprint 1
- [ ] Aguardar revisão inicial deste Epic PR
- [ ] Quando pronto, criar branch `feat/fix-template-flags`
- [ ] Base: `copilot/improve-ddap-project`
- [ ] Implementar Ação 1.1 (4-6h)

### 3. Criar PR do Sprint 1
- [ ] Abrir PR no GitHub
- [ ] Base: `copilot/improve-ddap-project` (não main!)
- [ ] Título: "Sprint 1: Fix Template API Provider Flags"
- [ ] Descrição: Link para este epic + escopo específico

### 4. Repetir Para Próximos Sprints
- [ ] Sprint 2 a partir de Sprint 1
- [ ] Sprint 3 a partir de Sprint 2
- [ ] Sprint 4 a partir de Sprint 3

---

## ✅ Checklist de Epic

### Epic Setup
- [x] Epic branch criada (`copilot/improve-ddap-project`)
- [x] Epic PR aberto (este)
- [x] Documentação base completa
- [ ] Epic PR descrito como tal
- [ ] Roadmap de sprints documentado

### Sprint 1
- [ ] Branch criada a partir do epic
- [ ] PR aberto (base: epic branch)
- [ ] Implementação completa
- [ ] Testes passando
- [ ] Aprovado e merged para epic

### Sprint 2
- [ ] Branch criada a partir do epic (com Sprint 1)
- [ ] PR aberto (base: epic branch)
- [ ] Implementação completa
- [ ] Testes passando
- [ ] Aprovado e merged para epic

### Sprint 3
- [ ] Branch criada a partir do epic (com Sprints 1-2)
- [ ] PR aberto (base: epic branch)
- [ ] Implementação completa
- [ ] Testes passando
- [ ] Aprovado e merged para epic

### Sprint 4
- [ ] Branch criada a partir do epic (com Sprints 1-3)
- [ ] PR aberto (base: epic branch)
- [ ] Implementação completa
- [ ] Testes passando
- [ ] Aprovado e merged para epic

### Epic Finalização
- [ ] Todos os sprints merged para epic
- [ ] Validação final do conjunto
- [ ] Sync final com main
- [ ] PR final: epic → main
- [ ] Aprovação e merge para main
- [ ] Deploy em produção
- [ ] Branches limpas

---

## 📚 Referências

- **ROTEIRO_ACOES.md**: Detalhes de cada sprint
- **ANALISE_TEMPO_PRODUTIVIDADE.md**: Estimativas de tempo
- **STRATEGIC_ROADMAP.md**: Visão de longo prazo

---

**Esta estratégia de épico permite trabalho contínuo, revisão incremental, e flexibilidade máxima!** 🚀
