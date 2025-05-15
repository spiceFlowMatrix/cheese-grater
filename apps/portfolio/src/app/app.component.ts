import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NxWelcomeComponent } from './nx-welcome.component';
import { isPlatformBrowser } from '@angular/common';

@Component({
  imports: [NxWelcomeComponent, RouterModule],
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'portfolio';

  constructor(@Inject(PLATFORM_ID) private platformId: object) {
    if (isPlatformBrowser(this.platformId)) {
      console.log(window.location); // Browser-only
    }
  }
}
