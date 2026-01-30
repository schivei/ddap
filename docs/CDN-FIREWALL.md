# Domínios CDN para Liberar no Firewall

Este documento lista todos os domínios CDN que precisam ser liberados no firewall para que a documentação DDAP funcione corretamente.

## Domínios Necessários

### 1. cdnjs.cloudflare.com
**Usado para:** GitHub Markdown CSS  
**Recurso:** `https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/5.5.1/github-markdown.min.css`  
**Propósito:** Estilização do conteúdo markdown nas páginas de documentação

### 2. cdn.jsdelivr.net
**Usado para:** Bibliotecas JavaScript (marked.js e DOMPurify)  
**Recursos:**
- `https://cdn.jsdelivr.net/npm/marked/marked.min.js` - Parser de Markdown para JavaScript
- `https://cdn.jsdelivr.net/npm/dompurify@3.0.8/dist/purify.min.js` - Sanitização de HTML (segurança XSS)  
**Propósito:** Renderização de arquivos markdown (.md) em HTML nas páginas de documentação

### 3. img.shields.io (Opcional)
**Usado para:** Badges/shields no index.html  
**Recursos:**
- `https://img.shields.io/nuget/v/Ddap.Core` - Badge da versão NuGet
- `https://img.shields.io/github/license/schivei/ddap` - Badge da licença
- `https://img.shields.io/github/actions/workflow/status/schivei/ddap/build.yml` - Badge do status do build  
**Propósito:** Exibir badges informativos na página inicial (não crítico para funcionalidade)

## Resumo para Configuração do Firewall

### Domínios CRÍTICOS (obrigatórios):
```
cdnjs.cloudflare.com
cdn.jsdelivr.net
```

### Domínios OPCIONAIS (para badges):
```
img.shields.io
```

## Portas
- **Porta 443 (HTTPS)** - Todas as conexões são via HTTPS

## Arquivos Afetados

Sem esses CDNs liberados, as seguintes páginas mostrarão "Error Loading Document":
- get-started.html
- philosophy.html
- database-providers.html
- api-providers.html
- architecture.html
- auto-reload.html
- templates.html
- advanced.html
- troubleshooting.html
- client-getting-started.html
- client-rest.html
- client-graphql.html
- client-grpc.html
- extended-types.html
- raw-queries.html

**Nota:** A página index.html funciona mesmo sem os CDNs pois usa HTML estático, mas os badges não serão exibidos.

## Testando Após Liberação

Após liberar os domínios no firewall, verifique se as páginas carregam corretamente acessando:
- http://localhost:8080/get-started.html
- http://localhost:8080/philosophy.html

O conteúdo markdown deve ser renderizado corretamente ao invés de mostrar "Error Loading Document".

## Alternativa (Hospedar Localmente)

Se não for possível liberar os CDNs, você pode baixar e hospedar as bibliotecas localmente:

1. **marked.min.js** - https://cdn.jsdelivr.net/npm/marked/marked.min.js
2. **purify.min.js** - https://cdn.jsdelivr.net/npm/dompurify@3.0.8/dist/purify.min.js
3. **github-markdown.min.css** - https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/5.5.1/github-markdown.min.css

Depois, atualize todos os arquivos HTML para referenciar os arquivos locais ao invés dos CDNs.
