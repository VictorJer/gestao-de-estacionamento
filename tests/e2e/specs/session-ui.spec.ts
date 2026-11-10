import { expect, test } from '@playwright/test';

// Contract-level UI test with mocked API. Real SQL/proxy/TLS E2E is tracked separately in task 3.4.
test('login, logout and keyboard navigation use the session API and fresh CSRF tokens', async ({ page }) => {
  let signedIn = false;
  let tokenCount = 0;
  const mutations: { path: string; token: string | null }[] = [];
  await page.route('**/api/v1/session**', async (route) => {
    const { pathname } = new URL(route.request().url());
    const method = route.request().method();
    if (pathname.endsWith('/antiforgery')) {
      await route.fulfill({ json: { requestToken: `token-${++tokenCount}` } });
    } else if (method === 'GET') {
      await route.fulfill(signedIn
        ? { json: { username: 'caixa.teste', roles: ['caixa'] } }
        : { status: 401, body: '' });
    } else {
      mutations.push({ path: pathname, token: route.request().headers()['x-csrf-token'] ?? null });
      if (pathname.endsWith('/login')) {
        signedIn = true;
        await route.fulfill({ json: { username: 'caixa.teste', roles: ['caixa'] } });
      } else {
        signedIn = false;
        await route.fulfill({ status: 204, body: '' });
      }
    }
  });

  await page.goto('/');
  await expect(page.getByRole('heading', { name: 'Entrar' })).toBeVisible();
  await page.keyboard.press('Tab');
  await expect(page.getByLabel('Usuário')).toBeFocused();
  await page.keyboard.type('caixa.teste');
  await page.keyboard.press('Tab');
  await expect(page.getByLabel('Senha')).toBeFocused();
  await page.keyboard.type('SenhaDeTeste!12345');
  await page.keyboard.press('Enter');
  await expect(page.getByText('Sessão ativa: caixa.teste (caixa)')).toBeVisible();
  await expect(page.getByRole('button', { name: 'Sair' })).toBeVisible();
  await page.getByRole('button', { name: 'Sair' }).focus();
  await page.keyboard.press('Enter');
  await expect(page.getByRole('heading', { name: 'Entrar' })).toBeVisible();
  expect(mutations).toEqual([
    { path: '/api/v1/session/login', token: 'token-1' },
    { path: '/api/v1/session/logout', token: 'token-2' },
  ]);
});
