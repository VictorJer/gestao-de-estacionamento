# SDD — Fundação local e CI (Fase 1)

**Estado:** aprovado pelo solicitante em 10/11/2026 para avançar ao PR; não aprova produção nem Azure. **Escopo:** `feature/fundacao-infraestrutura-fase-1`, sem provisionamento ou publicação Azure. Referências: [`PRD`](../prd/PRD.md), [`TDD`](../tdd/tdd.md), [`OpenSpec`](../../openspec/changes/fundacao-infraestrutura-fase-1/), [evidência local](../evidence/fase-1-local.md), [pendências](../decisions/fase-1.md).

## Problema, fronteiras e rastreabilidade

A aplicação ainda não possuía serviços executáveis ou verificação da autenticação e do acesso por mesma origem. Esta solução entrega fundação da sessão de funcionários e edge local/CI, não operações de estacionamento. O PRD revisado é a fonte de IDs (o TDD anterior usa referências RF históricas divergentes). Não implementar RF-01–09, RF-10, RF-12 nem operações administrativas reais nesta fase.

| PRD vigente | TDD/decisão | Spec OpenSpec | Solução e verificação | Limite |
| --- | --- | --- | --- | --- |
| RF-11, RNF-01 | DT-06, fase 1 | `identity/employee-session` | Identity em SQL Server, três papéis, fallback deny-by-default, testes HTTP `401`/`403`, usuário provisionado e login real no Playwright | Matriz operacional completa e separação de aprovador/solicitante pertencem às próximas fases P0 |
| RNF-03 | DT-05/06 | `identity/employee-session` | Cookie HttpOnly/Secure/host-only, CSRF + Origin em mutações, nenhum dado de cliente no DTO da sessão; checks de log sem credenciais | Base legal, finalidade, acesso e retenção pendentes; dados sintéticos apenas |
| RNF-05 | DT-07/11, fase 1 | `platform/edge-api-access` | API isolada na rede interna do Compose, proxy HTTPS `/api` de mesma origem, SQL real e key ring persistente; teste de sessão após reinício | Sem meta de disponibilidade/backup aprovada nem verificação Azure |
| RNF-06 | DT-10/11 | ambas | CI preparado, testes de contrato HTTP/UI/E2E e relatório local sem tokens ou credenciais | Telemetria operacional e alertas ainda não definidos |

## Componentes e fluxo

- `frontend/`: Angular 22 standalone/zoneless; Signal Forms para login. `SessionService` consulta `/api/v1/session`; interceptor busca antiforgery novo imediatamente antes de cada mutação, inclusive depois do login. Erros `401` de consulta inicial significam visitante; `401` após sessão ativa mostra mensagem de expiração. Feedback em `role="alert"`; rótulos, foco e acionamento via teclado testados.
- `infra/nginx.conf` + `compose.yaml`: apenas o edge escuta em `127.0.0.1:8444` com HTTPS local. Nginx encaminha `/api/` para `https://api:8443`, verifica o certificado local e substitui os forwarded headers; descarta cabeçalhos de identidade indicados. Serviços API e SQL na rede Docker interna; sem portas publicadas. Origem permitida `https://localhost:8444`; somente IP do edge consta de `KnownProxies`. Não inferir confiança na topologia de Azure a partir da rede local.
- `backend/src/Estacionamento.Api/`: endpoints GET token/login/logout/GET sessão versionados, autenticação Identity por cookie no caminho `/api`, fallback de autorização e políticas manobrista/caixa/administrador. Mutação exige HTTPS, `Origin` exata e token antiforgery. Retorno de sessão mínimo e `no-store`; endpoint administrativo de verificação só em Development. Ausência de autorregistro.
- `backend/src/Estacionamento.Infrastructure/Identity/`: entidades/migração EF Core SQL Server versionada; provisionador separado restrito a Development. Migrações automáticas apenas quando `Database:ApplyMigrations=true` em Development; integração MSTest usa banco exclusivo e descartável. Segredos locais em `.env` e `local-secrets/`, ignorados por Git e Docker build context. Key ring persistente em volume de desenvolvimento; retenção, backup e rotação são gates futuros.

## Cenários e evidências

`dotnet test backend/Estacionamento.slnx` verifica SQL Server real, login, cookie, expiração, logout, autorização e CSRF. `npm test -- --watch=false` (frontend) verifica UI/estado/interceptor; `npm test` (Playwright) verifica teclado e, quando `ESTACIONAMENTO_EDGE_URL` e credenciais de teste estão definidos, navegador real com SQL/HTTPS/proxy, falha CSRF, persistência após reinício e ausência de cookie de terceiros. Resultado local: 4/4 Playwright; detalhes e headers sanitizados em `docs/evidence/fase-1-local.md`. CI em `.github/workflows/fase-1.yml` depende do segredo de repositório `FASE1_SQL_TEST_PASSWORD`; a run 43467964243 do PR #1 passou em 13/11/2026, inclusive provisionamento e Playwright. Essa validação não substitui os gates de produção.

## Riscos, operação e aceite

O certificado autoassinado é apenas de desenvolvimento; a aplicação não prova ingress Azure nem gestão de chaves. Reinício não é teste de restore. A API em Compose só deve ser usada com dados sintéticos e `.env` fora do Git. Publicação Azure está reservada à fase 7 após P0 e P1. O checklist de `docs/decisions/fase-1.md` está integralmente pendente e impede liberação produtiva. Rollback de app preserva volumes/dados e exige compatibilidade de esquema; não apagar registros para reverter versão.

**Revisão solicitada:** confirmar rastreabilidade, arquitetura, limites e evidências. Registrar nome/data/decisão aqui antes do PR; aprovação deste SDD não aprova implantação, política de dados nem controle Azure.

**Aprovação do SDD:** solicitante aprovou em 10/11/2026 nesta conversa, exclusivamente para avançar ao PR. Gates de produção permanecem pendentes.
