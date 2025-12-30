import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import Keycloak from 'keycloak-js';
import { SpaAuthConfigService } from './core/services/spa-auth-config.service';

@Component({
  imports: [RouterModule],
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  private readonly keycloak = inject(Keycloak);
  private readonly spaAuthConfigService = inject(SpaAuthConfigService);
  title = 'todo';

  async login() {
    const config = await this.spaAuthConfigService.loadConfig();
    await this.keycloak.login({ redirectUri: config.redirectUri });
  }

  async logout() {
    const config = await this.spaAuthConfigService.loadConfig();
    await this.keycloak.logout({ redirectUri: config.logoutRedirectUri });
  }
}
