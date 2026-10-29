# language: pt
@esg-environmental
Funcionalidade: Leituras de consumo
  Como operador de recursos hídricos
  Quero registrar e consultar leituras
  Para identificar padrões e desperdícios

  Cenário: Listar leituras paginadas
    Dado que estou autenticado como "Operador"
    Quando envio GET para "/api/leituras?page=1&pageSize=10"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "paginado-leitura"

  Cenário: Consultar leituras por período
    Dado que estou autenticado como "Auditor"
    Quando envio GET para "/api/leituras/periodo?inicio=2020-01-01T00:00:00Z&fim=2030-01-01T00:00:00Z"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "leituras-array"

  @negativo @bug-api-periodo-invertido
  Cenário: Período com fim anterior ao início deve ser rejeitado
    Dado que estou autenticado como "Auditor"
    Quando envio GET para "/api/leituras/periodo?inicio=2030-01-01T00:00:00Z&fim=2020-01-01T00:00:00Z"
    Então a resposta deve ter status 400
    E o corpo deve ser válido contra o schema "erro"

  @limpar
  Cenário: Consultar leituras de um medidor
    Dado que estou autenticado como "Operador"
    E que existe um medidor criado para o cenário
    Quando envio GET para "/api/leituras/medidor/__MEDIDOR_ID__"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "leituras-array"

  @limpar @esg-governance @rastreabilidade
  Cenário: Leitura registra o usuário responsável
    Dado que estou autenticado como "Operador"
    E que existe um medidor criado para o cenário
    Quando registro uma leitura de 100 litros nesse medidor
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "leitura"
    E a leitura deve identificar o usuário autenticado
    E não deve existir alerta pendente para o medidor do cenário

  @limpar @vazamento
  Cenário: Leitura acima de 150 por cento da média gera alerta de vazamento
    Dado que estou autenticado como "Operador"
    E que existe um medidor criado para o cenário
    Quando registro uma leitura de 100 litros nesse medidor
    E registro uma leitura de 250 litros nesse medidor
    Então a resposta deve ter status 200
    E deve existir um alerta pendente do tipo "Vazamento" para o medidor do cenário
    E a descrição do alerta deve conter o percentual de desvio

  @limpar @consumo-excessivo
  Cenário: Consumo mensal acima do limite gera alerta
    Dado que estou autenticado como "Operador"
    E que existe um medidor com limite mensal de 500 litros
    Quando registro uma leitura de 300 litros nesse medidor
    E registro uma leitura de 300 litros nesse medidor
    Então a resposta deve ter status 200
    E deve existir um alerta pendente do tipo "ConsumoExcessivo" para o medidor do cenário

  @negativo @rbac
  @limpar
  Cenário: Auditor não pode registrar leitura
    Dado que estou autenticado como "Auditor"
    E que existe um medidor criado para o cenário
    Quando registro uma leitura de 100 litros nesse medidor
    Então a resposta deve ter status 403

  @negativo
  @limpar
  Cenário: Leitura com zero litros é rejeitada
    Dado que estou autenticado como "Operador"
    E que existe um medidor criado para o cenário
    Quando envio POST para "/api/leituras" com o corpo:
      """
      { "MedidorId": __MEDIDOR_ID__, "LitrosConsumidos": 0, "DataLeitura": "__NOW_UTC__" }
      """
    Então a resposta deve ter status 400
    E o corpo deve ser válido contra o schema "erro"

  @negativo
  Cenário: Leitura de medidor inexistente é rejeitada
    Dado que estou autenticado como "Operador"
    Quando envio POST para "/api/leituras" com o corpo:
      """
      { "MedidorId": 2147483647, "LitrosConsumidos": 100, "DataLeitura": "__NOW_UTC__" }
      """
    Então a resposta deve ter status 400
    E o corpo deve ser válido contra o schema "erro"

  @negativo
  Cenário: Listagem de leituras sem token é negada
    Dado que não estou autenticado
    Quando envio GET para "/api/leituras?page=1&pageSize=10"
    Então a resposta deve ter status 401
