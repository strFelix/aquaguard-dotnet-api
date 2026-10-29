# language: pt
@esg-governance
Funcionalidade: Autenticação por perfil
  Como consumidor da API
  Quero autenticar os perfis disponíveis
  Para acessar recursos conforme as permissões

  Esquema do Cenário: Login válido retorna token para cada perfil
    Quando faço login como "<perfil>"
    Então a resposta deve ter status 200
    E o corpo deve ser válido contra o schema "login"

    Exemplos:
      | perfil   |
      | Admin    |
      | Operador |
      | Auditor  |

  @negativo
  Cenário: Senha incorreta é rejeitada
    Quando envio POST para "/api/auth/login" com o corpo:
      """
      { "Email": "admin@aquaguard.com", "Senha": "senha-incorreta" }
      """
    Então a resposta deve ter status 401
    E o corpo deve ser válido contra o schema "erro"

  @negativo
  Cenário: Dados de login inválidos são rejeitados
    Quando envio POST para "/api/auth/login" com o corpo:
      """
      { "Email": "", "Senha": "" }
      """
    Então a resposta deve ter status 400
    E o corpo deve ser válido contra o schema "erro"
