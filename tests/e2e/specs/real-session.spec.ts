import { execFileSync } from 'node:child_process';
import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { expect, test } from '@playwright/test';

test('real edge login, CSRF rejection, API restart and logout', async ({ page }) => {
  test.skip(!process.env.ESTACIONAMENTO_REAL_SESSION, 'Requires isolated Compose and provisioned test employee');
  const credentials = JSON.parse(readFileSync(resolve(__dirname, '../../../local-secrets/test-credentials.json'), 'utf8')) as {
    username: string;
    password: string;
  };
  const origin = new URL(process.env.ESTACIONAMENTO_EDGE_URL!).origin;
  const apiHosts = new Set<string>();
  page.on('request', (request) => {
    if (request.url().includes('/api/')) apiHosts.add(new URL(request.url()).origin);
  });
  await page.goto('/');
  await expect(page.getByRole('heading', { name: 'Entrar' })).toBeVisible();
  await page.getByLabel('Usuário').fill(credentials.username);
  await page.getByLabel('Senha').fill(credentials.password);
  await page.getByRole('button', { name: 'Entrar' }).click();
  await expect(page.getByText(`Sessão ativa: ${credentials.username} (caixa)`)).toBeVisible();

  const cookies = await page.context().cookies();
  expect(cookies.every((cookie) => cookie.domain === 'localhost' && cookie.secure)).toBe(true);
  expect(cookies.find((cookie) => cookie.name === 'Estacionamento.Session')?.httpOnly).toBe(true);
  expect([...apiHosts]).toEqual([origin]);

  const invalidCsrf = await page.evaluate(async () => (await fetch('/api/v1/session/logout', { method: 'POST' })).status);
  expect(invalidCsrf).toBe(403);
  await expect(page.getByText(`Sessão ativa: ${credentials.username} (caixa)`)).toBeVisible();

  if (process.env.ESTACIONAMENTO_RESTART_TEST) {
    execFileSync('docker', ['compose', 'restart', 'api'], { cwd: resolve(__dirname, '../../..'), stdio: 'ignore' });
    await expect.poll(async () => {
      try { return (await page.request.get(`${origin}/api/v1/session`, { ignoreHTTPSErrors: true })).status(); }
      catch { return 0; }
    }, { timeout: 60_000 }).toBe(200);
  }

  await page.getByRole('button', { name: 'Verificar sessão' }).click();
  await expect(page.getByText(`Sessão ativa: ${credentials.username} (caixa)`)).toBeVisible();
  await page.getByRole('button', { name: 'Sair' }).click();
  await expect(page.getByRole('heading', { name: 'Entrar' })).toBeVisible();
  expect((await page.request.get(`${origin}/api/v1/session`, { ignoreHTTPSErrors: true })).status()).toBe(401);
});
