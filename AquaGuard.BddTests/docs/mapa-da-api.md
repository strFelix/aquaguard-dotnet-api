# Mapa da API AquaGuard

Contratos conferidos nos controllers, view models, validadores e no Swagger acessível em `http://localhost:5187/swagger/v1/swagger.json`. Os corpos JSON usam camelCase nas respostas. Os request bodies usam propriedades `Email`/`Senha`, `Codigo`/`Localizacao`/`LimiteMensalLitros` ou `MedidorId`/`LitrosConsumidos`/`DataLeitura` (o model binding aceita o casing padrão JSON).

| Método e rota | Autorização | Request | Sucesso | Body de sucesso |
|---|---|---|---|---|
| `POST /api/auth/login` | Público | `{ email, senha }` | 200 | `{ success, message, data: { token, nome, email, role, expiracao } }` |
| `GET /api/medidores?page=&pageSize=` | Autenticado | Query opcional (1, 10) | 200 | Página `{ page, pageSize, totalRecords, totalPages, data: Medidor[] }` |
| `GET /api/medidores/{id}` | Autenticado | ID inteiro | 200 | Medidor direto |
| `POST /api/medidores` | Admin, Operador | `{ codigo, localizacao, limiteMensalLitros }` | 201 | Medidor direto; `Location` via CreatedAtAction |
| `PUT /api/medidores/{id}` | Admin, Operador | mesmos campos da criação | 200 | Medidor direto |
| `DELETE /api/medidores/{id}` | Admin | ID inteiro | 200 | Envelope `{ success, message, data }`; desativação lógica |
| `GET /api/leituras?page=&pageSize=` | Autenticado | Query opcional (1, 10) | 200 | Página com `Leitura[]` |
| `GET /api/leituras/periodo?inicio=&fim=` | Autenticado | `DateTime` ISO 8601 | 200 | Array de leituras direto |
| `GET /api/leituras/medidor/{id}` | Autenticado | ID inteiro | 200 | Array de leituras direto |
| `POST /api/leituras` | Admin, Operador | `{ medidorId, litrosConsumidos, dataLeitura }` | 200 | Leitura direta, com `usuarioId` e `nomeUsuario` |
| `GET /api/alertas?page=&pageSize=` | Autenticado | Query opcional (1, 10) | 200 | Página com `Alerta[]` |
| `GET /api/alertas/pendentes` | Autenticado | — | 200 | Array de alertas direto |
| `PUT /api/alertas/{id}/resolver` | Admin, Operador | ID inteiro na rota | 200 | Alerta direto |
| `GET /api/relatorios/mensal?ano=&mes=` | Autenticado | Ano e mês inteiros | 200 | `{ ano, mes, consumoTotal, mediaDiaria, quantidadeAlertas, economiaPercentual }` |
| `GET /api/relatorios/medidor/{id}` | Autenticado | ID inteiro | 200 | Indicadores do medidor, comparação mensal e tendência |
| `GET /api/dashboard` | Autenticado | — | 200 | Indicadores do dashboard |
| `GET /health` | Público | — | 200 | Health check |

## Erros e validações observados

- Login inválido: 401 com envelope `success=false`, `message` e `data=null`.
- IDs de medidor/alerta inexistentes nos endpoints que verificam existência: 404.
- Validação FluentValidation/model binding: 400. O envelope depende do caminho de erro (validação automática ou middleware).
- Usuário sem role para a operação: 403; challenge de autenticação sem token: 401. O middleware de autorização pode responder sem JSON.
- Leitura aceita litros maiores que zero, data não futura e medidor existente. O endpoint de período não valida a ordem de início/fim.
- O alerta de vazamento exige leitura acima de 150% da média das leituras anteriores nos últimos sete dias. O limite mensal é ultrapassado somente quando o total for maior que o limite configurado.
- A resposta de leitura guarda o nome/ID do usuário, não seu e-mail.
