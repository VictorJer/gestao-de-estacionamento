import { expect, test } from '@playwright/test';

test('mostra a fundação sem expor jornadas operacionais incompletas', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('heading', { name: 'Gestão de Estacionamento' })).toBeVisible();
  await expect(page.getByText('A operação de estacionamento ainda não está disponível.')).toBeVisible();
});
