import { expect, test } from '@playwright/test';

test('bootstrap reaches the API via the HTTPS SPA origin without a third-party cookie', async ({ page }) => {
  test.skip(!process.env.ESTACIONAMENTO_EDGE_URL, 'Requires the isolated Compose environment');
  const origin = new URL(process.env.ESTACIONAMENTO_EDGE_URL!).origin;
  const apiRequests: string[] = [];
  page.on('request', (request) => {
    if (request.url().includes('/api/')) apiRequests.push(request.url());
  });
  await page.goto('/');
  await expect(page.getByRole('heading', { name: 'Entrar' })).toBeVisible();
  const bootstrap = await page.evaluate(async () => {
    const response = await fetch('/api/v1/session/antiforgery');
    return { status: response.status, token: (await response.json()).requestToken as string };
  });
  expect(bootstrap.status).toBe(200);
  expect(bootstrap.token.length).toBeGreaterThan(10);
  expect(apiRequests.length).toBeGreaterThan(0);
  expect(apiRequests.every((url) => new URL(url).origin === origin)).toBe(true);
  const cookies = await page.context().cookies();
  expect(cookies.every((cookie) => cookie.domain === 'localhost' && cookie.secure)).toBe(true);
  expect(cookies.find((cookie) => cookie.name === 'Estacionamento.Session')).toBeUndefined();
});
