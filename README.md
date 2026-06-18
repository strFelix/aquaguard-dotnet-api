# AquaGuard API

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
| Banco de dados | SQL Server (LocalDB em desenvolvimento) |
| Autenticação | JWT Bearer |
| Hashing de senha | BCrypt.Net |
| Validação | FluentValidation |
| Documentação | Swagger / OpenAPI |
| Testes | xUnit + EF Core InMemory Provider |
| Containerização | Docker |
| Health Check | AspNetCore.HealthChecks.SqlServer |

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
- SQL Server (LocalDB ou instância completa)
- Visual Studio 2022 ou superior

### Configuração

1. Clone o repositório
2. Ajuste a connection string em `appsettings.json`, se necessário:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AquaGuardDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

3. Aplique as migrations:

```powershell
Add-Migration InitialCreate
Update-Database
```

4. Execute o projeto (`F5` no Visual Studio ou `dotnet run`)
5. Acesse o Swagger em `https://localhost:{porta}/swagger`

### Usuários de Seed

| Email | Senha | Role |
|---|---|---|
| admin@aquaguard.com | Admin@123 | Admin |
| operador@aquaguard.com | Operador@123 | Operador |
| auditor@aquaguard.com | Auditor@123 | Auditor |

### Testando a API

Uma collection do Insomnia com todos os endpoints já configurados está disponível em `AquaGuard_Insomnia_Collection.json`. Após importar, execute a requisição de login, copie o token retornado para a variável de ambiente `token`, e as demais requisições já estarão autenticadas automaticamente.

## Testes Automatizados

O projeto possui testes de integração (via `WebApplicationFactory` com banco InMemory) cobrindo o retorno HTTP 200 de cada controller, além de testes unitários da camada de Services validando as regras de detecção de vazamento, consumo excessivo e cálculo de relatórios.

```powershell
dotnet test
```

## Diferenciais Implementados

- Repository Pattern com interfaces segregadas por entidade
- Service Layer isolando regras de negócio dos Controllers
- DTOs (ViewModels) totalmente desacoplados dos Models
- Swagger com suporte a autenticação JWT integrada
- Logging estruturado via `ILogger`
- Health Check de conectividade com o banco de dados
- Dockerfile funcional para containerização
- Response Pattern padronizado (`ApiResponseViewModel`, `ErrorResponseViewModel`)
- Soft Delete em medidores
- Versionamento de rotas preparado para evolução (`/api/v1`)
