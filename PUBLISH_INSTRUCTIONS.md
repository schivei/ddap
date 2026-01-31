# Instruções para Publicar o Site DDAP

## Situação Atual

O site está configurado para ser publicado automaticamente via GitHub Pages através do workflow `.github/workflows/docs.yml`.

## Como Publicar o Site

### Opção 1: Merge do Pull Request (Recomendado)

1. **Acesse o Pull Request** no GitHub:
   - URL: https://github.com/schivei/ddap/pulls
   - Encontre o PR `copilot/move-language-theme-selector`

2. **Faça o Merge** para o branch `main`:
   ```
   Merge pull request → Confirm merge
   ```

3. **Aguarde o Deploy Automático**:
   - O workflow `docs.yml` será executado automaticamente
   - Tempo estimado: 2-5 minutos
   - Você pode acompanhar em: https://github.com/schivei/ddap/actions

4. **Acesse o Site Publicado**:
   - URL: https://schivei.github.io/ddap

### Opção 2: Executar Workflow Manualmente

Se preferir executar o workflow sem fazer merge:

1. **Acesse a página de Actions**:
   - URL: https://github.com/schivei/ddap/actions/workflows/docs.yml

2. **Clique em "Run workflow"**:
   - Selecione o branch `copilot/move-language-theme-selector`
   - Clique em "Run workflow" (botão verde)

3. **Aguarde a execução**:
   - O workflow irá build e deploy o site
   - Tempo estimado: 2-5 minutos

4. **Acesse o Site Publicado**:
   - URL: https://schivei.github.io/ddap

### Opção 3: Via Command Line (Se você tem permissões)

```bash
# 1. Checkout do branch main (criar se não existir)
git checkout -b main

# 2. Fazer merge do branch de feature
git merge copilot/move-language-theme-selector

# 3. Push para o remoto
git push -u origin main
```

## Configuração do GitHub Pages

Certifique-se de que o GitHub Pages está configurado corretamente:

1. Acesse: https://github.com/schivei/ddap/settings/pages

2. Configure:
   - **Source**: GitHub Actions
   - **Branch**: main
   - **Path**: / (root)

3. Salve as configurações

## Verificação

Após o deploy, você pode verificar:

1. **Status do Workflow**:
   - https://github.com/schivei/ddap/actions

2. **Site Publicado**:
   - https://schivei.github.io/ddap

3. **Verificar se as mudanças estão visíveis**:
   - ✅ Badges atualizados no topo
   - ✅ Seletores de idioma e tema fora do menu hamburger (mobile)
   - ✅ Ícone PNG aparecendo corretamente

## O Que Foi Alterado

### 1. Mobile UX Melhorado
- Seletores de tema e idioma agora ficam fixos no topo em mobile
- Não quebram mais dentro do menu hamburger

### 2. Badges Adicionados
- NuGet Version e Downloads
- GitHub Release
- Build, CodeQL e Docs Status
- Stars e Forks

### 3. Ícone PNG
- Convertido de SVG para PNG
- Múltiplos tamanhos (32, 64, 128, 256)
- Configurado para pacotes NuGet

## Troubleshooting

### Site não atualiza após deploy
- Limpe o cache do navegador (Ctrl+Shift+R)
- Aguarde alguns minutos para propagação do CDN

### Workflow falha
- Verifique os logs em: https://github.com/schivei/ddap/actions
- Procure por erros de build ou permissões

### GitHub Pages não está habilitado
- Acesse Settings → Pages
- Configure Source como "GitHub Actions"

## Contato

Se precisar de ajuda, abra uma issue em:
https://github.com/schivei/ddap/issues
