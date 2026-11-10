import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './specs',
  use: { baseURL: process.env.ESTACIONAMENTO_EDGE_URL ?? 'http://127.0.0.1:4200',
    ignoreHTTPSErrors: !!process.env.ESTACIONAMENTO_EDGE_URL },
  webServer: process.env.ESTACIONAMENTO_EDGE_URL ? undefined : {
    command: 'npm --prefix ../../frontend run start -- --host 127.0.0.1 --port 4200',
    url: 'http://127.0.0.1:4200',
    reuseExistingServer: !process.env.CI,
    timeout: 120_000,
  },
});
