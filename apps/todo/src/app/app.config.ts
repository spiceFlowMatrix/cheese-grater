import {
  ApplicationConfig,
  inject,
  PLATFORM_ID,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { appRoutes } from './app.routes';
import {
  provideClientHydration,
  withEventReplay,
} from '@angular/platform-browser';
import {
  includeBearerTokenInterceptor,
  provideKeycloak,
} from 'keycloak-angular';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';

// Shared providers used in both browser and server configurations
const sharedProviders = [
  provideRouter(appRoutes),
  provideClientHydration(withEventReplay()),
  provideZoneChangeDetection({ eventCoalescing: true }),
  provideHttpClient(withInterceptors([includeBearerTokenInterceptor])),
];

// Browser-specific configuration
export const appConfig: ApplicationConfig = {
  providers: [
    provideKeycloak({
      config: {
        url: 'http://localhost:8081',
        realm: 'Test',
        clientId: 'todo-web',
      },
      initOptions: () => {
        const platformId = inject(PLATFORM_ID);
        return {
          onLoad: 'check-sso',
          silentCheckSsoRedirectUri: isPlatformBrowser(platformId)
            ? `${window.location.origin}/assets/silent-check-sso.html`
            : undefined,
        };
      },
    }),
    ...sharedProviders,
  ],
};

// Server-specific configuration
export const serverAppConfig: ApplicationConfig = {
  providers: [
    provideKeycloak({
      config: {
        url: 'http://localhost:8081',
        realm: 'Test',
        clientId: 'todo-web',
      },
    }),

    ...sharedProviders,
  ],
};
