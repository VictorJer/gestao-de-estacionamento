# Spec Delta

## Purpose

Garantir que o navegador interaja com a API somente pela origem da aplicação, mantendo sessão de primeira parte quando SPA e API executam como serviços separados no ambiente local/CI.

## ADDED Requirements

### Requirement: API disponível na origem da SPA
O sistema SHALL oferecer a API ao navegador no caminho `/api` da mesma origem da SPA, preservando o encaminhamento das rotas versionadas e dos dados necessários à sessão e à proteção antiforgery.

#### Scenario: Acesso à sessão via edge
- **WHEN** o navegador da SPA solicita `/api/v1` por HTTPS com os dados de sessão necessários
- **THEN** a resposta da API chega pela origem da SPA sem chamada direta do navegador ao domínio de hospedagem da API

#### Scenario: Cookies e antiforgery retornam ao navegador
- **WHEN** o login ou a emissão da proteção antiforgery retorna pelo caminho `/api`
- **THEN** o navegador recebe cookies e cabeçalhos necessários sem cookie cross-site nem exposição do cookie de sessão a scripts

### Requirement: API não aceita bypass do proxy pelo navegador no ambiente local/CI
No ambiente de teste reproduzível, o sistema SHALL restringir o ingresso direto do navegador à API e SHALL aceitar tráfego externo somente pelo proxy configurado, sem confiar em cabeçalhos de identidade fornecidos pelo cliente.

#### Scenario: Requisição direta ao serviço de API
- **WHEN** um navegador tenta acessar diretamente o serviço de API no ambiente local/CI
- **THEN** o acesso é negado antes de executar uma operação de negócio ou de sessão

#### Scenario: Cabeçalho de identidade forjado
- **WHEN** uma requisição encaminhada contém cabeçalho de identidade arbitrário enviado pelo cliente sem sessão válida
- **THEN** a API não autentica o cliente com base nesse cabeçalho

### Requirement: Tráfego seguro e respostas privadas
O sistema SHALL usar HTTPS no acesso do navegador ao proxy e no encaminhamento deste à API no ambiente de teste reproduzível e SHALL impedir armazenamento em cache compartilhado de respostas de sessão ou com dados privados.

#### Scenario: Resposta de sessão não armazenável
- **WHEN** o navegador consulta o estado da própria sessão por `/api`
- **THEN** a resposta contém diretivas que impedem cache compartilhado de dados privados

#### Scenario: Transporte inseguro
- **WHEN** uma tentativa de acessar a API pela borda usa conexão insegura
- **THEN** ela não é processada como operação autenticada em transporte inseguro
