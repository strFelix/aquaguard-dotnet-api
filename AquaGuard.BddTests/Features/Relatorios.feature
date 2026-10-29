# language: pt
@esg-environmental @esg-social
Funcionalidade: Relatórios de consumo e economia
  Como auditor ou gestor
  Quero consultar indicadores de consumo e economia
  Para avaliar a preservação dos recursos hídricos

  Cenário: Relatório mensal apresenta indicadores
    Dado que estou autenticado como "Auditor"
    Quando envio GET para "/api/relatorios/mensal?ano=__CURRENT_YEAR__&mes=__CURRENT_MONTH__"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "relatorio-mensal"
    E a resposta deve conter o campo "economiaPercentual"

  @negativo
  Cenário: Mês inválido é rejeitado
    Dado que estou autenticado como "Admin"
    Quando envio GET para "/api/relatorios/mensal?ano=__CURRENT_YEAR__&mes=13"
    Então a resposta deve ter status 400
    E o corpo deve ser válido contra o schema "erro"

  @limpar
  Cenário: Relatório por medidor compara consumo mensal
    Dado que estou autenticado como "Operador"
    E que existe um medidor criado para o cenário
    Quando envio GET para "/api/relatorios/medidor/__MEDIDOR_ID__"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "relatorio-medidor"
    E a resposta deve conter o campo "economiaPercentual"
    E a resposta deve conter o campo "diferencaLitros"

  @negativo
  Cenário: Relatório de medidor inexistente retorna não encontrado
    Dado que estou autenticado como "Auditor"
    Quando envio GET para "/api/relatorios/medidor/2147483647"
    Então a resposta deve ter status 404
    E o corpo deve ser válido contra o schema "erro"
