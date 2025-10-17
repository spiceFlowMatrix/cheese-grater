import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRedirectPanelComponent } from '@cheese-grater/angular/shared/ui';

@Component({
  selector: 'lib-auth-redirect',
  imports: [CommonModule, AuthRedirectPanelComponent],
  templateUrl: './auth-redirect.component.html',
  styleUrl: './auth-redirect.component.scss',
  standalone: true,
})
export class AuthRedirectComponent {}
