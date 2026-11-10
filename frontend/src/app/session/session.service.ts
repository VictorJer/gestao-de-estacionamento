import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export interface EmployeeSession {
  username: string;
  roles: string[];
}

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly http = inject(HttpClient);
  readonly session = signal<EmployeeSession | null>(null);
  readonly busy = signal(false);
  readonly error = signal('');
  readonly expired = signal(false);
  readonly authenticated = computed(() => this.session() !== null);

  async refresh(): Promise<void> {
    this.busy.set(true);
    try {
      this.session.set(await firstValueFrom(this.http.get<EmployeeSession>('/api/v1/session')));
      this.error.set('');
      this.expired.set(false);
    } catch (error) {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        this.expired.set(this.authenticated());
        this.session.set(null);
        this.error.set(this.expired() ? 'Sua sessão expirou. Entre novamente.' : '');
      } else {
        this.error.set('Não foi possível consultar a sessão. Tente novamente.');
      }
    } finally {
      this.busy.set(false);
    }
  }

  async login(username: string, password: string): Promise<void> {
    this.busy.set(true);
    this.error.set('');
    try {
      this.session.set(await firstValueFrom(this.http.post<EmployeeSession>('/api/v1/session/login', { username, password })));
      this.expired.set(false);
    } catch (error) {
      this.session.set(null);
      this.error.set(error instanceof HttpErrorResponse && error.status === 401
        ? 'Usuário ou senha inválidos.'
        : 'Não foi possível entrar. Tente novamente.');
    } finally {
      this.busy.set(false);
    }
  }

  async logout(): Promise<void> {
    this.busy.set(true);
    this.error.set('');
    try {
      await firstValueFrom(this.http.post<void>('/api/v1/session/logout', {}));
      this.session.set(null);
      this.expired.set(false);
    } catch {
      this.error.set('Não foi possível sair. Tente novamente.');
    } finally {
      this.busy.set(false);
    }
  }
}
