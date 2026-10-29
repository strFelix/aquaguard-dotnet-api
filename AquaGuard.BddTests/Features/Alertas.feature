# language: pt
@esg-governance
Funcionalidade: Alertas de consumo
  Como responsável pelo monitoramento
  Quero consultar e resolver alertas
  Para acompanhar eventos que exigem ação

  Cenário: Listar alertas paginados
    Dado que estou autenticado como "Auditor"
    Quando envio GET para "/api/alertas?page=1&pageSize=10"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "paginado-alerta"

  Cenário: Listar alertas pendentes
    Dado que estou autenticado como "Operador"
    Quando envio GET para "/api/alertas/pendentes"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "alertas-array"

  @limpar
  Cenário: Operador resolve alerta pendente
    Dado que foi gerado um alerta de vazamento para o cenário
    Quando resolvo o alerta criado para o cenário
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "alerta"
    E o alerta do cenário está resolvido

  @negativo @rbac
  @limpar
  Cenário: Auditor não pode resolver alerta
    Dado que foi gerado um alerta de vazamento para o cenário
    Quando tento resolver o alerta criado como "Auditor"
    Então a resposta deve ter status 403

  @negativo
  Cenário: Resolver alerta inexistente retorna não encontrado
    Dado que estou autenticado como "Operador"
    Quando tento resolver um alerta inexistente como "Operador"
    Então a resposta deve ter status 404
    E o corpo deve ser válido contra o schema "erro"
