# Evidência local — Fase 1 (10/11/2026)

Ambiente de desenvolvimento, não homologação de produção nem Azure. `docker compose up --build -d` subiu SQL Server 2022, API .NET 10 e edge Nginx com Angular 22. Apenas `127.0.0.1:8444 → edge:443` foi publicada; API (`api:8443`) e SQL (`sql:1433`) permaneceram em rede Docker `internal`. Certificado local autoassinado válido para `localhost` e `api` foi gerado em volume privado; Nginx verifica o certificado ao acessar a API via HTTPS. O navegador de teste ignorou apenas a confiança no certificado autoassinado local.

Configuração sanitizada: `Security:AllowedOrigin=https://localhost:8444`; `Security:KnownProxy=172.30.96.10` (IP fixo do edge na rede interna); `X-Forwarded-Proto=https`, `X-Forwarded-Host=$http_host`, `X-Forwarded-For=$remote_addr`; cabeçalhos de identidade do cliente removidos no proxy. Nenhuma senha, token ou cookie consta desta evidência.

Verificações executadas:

- Playwright `bootstrap reaches the API via the HTTPS SPA origin without a third-party cookie`: passou; bootstrap por `/api/v1/session/antiforgery`, chamadas na origem da SPA, cookie `Secure` apenas para `localhost`.
- Playwright `real edge login, CSRF rejection, API restart and logout`: passou; usuário de teste provisionado fora do Git, login real, cookie de sessão `Secure`/`HttpOnly`, POST de logout sem CSRF = `403`, sessão preservada após `docker compose restart api`, logout válido e sessão posterior = `401`.
- Suíte Playwright completa contra `https://localhost:8444` com `ESTACIONAMENTO_REAL_SESSION=1` e `ESTACIONAMENTO_RESTART_TEST=1`: **4 passaram / 0 falharam**, incluindo cenário de teclado e UI simulada. O teste real usa SQL Server em contêiner, sem mock da API.
- `curl` pela borda com `X-User-Role: administrador` e `X-Forwarded-Proto: http` sem sessão: `401`, `Cache-Control: no-store`. HTTP em `:8444` não processou operação (`400` do listener TLS); conexão direta do host a `localhost:8443` falhou; `docker inspect` da API não mostrou portas publicadas.
- Inspeção dos logs de API/edge por comparação com credencial de teste e marcadores de cookie/token: nenhuma ocorrência de senha, usuário de teste, `Estacionamento.Session=` ou `X-CSRF-TOKEN:`. O key ring permanece em volume Docker separado e reutilizado na reinicialização.

Pendências: evidência equivalente no GitHub Actions, gestão/backup/rotação de chaves e certificados de produção, restauração, validação de topologia Azure e revisão de privacidade. Não usar esta evidência como aprovação de produção.
