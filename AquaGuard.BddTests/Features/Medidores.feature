# language: pt
@esg-governance
Funcionalidade: Gestão de medidores
  Como usuário autorizado
  Quero consultar e administrar medidores
  Para acompanhar o consumo de água

  Cenário: Listar medidores paginados
    Dado que estou autenticado como "Admin"
    Quando envio GET para "/api/medidores?page=1&pageSize=10"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "paginado-medidor"

  @limpar
  Cenário: Criar medidor como Operador
    Dado que estou autenticado como "Operador"
    Quando crio um medidor exclusivo para o cenário
    Então a resposta deve ter status 201
    E a resposta deve incluir o cabeçalho Location
    E o corpo deve ser válido contra o schema "medidor"

  @limpar
  Cenário: Criar medidor como Admin
    Dado que estou autenticado como "Admin"
    Quando crio um medidor exclusivo para o cenário
    Então a resposta deve ter status 201
    E a resposta deve incluir o cabeçalho Location
    E o corpo deve ser válido contra o schema "medidor"

  @limpar
  Cenário: Consultar e atualizar medidor existente
    Dado que estou autenticado como "Operador"
    E que existe um medidor criado para o cenário
    Quando busco o medidor criado para o cenário
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "medidor"
    Quando atualizo o medidor criado para o cenário
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "medidor"

  @limpar
  Cenário: Admin desativa medidor sem removê-lo
    Dado que estou autenticado como "Admin"
    E que existe um medidor criado para o cenário
    Quando desativo o medidor criado para o cenário
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "api-response"
    Quando busco o medidor criado para o cenário
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "medidor"
    E o campo "ativo" deve ser "false"

  @negativo
  Cenário: Medidor inexistente retorna não encontrado
    Dado que estou autenticado como "Admin"
    Quando envio GET para "/api/medidores/2147483647"
    Então a resposta deve ter status 404
    E o corpo deve ser válido contra o schema "erro"

  @negativo @rbac
  Cenário: Auditor não pode criar medidor
    Dado que estou autenticado como "Auditor"
    Quando envio POST para "/api/medidores" com o corpo:
      """
      { "Codigo": "BDD-NEG-01", "Localizacao": "Teste", "LimiteMensalLitros": 500 }
      """
    Então a resposta deve ter status 403

  @negativo
  Cenário: Medidor com limite inválido é rejeitado
    Dado que estou autenticado como "Operador"
    Quando envio POST para "/api/medidores" com o corpo:
      """
      { "Codigo": "BDD-INVALIDO", "Localizacao": "Teste", "LimiteMensalLitros": -1 }
      """
    Então a resposta deve ter status 400
    E o corpo deve ser válido contra o schema "erro"

  @negativo @rbac
  @limpar
  Cenário: Operador não pode desativar medidor
    Dado que estou autenticado como "Operador"
    E que existe um medidor criado para o cenário
    Quando envio DELETE para "/api/medidores/__MEDIDOR_ID__"
    Então a resposta deve ter status 403

  @negativo
  Cenário: Consulta de medidor sem token é negada
    Dado que não estou autenticado
    Quando envio GET para "/api/medidores?page=1&pageSize=10"
    Então a resposta deve ter status 401
