import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'lib-auth-redirect-panel',
  imports: [CommonModule, MatCardModule, MatButtonModule],
  templateUrl: './auth-redirect-panel.component.html',
  styleUrl: './auth-redirect-panel.component.scss',
})
export class AuthRedirectPanelComponent {
  showLogin = input<boolean>(true);
  showSignup = input<boolean>(true);
  titleLabel = input<string>('Please log in or sign up to continue.');
  loginButtonLabel = input<string>('Log In');
  signupButtonLabel = input<string>('Sign Up');
  loginClicked = output();
  signupClicked = output();
}
