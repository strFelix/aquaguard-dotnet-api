# Projeto - Cidades ESGInteligentes

## AquaGuard API

API REST para monitoramento de consumo de água, detecção automática de vazamentos e geração de indicadores ESG de preservação de recursos hídricos, desenvolvida como projeto acadêmico com foco no tema **Acesso à Água e Preservação de Recursos Naturais**.

## Conceito do Projeto

O AquaGuard nasce da necessidade de dar visibilidade ao consumo de água em residências, condomínios e empresas, permitindo identificar desperdícios antes que se tornem problemas financeiros ou ambientais relevantes. A proposta central é simples: um medidor registra leituras periódicas de consumo, e o sistema analisa esses dados continuamente para detectar dois cenários críticos — vazamentos (quando o consumo de um período foge muito do padrão histórico) e consumo excessivo (quando o total mensal ultrapassa um limite definido para aquele ponto de monitoramento).

A partir dessas leituras, o sistema também consolida indicadores de economia, comparando o consumo do mês corrente com o mês anterior, e disponibiliza um dashboard único com a visão geral de todos os medidores ativos, alertas pendentes e vazamentos detectados.

A API não possui interface gráfica própria; o consumo é feito via Swagger, Postman ou Insomnia, e o projeto prioriza arquitetura limpa, segurança baseada em perfis de acesso e cobertura de testes automatizados.

## Stack Tecnológica

| Camada | Tecnologia |
|---|---|
| Framework | .NET 8 / ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Banco de dados | PostgreSQL (Supabase) |
| Autenticação | JWT Bearer |
| Hashing de senha | BCrypt.Net |
| Validação | FluentValidation |
| Documentação | Swagger / OpenAPI |
| Testes | xUnit + EF Core InMemory Provider |
| Containerização e orquestração | Docker e Docker Compose |
| Health Check | Entity Framework Core |

## Arquitetura

O projeto adapta o conceito MVVM para o contexto de uma Web API REST, mantendo a separação de responsabilidades exigida pela disciplina:

- **Model** — entidades de domínio (`Models/`) e camada de acesso a dados via Repository Pattern (`Repositories/`)
- **ViewModel** — contratos de entrada e saída da API (`ViewModels/`), desacoplados das entidades de banco
- **View** — os endpoints REST (`Controllers/`), consumidos via Swagger, Postman ou Insomnia
- **Service** — camada intermediária de regras de negócio (`Services/`), orquestrando repositórios e aplicando as validações de domínio

```
AquaGuard.API
│
├── Controllers/              Endpoints REST organizados por recurso
├── Models/
│   └── Enums/                UserRole, TipoAlerta, StatusAlerta
├── ViewModels/                Request/Response DTOs por módulo
├── Services/
│   └── Interfaces/            Contratos da camada de negócio
├── Repositories/
│   └── Interfaces/            Contratos da camada de persistência
├── Data/                      DbContext e configuração do EF Core
├── Authentication/             JwtSettings e geração de token
├── Validators/                Regras do FluentValidation
├── Middleware/                Tratamento global de exceções
├── Extensions/                 Configuração de DI, Swagger e Auth
├── Helpers/                    Lógica auxiliar (paginação, análise de consumo, datas)
└── Program.cs

tests/AquaGuard.Tests/
├── Controllers/                Testes de integração por controller
├── Services/                    Testes unitários da camada de negócio
└── Fixtures/                    WebApplicationFactory customizada
```

## Modelo de Dados

| Entidade | Descrição |
|---|---|
| `Usuario` | Responsável pela autenticação; possui uma `Role` (Admin, Operador ou Auditor) |
| `Medidor` | Ponto de monitoramento físico, com código único, localização e limite mensal configurável |
| `LeituraConsumo` | Registro pontual de consumo em litros, vinculado a um medidor e ao usuário que o registrou |
| `Alerta` | Evento gerado automaticamente pelo sistema, classificado por tipo (Vazamento, Consumo Excessivo, Falha de Leitura) e status (Pendente, Resolvido) |

## Regras de Negócio

### Detecção de Vazamento

A cada nova leitura registrada, o sistema calcula a média de consumo dos últimos 7 dias daquele medidor. Se o consumo da leitura atual ultrapassar 150% dessa média, um alerta do tipo **Vazamento** é criado automaticamente, com a descrição já contendo o percentual de desvio identificado.

### Consumo Excessivo

Cada medidor possui um limite mensal configurável (`LimiteMensalLitros`). Sempre que o consumo acumulado do mês corrente ultrapassa esse limite, um alerta do tipo **Consumo Excessivo** é gerado, registrando o percentual excedido.

### Indicador de Economia

Os relatórios mensais e o dashboard comparam o consumo total do mês corrente com o mês anterior, retornando o percentual de economia (ou aumento) e a diferença absoluta em litros.

## Segurança e Perfis de Acesso

A autenticação é feita via JWT, com claims de identificação e role do usuário embutidas no token. Os perfis seguem a seguinte matriz de permissões:

| Perfil | Permissões |
|---|---|
| **Admin** | Acesso total, incluindo desativação de medidores |
| **Operador** | Cadastro e atualização de medidores, registro de leituras, resolução de alertas |
| **Auditor** | Somente leitura em todos os recursos |

## Endpoints

### Autenticação

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/auth/login` | Autentica o usuário e retorna o token JWT |

### Medidores

| Método | Rota | Perfil mínimo |
|---|---|---|
| GET | `/api/medidores?page=&pageSize=` | Qualquer autenticado |
| GET | `/api/medidores/{id}` | Qualquer autenticado |
| POST | `/api/medidores` | Admin, Operador |
| PUT | `/api/medidores/{id}` | Admin, Operador |
| DELETE | `/api/medidores/{id}` | Admin (soft delete) |

### Leituras

| Método | Rota | Perfil mínimo |
|---|---|---|
| GET | `/api/leituras?page=&pageSize=` | Qualquer autenticado |
| GET | `/api/leituras/periodo?inicio=&fim=` | Qualquer autenticado |
| GET | `/api/leituras/medidor/{id}` | Qualquer autenticado |
| POST | `/api/leituras` | Admin, Operador |

### Alertas

| Método | Rota | Perfil mínimo |
|---|---|---|
| GET | `/api/alertas?page=&pageSize=` | Qualquer autenticado |
| GET | `/api/alertas/pendentes` | Qualquer autenticado |
| PUT | `/api/alertas/{id}/resolver` | Admin, Operador |

### Relatórios

| Método | Rota | Perfil mínimo |
|---|---|---|
| GET | `/api/relatorios/mensal?ano=&mes=` | Qualquer autenticado |
| GET | `/api/relatorios/medidor/{id}` | Qualquer autenticado |

### Dashboard

| Método | Rota | Perfil mínimo |
|---|---|---|
| GET | `/api/dashboard` | Qualquer autenticado |

### Infraestrutura

| Método | Rota | Descrição |
|---|---|---|
| GET | `/health` | Health check de disponibilidade do banco de dados |

Todos os endpoints de listagem seguem o padrão de paginação:

```json
{
  "page": 1,
  "pageSize": 10,
  "totalRecords": 150,
  "totalPages": 15,
  "data": []
}
```

## Tratamento de Erros

Exceções não tratadas são capturadas por um middleware global, retornando sempre o formato:

```json
{
  "success": false,
  "message": "Descrição do erro",
  "statusCode": 400,
  "timestamp": "2026-06-17T10:00:00"
}
```

## Como Executar

### Pré-requisitos

- .NET 8 SDK
- PostgreSQL 15 ou superior (local ou Supabase)
- Visual Studio 2022 ou superior

### Configuração

1. Clone o repositório
2. Configure uma instância PostgreSQL local ou crie um projeto gratuito no [Supabase](https://supabase.com/pricing). O plano grátis inclui 500 MB e pausa projetos após uma semana sem atividade. Configure a connection string como segredo, sem adicioná-la ao Git:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string-do-postgres>" --project AquaGuard.API
```

No Azure App Service, defina a configuração de aplicativo `ConnectionStrings__DefaultConnection` com a connection string **Session pooler** exibida no painel do Supabase e SSL habilitado (`SSL Mode=Require`). Use também uma chave JWT própria em `Jwt__Key`; não publique a chave demonstrativa do `appsettings.json`.

3. Aplique as migrations ao banco configurado:

```powershell
dotnet ef database update --project AquaGuard.API
```

> A migration PostgreSQL cria o schema do zero e inclui os dados iniciais; ela não converte nem transfere dados de uma instalação SQL Server anterior. Use um banco Supabase vazio.

4. Execute o projeto (`F5` no Visual Studio ou `dotnet run`)
5. Acesse o Swagger em `https://localhost:{porta}/swagger`

### Usuários de Seed

| Email | Senha | Role |
|---|---|---|
| admin@aquaguard.com | Admin@123 | Admin |
| operador@aquaguard.com | Operador@123 | Operador |
| auditor@aquaguard.com | Auditor@123 | Auditor |

> Essas contas e senhas são apenas para demonstração. Troque-as antes de disponibilizar a API publicamente.

### Testando a API

Uma collection do Insomnia com todos os endpoints já configurados está disponível em `AquaGuard_Insomnia_Collection.json`. Após importar, execute a requisição de login, copie o token retornado para a variável de ambiente `token`, e as demais requisições já estarão autenticadas automaticamente.

## Testes Automatizados

O projeto possui testes de integração (via `WebApplicationFactory` com banco InMemory) cobrindo o retorno HTTP 200 de cada controller, além de testes unitários da camada de Services validando as regras de detecção de vazamento, consumo excessivo e cálculo de relatórios.

```powershell
dotnet test AquaGuard.sln --configuration Release
```

O workflow [CI](.github/workflows/ci.yml) executa restore, build, todos os testes e build da imagem Docker em pull requests e pushes para `main` e `hml`. Os workflows [CD de produção](.github/workflows/main_aquaguardapi.yml) e [CD de staging](.github/workflows/hml_aquaguardapi-staging.yml) também exigem build, testes e build Docker aprovados antes do deploy, e validam `/health` depois da publicação.

### Ambientes publicados

| Ambiente | Branch | URL |
|---|---|---|
| Staging | `hml` | https://aquaguardapi-staging-e7aeezcqatemaght.canadacentral-01.azurewebsites.net |
| Produção | `main` | https://aquaguardapi-evh6dzbvg4b8d7cd.canadacentral-01.azurewebsites.net |

Após o deploy, valide o health check dos dois ambientes:

```powershell
./scripts/smoke-test.ps1
```

O script falha se algum endpoint `/health` não retornar HTTP 200 e `Healthy`. Os ambientes devem usar configurações e bancos separados; nunca configure staging para gravar no banco de produção.

## Como executar localmente com Docker

Requisitos: Docker Desktop com Docker Compose v2.

1. Crie o arquivo local de configuração a partir do exemplo:

   ```powershell
   Copy-Item .env.example .env
   ```

2. Se desejar, altere os valores de desenvolvimento no `.env`. Esse arquivo está no `.gitignore`; não coloque nele credenciais do Supabase.
3. Suba a API e o PostgreSQL:

   ```powershell
   docker compose up --build -d
   ```

   A API aguarda o banco ficar saudável e aplica as migrations na inicialização porque o Compose habilita explicitamente `Database__MigrateOnStartup`.
4. Confira os logs e acesse o Swagger:

   ```powershell
   docker compose logs -f api
   ```

   Swagger: http://localhost:8080/swagger

   Health check: http://localhost:8080/health

O Compose define uma rede isolada entre API e banco, publica a API na porta `8080`, expõe PostgreSQL localmente na porta `5433` e persiste os dados no volume `postgres_data`. `docker compose down` remove os containers, mas mantém o volume; para apagar também os dados locais use `docker compose down --volumes`.

## Containerização

O [Dockerfile](AquaGuard.API/Dockerfile) usa build multi-stage: restaura/compila/publica com a imagem SDK do .NET 8 e copia somente os artefatos publicados para a imagem ASP.NET Runtime. O container final usa o usuário não-root padrão da imagem. O [docker-compose.yml](docker-compose.yml) coordena API e PostgreSQL, com rede, variáveis de ambiente, health check do banco e volume persistente.

Construir a imagem manualmente:

```powershell
docker build -f AquaGuard.API/Dockerfile -t aquaguard-api:local .
```

## Pipeline CI/CD

- **CI (GitHub Actions):** em pull requests e pushes para `main` e `hml`, restaura dependências, compila em Release, executa a suíte xUnit existente e valida o build da imagem Docker.
- **CD (GitHub Actions + Azure App Service):** `main` publica em produção e `hml` publica em staging. Cada workflow bloqueia o deploy se build, testes ou build Docker falharem e testa o health check após publicar. Os segredos de autenticação do Azure ficam nos GitHub Secrets e as configurações da aplicação ficam no App Service; `ConnectionStrings__DefaultConnection` e `Jwt__Key` não devem ser colocados nos arquivos do repositório.
- **Verificação pós-deploy:** os workflows verificam `/health` automaticamente. Também é possível executar `./scripts/smoke-test.ps1` manualmente para validar os dois ambientes. Guarde capturas dos workflows concluídos e das respostas `/health` como evidências para o PDF/PPT.

O workflow de CI não publica imagens em registry: ele constrói a imagem como validação. O workflow de staging deve estar presente na branch `hml`; ao integrar as alterações deste repositório nessa branch, o CD de staging executará build, testes, deploy e smoke test em sequência.

## Prints do funcionamento

- [GitHub Actions — execuções de CI/CD](https://github.com/strFelix/aquaguard-dotnet-api/actions)
- [Health check de staging](https://aquaguardapi-staging-e7aeezcqatemaght.canadacentral-01.azurewebsites.net/health)
- [Health check de produção](https://aquaguardapi-evh6dzbvg4b8d7cd.canadacentral-01.azurewebsites.net/health)

Os links de health check foram verificados e retornaram `Healthy`. Para a documentação PDF/PPT, capture também uma execução aprovada do pipeline e uma tela do Swagger ou resposta de login em cada ambiente depois dos próximos deploys.

## Diferenciais Implementados

- Repository Pattern com interfaces segregadas por entidade
- Service Layer isolando regras de negócio dos Controllers
- DTOs (ViewModels) totalmente desacoplados dos Models
- Swagger com suporte a autenticação JWT integrada
- Logging estruturado via `ILogger`
- Health Check de conectividade com o banco de dados
- Dockerfile multi-stage e Docker Compose para API + PostgreSQL local
- Response Pattern padronizado (`ApiResponseViewModel`, `ErrorResponseViewModel`)
- Soft Delete em medidores
- Versionamento de rotas preparado para evolução (`/api/v1`)
