# language: pt
@esg-social @esg-environmental
Funcionalidade: Indicadores do dashboard
  Como usuário da plataforma
  Quero consultar os indicadores gerais de consumo
  Para acompanhar a sustentabilidade hídrica

  Esquema do Cenário: Cada perfil autenticado consulta o dashboard
    Dado que estou autenticado como "<perfil>"
    Quando envio GET para "/api/dashboard"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "dashboard"

    Exemplos:
      | perfil   |
      | Admin    |
      | Operador |
      | Auditor  |

  @negativo
  Cenário: Dashboard sem token é negado
    Dado que não estou autenticado
    Quando envio GET para "/api/dashboard"
    Então a resposta deve ter status 401
