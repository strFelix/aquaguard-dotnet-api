# language: pt
@infra @esg-governance
Funcionalidade: Disponibilidade da API
  Como equipe de operação
  Quero verificar o health check
  Para confirmar a disponibilidade da aplicação e do banco

  Cenário: Health check retorna saudável
    Quando envio GET para "/health"
    Então a resposta deve ter status 200

