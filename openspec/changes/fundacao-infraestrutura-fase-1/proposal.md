# Proposal

## Why

O projeto ainda contém apenas documentação; sem fundação executável não é possível iniciar as jornadas P0 nem verificar localmente a sessão de funcionários e o acesso da SPA à API pela mesma origem. A Fase 1 do TDD propõe também infraestrutura Azure, mas esta mudança limita a entrega à fundação local/CI; seus números de RF não coincidem com o PRD revisado, que é a referência de comportamento.

## What Changes

- Preparar a solução ASP.NET Core/.NET 10 e o aplicativo Angular 22, com versões compatíveis fixadas, testes iniciais e pipeline de CI.
- Disponibilizar em ambiente local/CI uma SPA que chama a API via `/api` na própria origem, com proxy para API em serviço separado e acesso direto do navegador à API restrito nesse ambiente.
- Estabelecer autenticação de funcionários provisionados previamente, login/logout/estado da sessão, papéis fixos, autorização básica no servidor e proteção antiforgery das ações mutáveis, inclusive login/logout.
- Configurar ambiente de desenvolvimento e testes/CI com SQL Server, migrações iniciais de identidade, proteção de segredos/chaves de desenvolvimento e testes automatizados em navegador real; manter configurações portáveis para implantação futura.
- Registrar responsáveis e decisões pendentes (fuso, moeda, volumes, domínios, política de dados, backup/continuidade) como gates de implantação, sem tratá-los como aprovação de produção.
- **Fora do escopo:** publicação, provisionamento de recursos ou validação da topologia na Azure; homologação/produção na nuvem; check-in, vagas, tarifas, retirada, cobrança, pagamento, entrega, relatórios e cadastro operacional completo. A Fase 1 não antecipa a aprovação das regras comerciais nem de privacidade.

## Capabilities

### New Capabilities

- `identity/employee-session`: acesso à sessão de funcionários, autenticação, logout, perfis e proteção contra CSRF nas operações autenticadas e de sessão.
- `platform/edge-api-access`: acesso do navegador à API por proxy de mesma origem e bloqueio de bypass no ambiente local/CI; topologia Azure fica para validação futura.

### Modified Capabilities

Nenhuma: não há especificações principais existentes.

## Impact

Novos `frontend/`, `backend/`, `tests/e2e/` e configuração de CI local; endpoints versionados `/api/v1` para sessão/antiforgery, Identity/EF Core e banco SQL Server, proxy `/api` em ambiente reproduzível. Critérios de RF-11, RNF-01/03/05/06 do PRD vigente são atendidos parcialmente nesta fase; jornadas RF-01–09/12 continuam em fases posteriores. Publicação e segurança da topologia Azure permanecem pendentes, sem aceite de produção. Antes de abrir PR, elaborar SDD rastreável ao PRD/TDD e solicitar revisão; não integrar à `main` antes de aprovações e verificações exigidas.
