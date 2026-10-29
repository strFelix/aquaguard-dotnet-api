# Bugs e divergências observados

## Período de leitura aceitava `fim` anterior a `inicio` — corrigido

- **Endpoint:** `GET /api/leituras/periodo?inicio=&fim=`
- **Esperado pelo guia:** `400 Bad Request` para um intervalo invertido.
- **Evidência:** a execução BDD retornou `200` e `[]` para um intervalo invertido.
- **Correção:** o controller agora retorna `400` e uma mensagem de validação antes de consultar o serviço; o cenário `@bug-api-periodo-invertido` em [Leituras.feature](../Features/Leituras.feature) preserva o contrato esperado.
- **Estado:** corrigido e validado pela suíte BDD completa em instância local da API.

## Validação FluentValidation assíncrona falhava para medidores e leituras — corrigido

- **Causa:** `Program.cs` registrava a integração síncrona mantida `FluentValidation.AspNetCore` em paralelo com SharpGrip. A primeira rejeitava regras `MustAsync`, fazendo `POST /api/medidores` responder 400 com erro interno de configuração para payloads válidos.
- **Impacto observado:** criação de medidores e cenários dependentes falhavam, inclusive leituras e alertas.
- **Correção:** removido o registro duplicado/síncrono e a dependência antiga; mantida a integração assíncrona `SharpGrip.FluentValidation.AutoValidation.Mvc` com `FluentValidation.DependencyInjectionExtensions` 11.11.0.
- **Estado:** corrigido e validado pela suíte BDD completa em instância local da API.

## Resposta da leitura criada omitia o nome do usuário — corrigido

- **Endpoint:** `POST /api/leituras`
- **Evidência:** o cenário de rastreabilidade recebeu `usuarioId` válido, mas `nomeUsuario` vazio. A consulta do registro recém-criado usava `FindAsync`, sem carregar a navegação `Usuario`.
- **Correção:** o repositório agora tem uma consulta detalhada que inclui medidor e usuário; as consultas de leituras por medidor/período também carregam usuário para preencher o DTO.
- **Estado:** corrigido e validado pela suíte BDD completa em instância local da API.
