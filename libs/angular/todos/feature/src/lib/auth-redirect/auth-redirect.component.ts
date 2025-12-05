import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import Keycloak from 'keycloak-js';
import { AuthRedirectPanelComponent } from '@cheese-grater/angular/shared/ui';

@Component({
  selector: 'lib-auth-redirect',
  imports: [CommonModule, AuthRedirectPanelComponent],
  templateUrl: './auth-redirect.component.html',
  styleUrl: './auth-redirect.component.scss',
  standalone: true,
})
export class AuthRedirectComponent {
  private readonly keycloak = inject(Keycloak);
  public authenticated = this.keycloak?.authenticated ?? false;

  async onLogin() {
    await this.keycloak.login();
    this.authenticated = this.keycloak?.authenticated ?? false;
  }

  async onLogout() {
    await this.keycloak.logout({
      redirectUri: window.location.origin,
    });
    this.authenticated = this.keycloak?.authenticated ?? false;
  }
}
