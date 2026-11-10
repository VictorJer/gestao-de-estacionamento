import { HttpBackend, HttpClient, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { switchMap } from 'rxjs';

// Always request a fresh token: ASP.NET binds tokens to the current identity,
// so a token issued before login cannot be reused for logout.
export const csrfInterceptor: HttpInterceptorFn = (request, next) => {
  if (!request.url.startsWith('/api/') || !['POST', 'PUT', 'PATCH', 'DELETE'].includes(request.method)) {
    return next(request);
  }

  const bootstrap = new HttpClient(inject(HttpBackend));
  return bootstrap.get<{ requestToken: string }>('/api/v1/session/antiforgery').pipe(
    switchMap(({ requestToken }) => next(request.clone({
      setHeaders: { 'X-CSRF-TOKEN': requestToken },
    }))),
  );
};
