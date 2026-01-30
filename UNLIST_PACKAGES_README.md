# Unlist Deprecated NuGet Packages

Este script automatiza o processo de remover (unlist) pacotes NuGet deprecados do NuGet.org que não fazem mais parte da solução atual do projeto DDAP.

## 📋 Pré-requisitos

- `dotnet` CLI instalado
- `jq` para parsing de JSON (o script tenta instalar automaticamente se não estiver presente)
- `curl` para fazer requisições HTTP
- Variável de ambiente `NUGET_API_KEY` configurada com sua chave de API do NuGet.org

## 🔑 Configurando a API Key

Antes de executar o script, você precisa configurar sua chave de API do NuGet.org:

```bash
export NUGET_API_KEY=your-api-key-here
```

Para tornar permanente, adicione ao seu `.bashrc`, `.zshrc`, ou `.profile`:

```bash
echo 'export NUGET_API_KEY=your-api-key-here' >> ~/.bashrc
source ~/.bashrc
```

## 🚀 Uso

### Executar em modo de teste (dry-run)

Recomendado para a primeira execução para ver o que seria feito sem fazer alterações reais:

```bash
./unlist-deprecated-packages.sh --dry-run
```

ou

```bash
./unlist-deprecated-packages.sh -d
```

### Executar para unlist real

Quando estiver pronto para realmente remover os pacotes:

```bash
./unlist-deprecated-packages.sh
```

## 📦 Pacotes Gerenciados

### Pacotes Atuais (mantidos na listagem)

Os seguintes pacotes estão atualmente na solução e **NÃO** serão afetados:

- Ddap.Aspire
- Ddap.Auth
- Ddap.Client.Core
- Ddap.Client.GraphQL
- Ddap.Client.Grpc
- Ddap.Client.Rest
- Ddap.CodeGen
- Ddap.Core
- Ddap.Data.Dapper
- Ddap.Data.EntityFramework
- Ddap.GraphQL
- Ddap.Grpc
- Ddap.Rest
- Ddap.Subscriptions
- Ddap.Templates

### Pacotes Deprecados (serão removidos)

Os seguintes pacotes foram removidos da solução e serão **unlisted**:

- Ddap.Memory

## 🔄 Adicionando Novos Pacotes Deprecados

Se você remover um pacote da solução no futuro, edite o script e adicione o nome do pacote ao array `DEPRECATED_PACKAGES`:

```bash
DEPRECATED_PACKAGES=(
    "Ddap.Memory"
    "Ddap.NovoPackageDeprecado"  # Adicione aqui
)
```

## ⚙️ Como Funciona

1. **Validação**: Verifica se a API Key está configurada e se as ferramentas necessárias estão instaladas
2. **Listagem**: Mostra os pacotes atuais da solução e os pacotes marcados como deprecados
3. **Busca**: Para cada pacote deprecado, busca todas as versões publicadas no NuGet.org
4. **Unlist**: Remove cada versão usando o comando `dotnet nuget delete`
5. **Relatório**: Mostra um resumo do processo com sucessos e falhas

## 📝 Notas Importantes

- **Unlist não é Delete**: Pacotes "unlisted" ainda podem ser baixados se você souber a versão exata, mas não aparecem em buscas ou listagens do NuGet.org
- **Irreversível**: Uma vez unlisted via API, você precisará usar o portal web do NuGet.org para relistar
- **Rate Limiting**: O NuGet.org pode ter limites de taxa. O script processa um pacote por vez
- **Backup**: Não há backup automático. Certifique-se de que realmente quer remover os pacotes antes de executar

## 🐛 Troubleshooting

### Erro: "NUGET_API_KEY environment variable is not set"

Configure a variável de ambiente como mostrado na seção "Configurando a API Key".

### Erro: "dotnet CLI is not installed"

Instale o .NET SDK: https://dotnet.microsoft.com/download

### Erro: "jq is not installed"

O script tenta instalar automaticamente. Se falhar, instale manualmente:

- Ubuntu/Debian: `sudo apt-get install jq`
- MacOS: `brew install jq`
- Windows: `choco install jq` ou baixe de https://stedolan.github.io/jq/

### Pacote já está unlisted

Se o script reportar que um pacote já está unlisted, isso é normal e não é um erro. O pacote já foi removido anteriormente.

## 📄 Logs

O script fornece output colorido detalhado:

- 🔵 Azul: Informações gerais e progresso
- 🟢 Verde: Sucessos
- 🟡 Amarelo: Avisos e modo dry-run
- 🔴 Vermelho: Erros

## 🔒 Segurança

- Nunca commite sua API Key no repositório
- Mantenha a API Key segura e não a compartilhe
- Use `.gitignore` para garantir que arquivos com credenciais não sejam commitados
- Considere usar um secret manager para produção

## 📚 Referências

- [NuGet API Documentation](https://docs.microsoft.com/en-us/nuget/api/overview)
- [dotnet nuget delete](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-delete)
