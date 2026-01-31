# GitHub Actions Workflows

Este diretório contém os workflows automatizados do projeto DDAP.

## 📄 Workflows Disponíveis

### 🔧 Build (`build.yml`)
Compila o projeto, executa testes e valida a qualidade do código.

**Triggers:**
- Push para branch `main`
- Pull requests

### 🔒 CodeQL (`codeql.yml`)
Análise de segurança automática do código.

**Triggers:**
- Push para branch `main`
- Pull requests
- Agendamento semanal

### 📚 Documentation (`docs.yml`)
Build e deploy da documentação para GitHub Pages.

**Triggers:**
- **Automático**: Push para `main` com mudanças em `docs/**`
- **Manual**: Pode ser executado em **qualquer branch**

#### Como Executar Manualmente o Workflow de Documentação

1. **Acesse**: https://github.com/schivei/ddap/actions/workflows/docs.yml

2. **Clique em "Run workflow"** (botão dropdown no canto superior direito)

3. **Configure a execução**:
   - **Use workflow from**: Selecione o branch desejado
   - **Deploy to GitHub Pages**: 
     - ✅ `true` - Faz build e deploy para https://schivei.github.io/ddap
     - ⬜ `false` - Apenas faz build (para testes/validação)

4. **Clique no botão verde "Run workflow"**

#### Casos de Uso

**Testar documentação em branch de feature:**
```
Branch: feature/new-docs
Deploy to GitHub Pages: false
```
Isso validará o build sem afetar o site publicado.

**Fazer hotfix na documentação:**
```
Branch: hotfix/fix-typo
Deploy to GitHub Pages: true
```
Isso fará deploy imediato sem precisar merge para main.

**Preview de mudanças antes do merge:**
```
Branch: pr/123
Deploy to GitHub Pages: false
```
Valida que o build funciona antes de fazer merge.

### 📦 Release (`release.yml`)
Automatiza a criação de releases e publicação de pacotes NuGet.

**Triggers:**
- Tags no formato `v*` (ex: `v1.0.0`)

### 🤖 Copilot Review (`copilot-review.yml`)
Review automatizado de código usando GitHub Copilot.

**Triggers:**
- Pull requests

## 🔐 Permissões

Os workflows possuem as seguintes permissões:

- **contents: write** - Necessário para commits automáticos
- **pages: write** - Necessário para deploy no GitHub Pages
- **id-token: write** - Necessário para autenticação OIDC

## 📊 Monitoramento

Você pode acompanhar o status de todos os workflows em:
https://github.com/schivei/ddap/actions

## 🐛 Troubleshooting

### Workflow não aparece para execução manual

Certifique-se de que você tem permissões de write no repositório.

### Deploy para GitHub Pages falha

Verifique se o GitHub Pages está configurado corretamente em:
https://github.com/schivei/ddap/settings/pages

**Source** deve estar configurado como: `GitHub Actions`

### Build falha em branch específico

Execute primeiro sem deploy (`deploy_to_pages: false`) para ver os erros de build sem afetar o site publicado.
