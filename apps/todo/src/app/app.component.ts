import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import Keycloak from 'keycloak-js';
import { HeaderComponent } from './header/header.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
  imports: [RouterModule, HeaderComponent, MatButtonModule],
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  private readonly keycloak = inject(Keycloak);
  title = 'todo';

  login() {
    this.keycloak.login();
  }
}
