import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRedirectPanelComponent } from '@cheese-grater/angular/shared/ui';

@Component({
  selector: 'app-auth-redirect',
  imports: [CommonModule, AuthRedirectPanelComponent],
  templateUrl: './auth-redirect.component.html',
  styleUrl: './auth-redirect.component.scss',
})
export class AuthRedirectComponent {}
