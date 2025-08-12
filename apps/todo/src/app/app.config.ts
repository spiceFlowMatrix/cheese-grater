import {
  APP_INITIALIZER,
  ApplicationConfig,
  inject,
  PLATFORM_ID,
  provideAppInitializer,
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
import Keycloak from 'keycloak-js';

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
    }),
    provideAppInitializer(() => {
      if (!isPlatformBrowser(inject(PLATFORM_ID))) {
        // Skip initialization on server
        return Promise.resolve(true);
      }
      const keycloak = inject(Keycloak);
      return keycloak.init({
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
      });
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
