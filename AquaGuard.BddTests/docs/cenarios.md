# Catálogo de cenários BDD

Os arquivos `.feature` contêm o Gherkin executável completo em português. Este catálogo identifica cada cenário, suas tags e a regra que valida; use os links para consultar o texto integral da feature.

## Disponibilidade — [Health.feature](../Features/Health.feature)

| Cenário | Tags | Validação |
|---|---|---|
| Health check retorna saudável | `@infra`, `@esg-governance` | Positivo: status 200 no health check. |

## Autenticação — [Auth.feature](../Features/Auth.feature)

| Cenário | Tags | Validação |
|---|---|---|
| Login válido retorna token para cada perfil (Admin, Operador e Auditor) | `@esg-governance` | Positivo: status 200 e contrato do token. |
| Senha incorreta é rejeitada | `@negativo` | Negativo: status 401 e corpo de erro. |
| Dados de login inválidos são rejeitados | `@negativo` | Negativo: status 400 e corpo de erro. |

## Medidores — [Medidores.feature](../Features/Medidores.feature)

| Cenário | Tags | Validação |
|---|---|---|
| Listar medidores paginados | `@esg-governance` | Positivo: página e contrato de cada medidor. |
| Criar medidor como Operador | `@limpar` | Positivo: 201 e schema do medidor; limpeza posterior. |
| Criar medidor como Admin | `@limpar` | Positivo: 201 e schema do medidor; limpeza posterior. |
| Consultar e atualizar medidor existente | `@limpar` | Positivo: GET/PUT, status e schemas. |
| Admin desativa medidor sem removê-lo | `@limpar` | Positivo: 200 e confirmação `ativo=false`. |
| Medidor inexistente retorna não encontrado | `@negativo` | Negativo: 404 e contrato de erro. |
| Auditor não pode criar medidor | `@negativo`, `@rbac` | Negativo: 403. |
| Medidor com limite inválido é rejeitado | `@negativo` | Negativo: 400 e contrato de erro. |
| Operador não pode desativar medidor | `@negativo`, `@rbac`, `@limpar` | Negativo: 403; limpeza posterior pelo Admin. |
| Consulta de medidor sem token é negada | `@negativo` | Negativo: 401. |

## Leituras — [Leituras.feature](../Features/Leituras.feature)

| Cenário | Tags | Validação |
|---|---|---|
| Listar leituras paginadas | `@esg-environmental` | Positivo: paginação e schema das leituras. |
| Consultar leituras por período | `@esg-environmental` | Positivo: 200 e schema do array. |
| Período com fim anterior ao início deve ser rejeitado | `@negativo`, `@bug-api-periodo-invertido` | Negativo esperado: 400; pendente por ausência de validação na API. |
| Consultar leituras de um medidor | `@limpar` | Positivo: 200 e schema do array. |
| Leitura registra o usuário responsável | `@limpar`, `@esg-governance`, `@rastreabilidade` | Positivo: schema, ID/nome não vazios do usuário real e ausência de alerta indevido. |
| Leitura acima de 150 por cento da média gera alerta de vazamento | `@limpar`, `@vazamento` | Ambiental: duas leituras geram alerta pendente com percentual. |
| Consumo mensal acima do limite gera alerta | `@limpar`, `@consumo-excessivo` | Ambiental: 600 L excedem limite de 500 L. |
| Auditor não pode registrar leitura | `@negativo`, `@rbac`, `@limpar` | Negativo: 403. |
| Leitura com zero litros é rejeitada | `@negativo`, `@limpar` | Negativo: validação 400. |
| Leitura de medidor inexistente é rejeitada | `@negativo` | Negativo: validação 400. |
| Listagem de leituras sem token é negada | `@negativo` | Negativo: 401. |

## Alertas — [Alertas.feature](../Features/Alertas.feature)

| Cenário | Tags | Validação |
|---|---|---|
| Listar alertas paginados | `@esg-governance` | Positivo: paginação e contrato dos alertas. |
| Listar alertas pendentes | `@esg-governance` | Positivo: array e schema. |
| Operador resolve alerta pendente | `@limpar` | Positivo: alerta criado na execução fica resolvido. |
| Auditor não pode resolver alerta | `@negativo`, `@rbac`, `@limpar` | Negativo: 403. |
| Resolver alerta inexistente retorna não encontrado | `@negativo` | Negativo: 404 e schema de erro. |

## Relatórios — [Relatorios.feature](../Features/Relatorios.feature)

| Cenário | Tags | Validação |
|---|---|---|
| Relatório mensal apresenta indicadores | `@esg-environmental`, `@esg-social` | Positivo: 200, schema e economia percentual. |
| Mês inválido é rejeitado | `@negativo` | Negativo: 400 e schema de erro. |
| Relatório por medidor compara consumo mensal | `@limpar`, `@esg-environmental`, `@esg-social` | Positivo: campos de economia e diferença em litros. |
| Relatório de medidor inexistente retorna não encontrado | `@negativo` | Negativo: 404 e schema de erro. |

## Dashboard — [Dashboard.feature](../Features/Dashboard.feature)

| Cenário | Tags | Validação |
|---|---|---|
| Cada perfil autenticado consulta o dashboard (Admin, Operador e Auditor) | `@esg-social`, `@esg-environmental` | Positivo: acesso e schema por perfil. |
| Dashboard sem token é negado | `@negativo` | Negativo: 401. |

## Evidências para o relatório final

1. Estrutura `AquaGuard.BddTests` aberta na IDE.
2. `Leituras.feature` e `Medidores.feature` abertos.
3. Um schema JSON.
4. `.env.example`, `Dockerfile` e `docker-compose.yml`, sem exibir `.env`.
5. Terminal com os cenários passando.
6. Relatório HTML em `test-results/`.
7. Execução via Docker, se utilizada.
8. Um cenário negativo passando (401, 403 ou 400).
