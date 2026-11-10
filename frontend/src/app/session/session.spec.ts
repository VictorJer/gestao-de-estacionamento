import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { csrfInterceptor } from './csrf.interceptor';
import { SessionService } from './session.service';

describe('SessionService and CSRF', () => {
  let service: SessionService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(withInterceptors([csrfInterceptor])), provideHttpClientTesting()],
    });
    service = TestBed.inject(SessionService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('handles unauthenticated, login, expiry, and logout without retaining a password', async () => {
    const initial = service.refresh();
    http.expectOne('/api/v1/session').flush({}, { status: 401, statusText: 'Unauthorized' });
    await initial;
    expect(service.error()).toBe('');

    const login = service.login('caixa', 'secret');
    http.expectOne('/api/v1/session/antiforgery').flush({ requestToken: 'before-login' });
    const request = http.expectOne('/api/v1/session/login');
    expect(request.request.headers.get('X-CSRF-TOKEN')).toBe('before-login');
    expect(request.request.body).toEqual({ username: 'caixa', password: 'secret' });
    request.flush({ username: 'caixa', roles: ['caixa'] });
    await login;
    expect(service.authenticated()).toBe(true);

    const expiry = service.refresh();
    http.expectOne('/api/v1/session').flush({}, { status: 401, statusText: 'Unauthorized' });
    await expiry;
    expect(service.session()).toBeNull();
    expect(service.error()).toContain('expirou');

    const invalid = service.login('caixa', 'wrong');
    http.expectOne('/api/v1/session/antiforgery').flush({ requestToken: 'new' });
    http.expectOne('/api/v1/session/login').flush({}, { status: 401, statusText: 'Unauthorized' });
    await invalid;
    expect(service.error()).toBe('Usuário ou senha inválidos.');

    const retry = service.login('caixa', 'secret');
    http.expectOne('/api/v1/session/antiforgery').flush({ requestToken: 'retry' });
    http.expectOne('/api/v1/session/login').flush({ username: 'caixa', roles: ['caixa'] });
    await retry;
    const logout = service.logout();
    http.expectOne('/api/v1/session/antiforgery').flush({ requestToken: 'after-login' });
    const logoutRequest = http.expectOne('/api/v1/session/logout');
    expect(logoutRequest.request.headers.get('X-CSRF-TOKEN')).toBe('after-login');
    logoutRequest.flush(null);
    await logout;
    expect(service.authenticated()).toBe(false);
  });
});
