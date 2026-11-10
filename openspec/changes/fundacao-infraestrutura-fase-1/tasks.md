# Tasks

## 1. Preparação técnica e pendências de implantação futura

- [x] 1.1 Registrar responsáveis e pendências futuras de domínios/topologia Azure, fuso, moeda e volumes em documento de decisões; verificar que itens não aprovados continuam pendentes e não bloqueiam os testes locais.
- [x] 1.2 Fixar versões compatíveis de SDK .NET 10, Angular 22/Node, EF Core SQL Server, Tailwind, Lucide, MSTest, Vitest e Playwright em manifests/lockfiles; verificar restore/instalação reproduzíveis e compatibilidade de Signal Forms e Zoneless.
- [x] 1.3 Criar `backend/Estacionamento.slnx` com Api/Application/Domain/Infrastructure e projetos MSTest, mantendo direção das referências do design; verificar `dotnet build` e `dotnet test` básicos.
- [x] 1.4 Criar `frontend/` com Angular standalone Zoneless, estilos Tailwind/Lucide e testes Vitest iniciais, mais `tests/e2e/` com Playwright; verificar build, teste unitário e E2E de inicialização da SPA.

## 2. Identidade, sessão e autorização

- [x] 2.1 Configurar Identity/EF Core no SQL Server local/CI com migração versionada de usuários/papéis e criação controlada de contas de teste sem senha padrão; verificar migração em SQL Server real e testes de conta ativa/inativa e ausência de autorregistro.
- [x] 2.2 Implementar endpoints versionados de token antiforgery, login, logout e estado de sessão, com cookie host-only HttpOnly/Secure e contratos OpenAPI mínimos; verificar testes HTTP de login válido/inválido, sessão, expiração, logout, atributos do cookie e `Cache-Control: no-store`.
- [x] 2.3 Aplicar validação de token antiforgery e `Origin` em todas as mutações, inclusive login/logout; verificar testes HTTP de token ausente, inválido, origem incorreta e requisição legítima após bootstrap.
- [x] 2.4 Configurar políticas deny-by-default para manobrista, caixa e administrador e endpoint protegido de verificação apenas em ambiente de teste local/CI; verificar testes HTTP `401`/`403` e que cabeçalhos de identidade forjados não concedem acesso.
- [x] 2.5 Integrar UI de login/logout e estado de sessão via `/api/v1`, interceptor CSRF e estados textuais de erro acessíveis por teclado; verificar Vitest de estados/expiração e Playwright de login/logout e navegação por teclado.

## 3. Edge, rede e segurança de transporte

- [x] 3.1 Configurar proxy reverso `/api` na origem da SPA e API em serviço separado na rede local/CI, preservando cookies, paths e antiforgery e aceitando forwarded headers só do proxy conhecido; verificar teste de navegador de mesma origem.
- [x] 3.2 Manter porta da API não publicada ao navegador, usar HTTPS local entre navegador/proxy/API e desabilitar cache compartilhado de sessão; verificar no ambiente de teste acesso direto negado, cabeçalho forjado ineficaz, TLS e `no-store`.
- [x] 3.3 Persistir key ring Data Protection e manter segredos de desenvolvimento/teste fora do Git; verificar sessão após reiniciar API e ausência de cookies, tokens e credenciais nos logs, sem alegar rotação ou persistência gerenciada na Azure.
- [x] 3.4 Executar Playwright em navegador real no ambiente local/CI para bootstrap CSRF, login, sessão, logout e falhas de CSRF, comprovando ausência de cookies de terceiros; anexar resultado e configuração de hosts/headers sanitizada às evidências da Fase 1.

## 4. Entrega contínua e gates da fase

- [ ] 4.1 Criar GitHub Actions com build/format, Vitest, MSTest unidade e integração em SQL Server real local/CI, Playwright com proxy/TLS e auditoria de dependências; verificar execução completa do workflow em PR sem segredos no log.
- [x] 4.2 Documentar como iniciar e testar localmente SPA/proxy, API e SQL Server, aplicar migrações em banco descartável e reiniciar/reverter aplicativo sem apagar registros; verificar smoke test e comandos documentados no ambiente de desenvolvimento.
- [x] 4.3 Registrar pendências de privacidade (finalidade, acesso, retenção/descarte), proteção/recuperação de chaves, backup, RPO/RTO, contingência, tarifa/moeda e parâmetros de sessão com responsáveis e bloqueio de produção; verificar checklist assinado/pendente sem declarar liberação.
- [x] 4.4 Elaborar SDD da Fase 1 com rastreabilidade entre `docs/prd/PRD.md` (IDs vigentes), `docs/tdd/tdd.md`, specs e resultados dos testes locais/CI; verificar revisão do SDD antes de abrir PR, registrando homologação Azure como pendente.
- [ ] 4.5 Executar checks integrados locais/CI e apresentar PR da feature branch com PRD, TDD, SDD, evidências, riscos e solicitação de revisão; verificar que não há alegação de publicação Azure nem merge para `main` com gates pendentes.

## Workflow follow-up

- Após revisão, aprovações documentais e verificações exigidas, integrar somente por Pull Request; se houver bloqueios, registrar estado do PR e não contornar proteções.
- Planejar em mudança futura o provisionamento/publicação na Azure, ingress restrito e autenticação entre edge/API, gestão de chaves/segredos, homologação real e gates de produção; não considerar os testes locais equivalentes a essa validação.
- Arquivar a mudança OpenSpec quando os requisitos de revisão e encerramento do projeto forem atendidos.
