# AquaGuard.BddTests

Projeto de testes BDD da API AquaGuard, executado por HTTP contra a API já iniciada. Os cenários validam contratos e regras de consumo de água, contribuindo para o pilar ambiental, a rastreabilidade e a governança de acesso.

## Pré-requisitos

- .NET 9 SDK (a API alvo continua em .NET 8).
- AquaGuard.API em execução; por padrão `http://localhost:5187`.
- Credenciais locais dos perfis Admin, Operador e Auditor.
- Docker Desktop apenas para a execução opcional em contêiner.

Os testes que criam medidores e leituras gravam no banco conectado à API. Use apenas um ambiente local/de homologação autorizado; os medidores criados são desativados ao final de cada cenário marcado com `@limpar`.

## Configuração

Na pasta `AquaGuard.BddTests`, copie `.env.example` para `.env` e preencha as três senhas. O arquivo real `.env` é ignorado pelo Git e não deve ser compartilhado.

```text
BASE_URL=http://localhost:5187
INSECURE_SSL=false
ADMIN_EMAIL=admin@aquaguard.com
ADMIN_PASSWORD=<senha local>
OPERADOR_EMAIL=operador@aquaguard.com
OPERADOR_PASSWORD=<senha local>
AUDITOR_EMAIL=auditor@aquaguard.com
AUDITOR_PASSWORD=<senha local>
```

Variáveis definidas no processo têm precedência sobre as do `.env`. Para certificados HTTPS locais, `INSECURE_SSL=true` desativa a validação do certificado apenas no cliente de testes. A execução é abortada se `BASE_URL` contiver o identificador do domínio de produção.

## Executar localmente

Na raiz do repositório:

```powershell
dotnet test .\AquaGuard.BddTests\AquaGuard.BddTests.csproj
```

Executar apenas os cenários do pilar ambiental:

```powershell
dotnet test .\AquaGuard.BddTests\AquaGuard.BddTests.csproj --filter "Category=esg-environmental"
```

As tags Gherkin são expostas pelo Reqnroll como categoria de teste. Os resultados podem ser salvos em `test-results/`:

```powershell
dotnet test .\AquaGuard.BddTests\AquaGuard.BddTests.csproj `
  --logger "trx;LogFileName=resultado.trx" `
  --logger "html;LogFileName=relatorio.html" `
  --results-directory .\AquaGuard.BddTests\test-results
```

## Executar via Docker

Com a API em execução no host e o `.env` preenchido, execute dentro de `AquaGuard.BddTests`:

```powershell
docker compose up --build
```

O Compose usa `host.docker.internal:5187` para alcançar a API do host. Se necessário, defina `BASE_URL_DOCKER` para substituir essa URL. Os relatórios são gravados em `test-results/`. O `.dockerignore` impede que o `.env` seja incorporado à imagem.

## Estrutura

```text
Features/   cenários Gherkin em português
Steps/      definições de steps e operações de teste
Support/    cliente HTTP, configuração e limpeza
Schemas/    contratos JSON de requests e respostas da API
docs/       mapa de endpoints e catálogo de cenários
```

## Cobertura de endpoints

| Endpoint | Cenários principais |
|---|---|
| `POST /api/auth/login` | login dos três perfis, senha inválida e body inválido |
| `GET /api/medidores` | listagem paginada e acesso sem token |
| `GET /api/medidores/{id}` | consulta existente e inexistente |
| `POST /api/medidores` | criação por Admin/Operador, negação ao Auditor e limite inválido |
| `PUT /api/medidores/{id}` | atualização de medidor criado no cenário |
| `DELETE /api/medidores/{id}` | soft delete Admin e negação ao Operador |
| `GET /api/leituras` | listagem paginada e acesso sem token |
| `GET /api/leituras/periodo` | consulta por período |
| `GET /api/leituras/medidor/{id}` | consulta por medidor |
| `POST /api/leituras` | leitura/rastreabilidade, alertas, valores inválidos e RBAC |
| `GET /api/alertas` | listagem paginada |
| `GET /api/alertas/pendentes` | lista e alerta gerado por leitura elevada |
| `PUT /api/alertas/{id}/resolver` | resolução, alerta inexistente e negação ao Auditor |
| `GET /api/relatorios/mensal` | indicadores mensais e mês inválido |
| `GET /api/relatorios/medidor/{id}` | comparação mensal e medidor inexistente |
| `GET /api/dashboard` | acesso dos perfis e negação sem token |
| `GET /health` | disponibilidade |

## Limitações e limpeza

- Nomes/status foram conferidos nos controllers, view models, validadores e Swagger da API em execução. Há respostas diretas em vários recursos, `ApiResponseViewModel` no login/alguns erros e validação automática nos erros 400.
- O cenário `@bug-api-periodo-invertido` verifica que a API rejeita intervalos em que `fim` antecede `inicio`.
- Correções para problemas encontrados durante os testes estão registradas em [docs/bugs-encontrados.md](./docs/bugs-encontrados.md).
- O modelo retornado de leitura identifica `UsuarioId` e `NomeUsuario`, não o e-mail. O cenário valida os campos e a API carrega a navegação do usuário na resposta.
- O valor do alerta na API é `ConsumoExcessivo` (nome da enum), sem espaço.
- A autorização padrão pode retornar 401/403 sem corpo JSON; nesses casos os cenários validam status, não inventam um schema de erro.
- Cenários de leitura/alerta criam medidores com código único `BDD-...` e os desativam via Admin no hook de limpeza. Leituras e alertas não são apagados pela API; use homologação e não execute em produção.
- O projeto não cria credenciais, banco ou API; as senhas são fornecidas pelo usuário no `.env`.
