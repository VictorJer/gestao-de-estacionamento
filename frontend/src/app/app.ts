import { Component, inject, OnInit, signal } from '@angular/core';
import { form, FormField, required } from '@angular/forms/signals';
import { RouterOutlet } from '@angular/router';
import { SessionService } from './session/session.service';

@Component({
  imports: [RouterOutlet, FormField],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  protected readonly session = inject(SessionService);
  protected readonly credentials = signal({ username: '', password: '' });
  protected readonly loginForm = form(this.credentials, (path) => {
    required(path.username);
    required(path.password);
  });

  ngOnInit(): void {
    void this.session.refresh();
  }

  protected async login(event: Event): Promise<void> {
    event.preventDefault();
    if (this.loginForm().invalid() || this.session.busy()) return;
    const { username, password } = this.credentials();
    try {
      await this.session.login(username, password);
    } finally {
      this.credentials.update((value) => ({ ...value, password: '' }));
    }
  }

  protected logout(): void {
    void this.session.logout();
  }
}
