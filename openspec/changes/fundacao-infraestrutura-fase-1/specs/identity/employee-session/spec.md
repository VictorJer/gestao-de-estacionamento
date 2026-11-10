# Spec Delta

## Purpose

Permitir que funcionários previamente habilitados acessem a aplicação com sessão verificável, limites de perfil e proteção contra ações não autorizadas na origem web.

## ADDED Requirements

### Requirement: Autenticação restrita a funcionários provisionados
O sistema SHALL autenticar apenas funcionários previamente provisionados e ativos, sem oferecer autorregistro público.

#### Scenario: Login válido
- **WHEN** um funcionário ativo envia credenciais válidas
- **THEN** o sistema inicia uma sessão vinculada a esse funcionário e retorna somente os dados de sessão necessários à interface

#### Scenario: Login inválido ou usuário inativo
- **WHEN** credenciais inválidas ou de funcionário inativo são apresentadas
- **THEN** o sistema nega o login sem revelar se a conta existe

### Requirement: Sessão de primeira parte revogável
O sistema SHALL manter a sessão do funcionário em cookie de primeira parte, HttpOnly e Secure, e SHALL invalidá-la no logout e na expiração.

#### Scenario: Consulta à sessão ativa
- **WHEN** um funcionário com sessão válida consulta o estado da sessão
- **THEN** o sistema informa identidade mínima e papel autorizado, sem expor credenciais nem dados pessoais de clientes

#### Scenario: Logout ou expiração
- **WHEN** o funcionário encerra a sessão ou a sessão expira
- **THEN** chamadas posteriores a recursos protegidos são negadas como não autenticadas

### Requirement: Autorização no servidor por perfil
O sistema SHALL reconhecer apenas os perfis manobrista, caixa e administrador e SHALL negar por padrão o acesso a operações protegidas não permitidas ao papel do funcionário, independentemente da navegação da interface.

#### Scenario: Perfil insuficiente
- **WHEN** um funcionário autenticado solicita uma operação protegida sem permissão para seu perfil
- **THEN** a API nega a operação sem executar alteração nem divulgar dados restritos

#### Scenario: Ausência de sessão
- **WHEN** uma operação protegida é solicitada sem sessão válida
- **THEN** a API responde como não autenticada, sem executar a operação

### Requirement: Proteção de ações mutáveis contra CSRF
O sistema SHALL emitir um mecanismo de antiforgery na própria origem da aplicação e SHALL exigir sua validação e a origem esperada em todas as requisições mutáveis, incluindo login e logout.

#### Scenario: Mutação com proteção válida
- **WHEN** o navegador envia uma ação mutável com prova antiforgery válida e origem permitida
- **THEN** o sistema pode prosseguir para as demais validações de autenticação e autorização

#### Scenario: Mutação sem proteção ou com origem inválida
- **WHEN** uma ação mutável, inclusive login ou logout, chega sem prova antiforgery válida ou de origem não permitida
- **THEN** o sistema a rejeita sem realizar a alteração solicitada
